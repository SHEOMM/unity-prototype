using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    private readonly List<OrbitBody> _orbits = new List<OrbitBody>();

    void Awake()
    {
        Instance = this;
    }

    public OrbitBody CreateOrbit(OrbitSO data, Vector2 center)
    {
        var go = new GameObject("Orbit_" + data.orbitName);
        var body = go.AddComponent<OrbitBody>();
        body.Initialize(data, center);
        _orbits.Add(body);
        return body;
    }

    public PlanetBody CreatePlanet(PlanetSO data)
    {
        var sprite = PlanetSpriteResolver.Resolve(data);
        var go = new GameObject(data.bodyName);
        var body = go.AddComponent<PlanetBody>();
        body.Initialize(data, sprite);
        go.AddComponent<PlanetLabelView>();
        go.AddComponent<GravityRangeView>();
        AttachHUDIfNeeded(go, data.effectId);
        return body;
    }

    public SatelliteBody CreateSatellite(SatelliteSO data)
    {
        var sprite = CelestialSpriteGenerator.GenerateSatelliteSprite(data.bodyColor);
        var body = new GameObject(data.bodyName).AddComponent<SatelliteBody>();
        body.Initialize(data, sprite);
        return body;
    }

    public void AttachSatelliteToPlanet(SatelliteBody sat, PlanetBody planet)
    {
        planet.AttachSatellite(sat);
    }

    public IReadOnlyList<OrbitBody> Orbits => _orbits;

    /// <summary>현재 씬의 모든 궤도·행성을 파괴하고 리스트를 초기화.</summary>
    public void ClearAll()
    {
        foreach (var orbit in _orbits)
            if (orbit != null) Destroy(orbit.gameObject);
        _orbits.Clear();

        // PlanetRegistry에 남은 행성들도 정리 (고아 행성 포함)
        var planets = PlanetRegistry.Instance?.GetAll();
        if (planets != null)
        {
            foreach (var p in planets)
                if (p != null) Destroy(p.gameObject);
        }
    }

    void AttachHUDIfNeeded(GameObject go, string effectId)
    {
        var effectType = EffectRegistry.GetEffectType(effectId);
        if (effectType == null) return;
        var hudAttr = effectType.GetCustomAttribute<PlanetHUDAttribute>();
        if (hudAttr?.HUDType == null) return;
        go.AddComponent(hudAttr.HUDType);
    }
}
