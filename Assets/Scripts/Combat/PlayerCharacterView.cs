using UnityEngine;

/// <summary>
/// 플레이어 본체 (Blue Witch 등) 시각 + 상태 머신.
/// 지상 좌측에 고정 배치되어 상태별 애니메이션으로 반응.
///
/// 상태 전이 규칙 (우선순위 높은 순):
///   TakeDamage  — HP 감소 감지 후 짧은 오버라이드 (takeDamage clip duration)
///   Attack      — ShipController 비행 중 (OnFlightStarted ~ OnShipComplete)
///   Charge      — ShipInput 조준 중 (OnAimStart ~ OnAimCancel/OnLaunch)
///   Idle        — 그 외 모든 상황
///
/// 구독:
///   ShipInput     — OnAimStart / OnAimCancel / OnLaunch
///   ShipController — OnFlightStarted / OnShipComplete
///   PlayerState    — OnHPChanged (감소 감지)
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerCharacterView : MonoBehaviour
{
    [Tooltip("상태별 애니메이션 클립 묶음. 주입 필수.")]
    public CharacterAnimationSet animations;

    [Tooltip("지상 좌측 배치 좌표.")]
    public Vector2 spawnPosition = new Vector2(-9f, -3f);

    [Tooltip("시각 스케일 (22px 스프라이트 기준 5배 = 월드 ~1.1 유닛).")]
    public float visualScale = 5f;

    [Tooltip("지상 개체 수준 정렬 순서 (Enemy/Ally와 동일 레이어).")]
    public int sortingOrder = GameConstants.SortingOrder.EnemyBody;

    CharacterAnimator _animator;
    SpriteRenderer _sr;

    // 상태 플래그
    bool _aiming;
    bool _flying;
    float _takeDamageTimer;
    float _prevHP;
    CharacterAnimationState _currentState = CharacterAnimationState.Idle;

    // 구독 해제용 캐시
    ShipInput _shipInput;
    ShipController _shipController;
    PlayerState _playerState;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.sortingOrder = sortingOrder;
        transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0f);
        transform.localScale = Vector3.one * visualScale;

        _animator = gameObject.AddComponent<CharacterAnimator>();
        Subscribe();
        ApplyState(CharacterAnimationState.Idle);
    }

    void Update()
    {
        if (_takeDamageTimer > 0f) _takeDamageTimer -= Time.deltaTime;
        ApplyState(ComputeState());
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
        if (_currentState == state) return;
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
