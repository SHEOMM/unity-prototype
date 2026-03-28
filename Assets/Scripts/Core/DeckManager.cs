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
        _stars.Add(sys);
        return sys;
    }

    public PlanetBody CreatePlanet(PlanetSO data)
    {
        var sprite = CelestialSpriteGenerator.GeneratePlanetSprite(data.element, data.bodyColor);
        var go = new GameObject(data.bodyName);
        var body = go.AddComponent<PlanetBody>();
        body.Initialize(data, sprite);
        go.AddComponent<PlanetLabelView>();
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

    void AttachHUDIfNeeded(GameObject go, string effectId)
    {
        var effectType = EffectRegistry.GetEffectType(effectId);
        if (effectType == null) return;
        var hudAttr = effectType.GetCustomAttribute<PlanetHUDAttribute>();
        if (hudAttr?.HUDType == null) return;
        go.AddComponent(hudAttr.HUDType);
    }
}
