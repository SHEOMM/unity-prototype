using UnityEngine;
using System.Reflection;

/// <summary>
/// 행성 런타임. 궤도 위에서 공전하며 슬래시 충돌 대상이 된다.
/// PlanetSO 데이터를 참조한다.
/// </summary>
public class PlanetBody : MonoBehaviour
{
    public PlanetSO Planet { get; private set; }
    private SpriteRenderer _sr;
    private bool _highlighted;
    private float _baseScale;
    private IPlanetState _state;
    private IPlanetHUD _hud;
    private readonly System.Collections.Generic.List<SatelliteBody> _satellites = new System.Collections.Generic.List<SatelliteBody>();

    /// <summary>이 행성에 부착된 지속 상태를 타입으로 조회한다.</summary>
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
        _sr.color = data.bodyColor;
        _sr.sortingOrder = 5;
        transform.localScale = Vector3.one * data.visualScale;
        _baseScale = data.visualScale;

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;

        AttachStateIfNeeded(data.effectId);
        AttachHUDIfNeeded(data.effectId);
        CreateLabel(data.bodyName);
    }

    void CreateLabel(string text)
    {
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(transform);
        labelGo.transform.localPosition = new Vector3(0, -1.2f, 0);
        // 부모 스케일을 상쇄하여 월드 기준 고정 크기로 표시
        float invScale = _baseScale > 0.01f ? 1f / _baseScale : 1f;
        labelGo.transform.localScale = Vector3.one * invScale;

        var tm = labelGo.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 64;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1f, 1f, 1f, 0.9f);
        tm.characterSize = 0.12f;

        var mr = labelGo.GetComponent<MeshRenderer>();
        mr.sortingOrder = 10;
    }

    void AttachStateIfNeeded(string effectId)
    {
        var effectType = EffectRegistry.GetEffectType(effectId);
        if (effectType == null) return;
        var stateAttr = effectType.GetCustomAttribute<PlanetStateAttribute>();
        if (stateAttr?.StateType == null) return;
        _state = (IPlanetState)gameObject.AddComponent(stateAttr.StateType);
    }

    void AttachHUDIfNeeded(string effectId)
    {
        var effectType = EffectRegistry.GetEffectType(effectId);
        if (effectType == null) return;
        var hudAttr = effectType.GetCustomAttribute<PlanetHUDAttribute>();
        if (hudAttr?.HUDType == null) return;
        _hud = (IPlanetHUD)gameObject.AddComponent(hudAttr.HUDType);
    }

    void Update()
    {
        _state?.Tick(Time.deltaTime);
        _hud?.UpdateHUD();
        float pulse = 1f + Mathf.Sin(Time.time * 3f) * 0.1f;
        transform.localScale = Vector3.one * _baseScale * pulse;
        _sr.color = _highlighted ? Color.yellow : Planet.bodyColor;
    }

    public void Highlight(bool on) { _highlighted = on; }

    public bool IntersectsLine(Vector2 a, Vector2 b, float width)
    {
        Vector2 pos = transform.position;
        float totalR = (_baseScale * 0.5f) + width * 0.5f;
        Vector2 ab = b - a;
        float t = Mathf.Clamp01(Vector2.Dot(pos - a, ab) / Vector2.Dot(ab, ab));
        return Vector2.Distance(pos, a + t * ab) <= totalR;
    }

    public float ProjectionT(Vector2 a, Vector2 b)
    {
        Vector2 pos = transform.position;
        Vector2 ab = b - a;
        return Mathf.Clamp01(Vector2.Dot(pos - a, ab) / Vector2.Dot(ab, ab));
    }
}
