using UnityEngine;
using System.Reflection;

/// <summary>
/// 행성 런타임. 궤도 위에서 공전하며 슬래시 충돌 대상이 된다.
/// 순수 모델 — UI(라벨, HUD)는 별도 View 컴포넌트가 담당.
/// </summary>
public class PlanetBody : MonoBehaviour, IGravitySource
{
    public PlanetSO Planet { get; private set; }
    private SpriteRenderer _sr;
    private bool _highlighted;
    private float _baseScale;
    private IPlanetState _state;
    private IGravityType _cachedGravityType;
    private readonly System.Collections.Generic.List<SatelliteBody> _satellites = new System.Collections.Generic.List<SatelliteBody>();

    private bool _gravityActive = true;
    private float _spinAngle;        // 자전 누적 각도 (도)
    private float _dockAngleOffset;  // 도킹 접촉 각도 오프셋 (도, 스핀 독립)
    private bool _isDocked;

    // IGravitySource 구현
    public Vector2 Position => transform.position;
    public float GravityStrength => Planet?.gravityStrength ?? 0f;
    public float EncounterRadius => Planet?.encounterRadius ?? GameConstants.PlanetAnim.ColliderRadius;
    public float GravityRange => (Planet?.encounterRadius ?? GameConstants.PlanetAnim.ColliderRadius)
                                 * (Planet?.gravityFieldRangeRatio ?? 6f);
    public bool IsActive => gameObject.activeInHierarchy && _gravityActive;
    public IGravityType CachedGravityType => _cachedGravityType;

    public T GetState<T>() where T : Component => GetComponent<T>();

    void OnEnable()
    {
        if (PlanetRegistry.Instance != null) PlanetRegistry.Instance.Register(this);
        if (GravitySourceRegistry.Instance != null) GravitySourceRegistry.Instance.Register(this);
    }
    void OnDisable()
    {
        if (PlanetRegistry.Instance != null) PlanetRegistry.Instance.Unregister(this);
        if (GravitySourceRegistry.Instance != null) GravitySourceRegistry.Instance.Unregister(this);
    }

    public System.Collections.Generic.List<SatelliteBody> Satellites => _satellites;
    public void AttachSatellite(SatelliteBody sat)
    {
        _satellites.Add(sat);
        sat.transform.SetParent(transform);
    }

    public void Initialize(PlanetSO data, Sprite sprite)
    {
        Planet = data;
        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null) _sr = gameObject.AddComponent<SpriteRenderer>();
        _sr.color = Color.white;
        _sr.sortingOrder = GameConstants.SortingOrder.PlanetBody;
        transform.localScale = Vector3.one * data.visualScale;
        _baseScale = data.visualScale;

        // 애니메이션 클립이 있으면 PlanetAnimator가 frames 루프 재생, 없으면 정적 sprite 폴백.
        var clip = PlanetSpriteResolver.ResolveClip(data);
        if (clip != null && clip.frames != null && clip.frames.Length >= 2)
        {
            _sr.sprite = clip.Icon;  // 즉시 첫 프레임 (Animator가 다음 Update에 덮어씀)
            gameObject.AddComponent<PlanetAnimator>().Play(clip);
        }
        else if (sprite != null)
        {
            _sr.sprite = sprite;
        }

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = GameConstants.PlanetAnim.ColliderRadius;

        AttachStateIfNeeded(data.effectId);
        _cachedGravityType = GravityTypeRegistry.Get(data.gravityTypeId);
    }

    void AttachStateIfNeeded(string effectId)
    {
        var effectType = EffectRegistry.GetEffectType(effectId);
        if (effectType == null) return;
        var stateAttr = effectType.GetCustomAttribute<PlanetStateAttribute>();
        if (stateAttr?.StateType == null) return;
        _state = (IPlanetState)gameObject.AddComponent(stateAttr.StateType);
    }

    void Update()
    {
        _spinAngle += (Planet?.spinSpeed ?? 0f) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, 0f, _spinAngle);

        _state?.Tick(Time.deltaTime);
        float pulse = 1f + Mathf.Sin(Time.time * GameConstants.PlanetAnim.PulseFrequency) * GameConstants.PlanetAnim.PulseAmplitude;
        transform.localScale = Vector3.one * _baseScale * pulse;

        if (!_gravityActive)
            _sr.color = new Color(0.35f, 0.35f, 0.35f, 1f);
        else if (_highlighted)
            _sr.color = GameConstants.Colors.PlanetHighlight;
        else
            _sr.color = Color.white;
    }

    public void Highlight(bool on) { _highlighted = on; }

    public void DeactivateGravity() { _gravityActive = false; }
    public void ReactivateGravity() { _gravityActive = true; }

    /// <summary>투사체가 행성 표면에 착지. 접촉 월드 위치를 기준으로 도킹 각도 계산.</summary>
    public void Dock(Vector2 shipWorldPos)
    {
        _isDocked = true;
        Vector2 local = shipWorldPos - (Vector2)transform.position;
        float contactDeg = Mathf.Atan2(local.y, local.x) * Mathf.Rad2Deg;
        _dockAngleOffset = contactDeg - _spinAngle;
    }

    /// <summary>현재 자전 각도 기준 도킹된 투사체의 월드 위치.</summary>
    public Vector2 GetDockedShipPosition()
    {
        float rad = (_dockAngleOffset + _spinAngle) * Mathf.Deg2Rad;
        return (Vector2)transform.position
             + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * EncounterRadius;
    }

    /// <summary>도킹 위치에서 행성 중심 반대 방향 (재발사 방향).</summary>
    public Vector2 GetDockedLaunchDirection()
        => (GetDockedShipPosition() - (Vector2)transform.position).normalized;

    public void Undock() { _isDocked = false; }

    public bool IntersectsLine(Vector2 a, Vector2 b, float width)
        => CollisionGeometry.IntersectsLine(transform.position, _baseScale * 0.5f, a, b, width);

    public float ProjectionT(Vector2 a, Vector2 b)
        => CollisionGeometry.ProjectionT(transform.position, a, b);
}
