using UnityEngine;
using System.Collections.Generic;

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
        var sys = new GameObject(data.bodyName).AddComponent<StarSystem>();
        sys.Initialize(data, sprite);
        _stars.Add(sys);
        return sys;
    }

    public PlanetBody CreatePlanet(PlanetSO data)
    {
        var sprite = CelestialSpriteGenerator.GeneratePlanetSprite(data.element, data.bodyColor);
        var body = new GameObject(data.bodyName).AddComponent<PlanetBody>();
        body.Initialize(data, sprite);
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
}
