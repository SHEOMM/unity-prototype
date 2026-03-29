using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    private List<PlanetBody> _celestials = new List<PlanetBody>();

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 에피사이클 궤도를 가진 천체를 생성한다.
    /// PlanetSO의 orbitTerms로 궤도를 정의하고, 독립적으로 움직인다.
    /// </summary>
    public PlanetBody CreateCelestialBody(PlanetSO data)
    {
        var sprite = CelestialSpriteGenerator.GeneratePlanetSprite(data.element, data.bodyColor);
        var go = new GameObject(data.bodyName);
        var body = go.AddComponent<PlanetBody>();
        body.Initialize(data, sprite);

        // 에피사이클 궤도 부착
        if (data.orbitTerms != null && data.orbitTerms.Length > 0)
        {
            var orbit = go.AddComponent<EpicyclicOrbit>();
            orbit.Initialize(data.orbitTerms, data.orbitCenter);
        }

        go.AddComponent<PlanetLabelView>();
        AttachHUDIfNeeded(go, data.effectId);
        _celestials.Add(body);
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

    public List<PlanetBody> Celestials => _celestials;

    void AttachHUDIfNeeded(GameObject go, string effectId)
    {
        var effectType = EffectRegistry.GetEffectType(effectId);
        if (effectType == null) return;
        var hudAttr = effectType.GetCustomAttribute<PlanetHUDAttribute>();
        if (hudAttr?.HUDType == null) return;
        go.AddComponent(hudAttr.HUDType);
    }
}
