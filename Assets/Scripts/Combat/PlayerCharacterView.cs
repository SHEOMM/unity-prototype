using UnityEngine;

/// <summary>
/// 플레이어 본체 (Blue Witch 등) 시각 + 상태 머신.
/// 지상 좌측에 배치되어 상태별 애니메이션으로 반응.
///
/// 위치·스케일은 **카메라와 divider Y 기준으로 동적 계산** (하드코딩 금지):
///   x = Lerp(camera.left, camera.right, xAnchor)
///   y = Lerp(camera.bottom, dividerY, yAnchor)  (지상 영역 내부)
///   scale = targetWorldHeight / sprite.native_height
///
/// 상태 전이 우선순위:
///   TakeDamage (타이머) > Attack (비행중) > Charge (조준중) > Idle
///
/// 구독:
///   ShipInput       — OnAimStart / OnAimCancel / OnLaunch
///   ShipController  — OnFlightStarted / OnShipComplete
///   PlayerState     — OnHPChanged (감소 감지)
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerCharacterView : MonoBehaviour
{
    [Tooltip("상태별 애니메이션 클립 묶음. 주입 필수.")]
    public CharacterAnimationSet animations;

    [Header("배치 (카메라 뷰 상대 비율)")]
    [Range(0f, 1f)]
    [Tooltip("0=카메라 좌측 가장자리, 1=우측. 기본 0.08 = 좌측 8%.")]
    public float xAnchor = 0.08f;

    [Range(0f, 1f)]
    [Tooltip("0=카메라 바닥, 1=divider(천상/지상 경계). 기본 0.5 = 지상 중앙.")]
    public float yAnchor = 0.5f;

    [Tooltip("월드 유닛 기준 원하는 캐릭터 높이. 스프라이트 native height로 스케일 계산.")]
    public float targetWorldHeight = 2.5f;

    [Tooltip("정렬 순서. 기본 EnemyBody 레벨.")]
    public int sortingOrder = GameConstants.SortingOrder.EnemyBody;

    CharacterAnimator _animator;
    SpriteRenderer _sr;

    // 상태 플래그
    bool _aiming;
    bool _flying;
    float _takeDamageTimer;
    float _prevHP;
    CharacterAnimationState _currentState;
    bool _stateInitialized;  // 초기 1회 강제 적용 구분

    // 구독 해제용 캐시
    ShipInput _shipInput;
    ShipController _shipController;
    PlayerState _playerState;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.sortingOrder = sortingOrder;
        _animator = gameObject.AddComponent<CharacterAnimator>();

        PositionAndScale();
        Subscribe();
        ApplyState(ComputeState());  // 초기 sprite 강제 주입 (stateInitialized=false → 통과)
    }

    void Update()
    {
        if (_takeDamageTimer > 0f) _takeDamageTimer -= Time.deltaTime;
        ApplyState(ComputeState());
    }

    void PositionAndScale()
    {
        var cam = CameraService.Instance?.Camera;
        if (cam == null)
        {
            Debug.LogWarning("[PlayerCharacterView] CameraService.Camera 없음 — 기본 위치(0,0) 사용");
            return;
        }

        float camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect;
        float camRight = cam.transform.position.x + cam.orthographicSize * cam.aspect;
        float camBottom = cam.transform.position.y - cam.orthographicSize;
        float groundTopY = GetGroundTopY(cam);

        float x = Mathf.Lerp(camLeft, camRight, xAnchor);
        float y = Mathf.Lerp(camBottom, groundTopY, yAnchor);
        transform.position = new Vector3(x, y, 0f);

        // 스프라이트 native 높이 기반 스케일 — targetWorldHeight에 맞춤
        var idleClip = animations != null ? animations.Get(CharacterAnimationState.Idle) : null;
        var icon = idleClip != null ? idleClip.Icon : null;
        if (icon != null)
        {
            float nativeH = icon.bounds.size.y;  // PPU 반영된 월드 단위
            if (nativeH > 0.0001f)
                transform.localScale = Vector3.one * (targetWorldHeight / nativeH);
        }
    }

    /// <summary>지상 영역의 상단 Y = divider 라인. CombatManager에서 계산, 없으면 카메라 중심.</summary>
    static float GetGroundTopY(Camera cam)
    {
        var cm = CombatManager.Instance;
        if (cm != null) return cm.celestialYCenter - cm.celestialRadius;
        return cam.transform.position.y;
    }

    CharacterAnimationState ComputeState()
    {
        if (_takeDamageTimer > 0f) return CharacterAnimationState.TakeDamage;
        if (_flying)               return CharacterAnimationState.Attack;
        if (_aiming)               return CharacterAnimationState.Charge;
        return CharacterAnimationState.Idle;
    }

    void ApplyState(CharacterAnimationState state)
    {
        if (_stateInitialized && _currentState == state) return;
        _stateInitialized = true;
        _currentState = state;

        var clip = animations != null ? animations.Get(state) : null;
        if (clip == null && state != CharacterAnimationState.Idle)
            clip = animations != null ? animations.Get(CharacterAnimationState.Idle) : null;  // fallback
        if (clip != null) _animator.Play(clip);
    }

    // ── 이벤트 구독 ────────────────────────────────────────────

    void Subscribe()
    {
        _shipInput = CombatManager.Instance != null ? CombatManager.Instance.GetComponent<ShipInput>() : null;
        if (_shipInput != null)
        {
            _shipInput.OnAimStart  += HandleAimStart;
            _shipInput.OnAimCancel += HandleAimCancel;
            _shipInput.OnLaunch    += HandleLaunch;
        }

        _shipController = ShipController.Instance;
        if (_shipController != null)
        {
            _shipController.OnFlightStarted += HandleFlightStarted;
            _shipController.OnShipComplete  += HandleShipComplete;
        }

        _playerState = PlayerState.Instance;
        if (_playerState != null)
        {
            _prevHP = _playerState.currentHP;
            _playerState.OnHPChanged += HandleHPChanged;
        }
    }

    void OnDestroy()
    {
        if (_shipInput != null)
        {
            _shipInput.OnAimStart  -= HandleAimStart;
            _shipInput.OnAimCancel -= HandleAimCancel;
            _shipInput.OnLaunch    -= HandleLaunch;
        }
        if (_shipController != null)
        {
            _shipController.OnFlightStarted -= HandleFlightStarted;
            _shipController.OnShipComplete  -= HandleShipComplete;
        }
        if (_playerState != null)
            _playerState.OnHPChanged -= HandleHPChanged;
    }

    // ── 핸들러 ─────────────────────────────────────────────────

    void HandleAimStart(Vector2 _)        { _aiming = true; }
    void HandleAimCancel()                { _aiming = false; }
    void HandleLaunch(Vector2 _, Vector2 __, float ___) { _aiming = false; }
    void HandleFlightStarted()            { _flying = true; }
    void HandleShipComplete(SpellResult _) { _flying = false; }

    void HandleHPChanged(float current, float max)
    {
        if (current < _prevHP)
        {
            var clip = animations != null ? animations.Get(CharacterAnimationState.TakeDamage) : null;
            _takeDamageTimer = clip != null ? clip.Duration : 0.3f;
        }
        _prevHP = current;
    }
}
