using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 게임의 유일한 Main Camera 래퍼. PersistentScene의 Main Camera에 부착한다.
/// 모든 게임 코드가 Camera.main 대신 CameraService.Instance.Camera를 사용.
///
/// 책임:
/// 1. 타입 안전한 Camera 접근 (null 체크 최소화)
/// 2. 스크린→월드 변환을 perspective/orthographic 양쪽에서 정확히 처리
/// 3. SceneEnvironment 적용 (씬 진입 시 SceneBootBase가 호출)
/// 4. 일시 카메라 뷰 변경을 IDisposable 스코프로 안전하게 관리 (예: 맵 화면 센터링)
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraService : MonoBehaviour
{
    public static CameraService Instance { get; private set; }

    public Camera Camera { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Camera = GetComponent<Camera>();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>마우스 등 스크린 좌표를 월드 z=0 평면 좌표로 변환. ortho/persp 무관.</summary>
    public Vector2 ScreenToWorld2D(Vector2 screenPos)
    {
        if (Camera == null) return Vector2.zero;
        Ray ray = Camera.ScreenPointToRay(screenPos);
        if (Mathf.Abs(ray.direction.z) < 1e-6f)
            return new Vector2(ray.origin.x, ray.origin.y);
        float t = -ray.origin.z / ray.direction.z;
        Vector3 world = ray.origin + ray.direction * t;
        return new Vector2(world.x, world.y);
    }

    /// <summary>SceneEnvironment에서 카메라 파라미터를 적용. 씬 진입 시 SceneBootBase가 호출.</summary>
    public void ApplyEnvironment(SceneEnvironment env)
    {
        if (env == null || Camera == null) return;
        Camera.transform.position = env.cameraPosition;
        Camera.transform.rotation = Quaternion.identity;
        Camera.orthographic = true;
        Camera.orthographicSize = env.orthographicSize;
        Camera.backgroundColor = env.backgroundColor;
        Camera.clearFlags = CameraClearFlags.SolidColor;
        _normalPosition = Camera.transform.position;
        _normalOrthoSize = Camera.orthographicSize;
    }

    // ── 조준 줌 ──
    private Vector3 _normalPosition;
    private float _normalOrthoSize;
    private Coroutine _zoomCoroutine;
    private const float ZoomDuration = 0.5f;
    private const float ZoomFactor = 2f;

    /// <summary>카메라 줌 전환 중이면 true.</summary>
    public bool IsZooming => _zoomCoroutine != null;

    /// <summary>
    /// 조준 시작 시 호출. 2× 줌아웃.
    /// 규칙: 발사점이 가로 중심, 발사점의 화면 Y 비율이 줌 전후로 동일.
    /// </summary>
    public void ZoomToAim(Vector2 launchOrigin)
    {
        if (Camera == null) return;
        StopShake();
        if (_zoomCoroutine != null) StopCoroutine(_zoomCoroutine);
        float targetSize = _normalOrthoSize * ZoomFactor;
        float z = Camera.transform.position.z;
        // "발사점의 화면 Y 비율 유지" = 정상 시점의 Y에 대해 launchOrigin을 반사
        float targetY = 2f * _normalPosition.y - launchOrigin.y;
        _zoomCoroutine = StartCoroutine(LerpCameraRoutine(
            new Vector3(launchOrigin.x, targetY, z),
            targetSize));
    }

    /// <summary>발사체 소멸 / 조준 취소 시 호출. 원래 뷰로 복귀.</summary>
    public void ZoomToNormal()
    {
        if (Camera == null) return;
        StopShake();
        if (_zoomCoroutine != null) StopCoroutine(_zoomCoroutine);
        _zoomCoroutine = StartCoroutine(LerpCameraRoutine(_normalPosition, _normalOrthoSize));
    }

    /// <summary>진행 중인 셰이크 코루틴이 있으면 중단. Zoom 호출 전에 항상 선행.</summary>
    void StopShake()
    {
        if (_shakeCoroutine == null) return;
        StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = null;
    }

    IEnumerator LerpCameraRoutine(Vector3 targetPos, float targetSize)
    {
        Vector3 startPos = Camera.transform.position;
        float startSize = Camera.orthographicSize;
        float elapsed = 0f;
        while (elapsed < ZoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / ZoomDuration));
            Camera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            Camera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }
        Camera.transform.position = targetPos;
        Camera.orthographicSize = targetSize;
        _zoomCoroutine = null;
    }

    private Coroutine _shakeCoroutine;

    /// <summary>
    /// 카메라를 짧은 시간 흔든다. Perlin 기반 랜덤 오프셋을 원본 위치에 더했다가 종료 시 복원.
    /// Phase 7 screen_flash 시너지 등 강한 임팩트에서 사용.
    /// </summary>
    public void Shake(float strength, float duration)
    {
        if (Camera == null) return;
        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeRoutine(strength, duration));
    }

    IEnumerator ShakeRoutine(float strength, float duration)
    {
        Vector3 origin = Camera.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float remaining = Mathf.Max(0f, 1f - elapsed / duration);
            float ox = (Mathf.PerlinNoise(Time.time * 40f, 0f) - 0.5f) * 2f * strength * remaining;
            float oy = (Mathf.PerlinNoise(0f, Time.time * 40f) - 0.5f) * 2f * strength * remaining;
            Camera.transform.position = origin + new Vector3(ox, oy, 0);
            yield return null;
        }
        Camera.transform.position = origin;
        _shakeCoroutine = null;
    }

    /// <summary>
    /// 일시적으로 카메라를 다른 위치/크기로 변경. 반환된 IDisposable을 Dispose하면 복원.
    /// MapView의 `using (CameraService.Instance.PushTemporaryView(...))` 또는 필드 보관 후 Hide에서 Dispose.
    /// </summary>
    public IDisposable PushTemporaryView(Vector3 pos, float orthoSize)
    {
        if (Camera == null) return NullScope.Instance;
        var savedPos = Camera.transform.position;
        var savedSize = Camera.orthographicSize;
        Camera.transform.position = pos;
        Camera.orthographicSize = orthoSize;
        return new TemporaryViewScope(this, savedPos, savedSize);
    }

    private sealed class TemporaryViewScope : IDisposable
    {
        private readonly CameraService _svc;
        private readonly Vector3 _pos;
        private readonly float _size;
        private bool _disposed;

        public TemporaryViewScope(CameraService svc, Vector3 pos, float size)
        {
            _svc = svc;
            _pos = pos;
            _size = size;
        }

        public void Dispose()
        {
            if (_disposed || _svc == null || _svc.Camera == null) return;
            _disposed = true;
            _svc.Camera.transform.position = _pos;
            _svc.Camera.orthographicSize = _size;
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new NullScope();
        public void Dispose() { }
    }
}
