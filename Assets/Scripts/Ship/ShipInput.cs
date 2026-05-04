using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 우주선 발사 입력 (앵그리버드 슬링샷 방식).
/// 고정 원점에서 뒤로 드래그 → 반대 방향으로 발사. 당김 거리 MaxPullDistance로 클램프.
/// MinPullDistance 미만이면 발사 취소.
/// 스크린→월드 변환은 CameraService.ScreenToWorld2D에 위임.
/// </summary>
public class ShipInput : MonoBehaviour
{
    private float _celestialYMin = 0f;

    /// <summary>발사 원점 X 좌표. CombatManager가 화면 왼쪽 끝 기준으로 설정.</summary>
    public float launchOriginX = 0f;

    public float celestialYMin
    {
        get => _celestialYMin;
        set { _celestialYMin = value; LaunchOrigin = new Vector2(launchOriginX, value); }
    }

    /// <summary>발사 원점. 초기값은 지면 원점(launchOriginX, celestialYMin). 착지 시 ShipController가 갱신.</summary>
    public Vector2 LaunchOrigin { get; set; }

    /// <summary>도킹 모드일 때 슬링샷 대신 단순 클릭으로 재발사.</summary>
    public bool IsDockedMode { get; set; }
    public event System.Action OnDockedClick;

    public System.Action<Vector2> OnAimStart;
    /// <summary>origin, clampedPullPos, pullRatio(0~1) — pullPos는 MaxPullDistance로 클램프됨.</summary>
    public System.Action<Vector2, Vector2, float> OnAimUpdate;
    /// <summary>origin, launchDirection(당김의 반대), power</summary>
    public System.Action<Vector2, Vector2, float> OnLaunch;
    public System.Action OnAimCancel;

    private bool _aiming;
    private Mouse _mouse;

    void Start()
    {
        _mouse = Mouse.current;
        LaunchOrigin = new Vector2(launchOriginX, celestialYMin);
    }

    void Update()
    {
        if (_mouse == null) _mouse = Mouse.current;
        if (_mouse == null || CameraService.Instance == null) return;

        // 도킹 모드: 클릭만 감지
        if (IsDockedMode)
        {
            if (_mouse.leftButton.wasPressedThisFrame)
                OnDockedClick?.Invoke();
            return;
        }

        Vector2 mp = CameraService.Instance.ScreenToWorld2D(_mouse.position.ReadValue());

        if (_mouse.leftButton.wasPressedThisFrame && !_aiming)
        {
            _aiming = true;
            OnAimStart?.Invoke(LaunchOrigin);
        }

        if (_aiming && _mouse.leftButton.isPressed)
        {
            Vector2 origin = LaunchOrigin;
            Vector2 delta = mp - origin;
            float dist = delta.magnitude;
            float maxPull = GameConstants.ShipPhysics.MaxPullDistance;
            float clampedDist = Mathf.Min(dist, maxPull);
            Vector2 clampedPullPos = dist > 0f
                ? origin + delta.normalized * clampedDist
                : origin;
            float pullRatio = maxPull > 0f ? clampedDist / maxPull : 0f;
            OnAimUpdate?.Invoke(origin, clampedPullPos, pullRatio);
        }

        if (_mouse.leftButton.wasReleasedThisFrame && _aiming)
        {
            _aiming = false;

            Vector2 origin = LaunchOrigin;
            Vector2 delta = mp - origin;
            float dist = delta.magnitude;

            if (dist < GameConstants.ShipPhysics.MinPullDistance)
            {
                Debug.Log($"[ShipInput] 발사 취소: 당김 거리 {dist:F2} < {GameConstants.ShipPhysics.MinPullDistance}");
                OnAimCancel?.Invoke();
                return;
            }

            float clampedDist = Mathf.Min(dist, GameConstants.ShipPhysics.MaxPullDistance);
            Vector2 launchDir = (-delta).normalized;
            float power = clampedDist * GameConstants.ShipPhysics.LaunchPowerMultiplier;
            Debug.Log($"[ShipInput] 발사: mp={mp} pull={dist:F2} clamp={clampedDist:F2} power={power:F1} dir={launchDir}");
            OnLaunch?.Invoke(origin, launchDir, power);
        }
    }

    /// <summary>외부에서 조준을 강제로 취소할 때 호출</summary>
    public void ForceCancel()
    {
        if (_aiming)
        {
            _aiming = false;
            OnAimCancel?.Invoke();
        }
    }
}
