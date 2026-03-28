using UnityEngine;
using System.Reflection;

/// <summary>
/// 행성 런타임. 궤도 위에서 공전하며 슬래시 충돌 대상이 된다.
/// 순수 모델 — UI(라벨, HUD)는 별도 View 컴포넌트가 담당.
/// </summary>
public class PlanetBody : MonoBehaviour
{
    public PlanetSO Planet { get; private set; }
    private SpriteRenderer _sr;
    private bool _highlighted;
    private float _baseScale;
    private IPlanetState _state;
    private readonly System.Collections.Generic.List<SatelliteBody> _satellites = new System.Collections.Generic.List<SatelliteBody>();

    public T GetState<T>() where T : Component => GetComponent<T>();

    void OnEnable() { if (PlanetRegistry.Instance != null) PlanetRegistry.Instance.Register(this); }
    void OnDisable() { if (PlanetRegistry.Instance != null) PlanetRegistry.Instance.Unregister(this); }

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
        if (sprite != null) _sr.sprite = sprite;
        _sr.color = Color.white;
        _sr.sortingOrder = GameConstants.SortingOrder.PlanetBody;
        transform.localScale = Vector3.one * data.visualScale;
        _baseScale = data.visualScale;

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = GameConstants.PlanetAnim.ColliderRadius;

        AttachStateIfNeeded(data.effectId);
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
        _state?.Tick(Time.deltaTime);
        float pulse = 1f + Mathf.Sin(Time.time * GameConstants.PlanetAnim.PulseFrequency) * GameConstants.PlanetAnim.PulseAmplitude;
        transform.localScale = Vector3.one * _baseScale * pulse;
        _sr.color = _highlighted ? GameConstants.Colors.PlanetHighlight : Color.white;
    }

    public void Highlight(bool on) { _highlighted = on; }

    public bool IntersectsLine(Vector2 a, Vector2 b, float width)
        => SlashGeometry.IntersectsLine(transform.position, _baseScale * 0.5f, a, b, width);

    public float ProjectionT(Vector2 a, Vector2 b)
        => SlashGeometry.ProjectionT(transform.position, a, b);
}
