using System;
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
