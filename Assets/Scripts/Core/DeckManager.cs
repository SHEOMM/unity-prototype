using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    private List<StarSystem> _stars = new List<StarSystem>();
    private Sprite _starSprite;
    private Sprite _planetSprite;

    void Awake()
    {
        Instance = this;
        _starSprite = Resources.Load<Sprite>("StarSprite");
        _planetSprite = _starSprite;
    }

    public StarSystem CreateStar(StarSO data)
    {
        var sys = new GameObject(data.bodyName).AddComponent<StarSystem>();
        sys.Initialize(data, _starSprite);
        _stars.Add(sys);
        return sys;
    }

    public PlanetBody CreatePlanet(PlanetSO data)
    {
        var body = new GameObject(data.bodyName).AddComponent<PlanetBody>();
        body.Initialize(data, _planetSprite);
        return body;
    }

    public bool PlacePlanetOnStar(PlanetBody planet, StarSystem star, int idx)
    {
        return star.PlacePlanet(planet, idx);
    }

    public SatelliteBody CreateSatellite(SatelliteSO data)
    {
        var body = new GameObject(data.bodyName).AddComponent<SatelliteBody>();
        body.Initialize(data, _starSprite);
        return body;
    }

    public void AttachSatelliteToPlanet(SatelliteBody sat, PlanetBody planet)
    {
        planet.AttachSatellite(sat);
    }

    public List<StarSystem> Stars => _stars;
}
