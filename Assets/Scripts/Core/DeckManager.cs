using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    private List<StarSystem> _stars = new List<StarSystem>();

    void Awake()
    {
        Instance = this;
    }

    public StarSystem CreateStar(StarSO data)
    {
        var sprite = CelestialSpriteGenerator.GenerateStarSprite(data.bodyColor);
        var go = new GameObject(data.bodyName);
        var sys = go.AddComponent<StarSystem>();
        sys.Initialize(data, sprite);
        go.AddComponent<StarLabelView>();
        go.AddComponent<GravityRangeView>();
        _stars.Add(sys);
        return sys;
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

    public bool PlacePlanetOnStar(PlanetBody planet, StarSystem star, int idx)
    {
        return star.PlacePlanet(planet, idx);
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

    public List<StarSystem> Stars => _stars;

    /// <summary>현재 씬의 모든 천체 GameObject를 파괴하고 리스트를 초기화한다.</summary>
    public void ClearAll()
    {
        foreach (var star in _stars)
            if (star != null) Destroy(star.gameObject);
        _stars.Clear();

        // PlanetRegistry에 남은 행성들도 정리
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
