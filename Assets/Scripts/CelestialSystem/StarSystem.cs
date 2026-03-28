using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 항성(Star) 런타임. 고정 위치에서 궤도들을 관리한다.
/// 행성을 궤도에 배치하고, 공전을 구동한다.
/// </summary>
public class StarSystem : MonoBehaviour
{
    public StarSO Data { get; private set; }
    private List<OrbitRing> _orbits = new List<OrbitRing>();

    public void Initialize(StarSO data, Sprite starSprite)
    {
        Data = data;
        transform.position = new Vector3(data.celestialPosition.x, data.celestialPosition.y, 0);
        transform.localScale = Vector3.one * data.visualScale;
        gameObject.name = data.bodyName;

        var sr = gameObject.AddComponent<SpriteRenderer>();
        if (starSprite != null) sr.sprite = starSprite;
        sr.color = Color.white;
        sr.sortingOrder = GameConstants.SortingOrder.StarBody;

        if (data.orbits == null) return;
        for (int i = 0; i < data.orbits.Length; i++)
        {
            var orbitGo = new GameObject("Orbit_" + i);
            orbitGo.transform.SetParent(transform);
            orbitGo.transform.localPosition = Vector3.zero;
            var ring = orbitGo.AddComponent<OrbitRing>();
            ring.Initialize(data.orbits[i]);
            _orbits.Add(ring);
        }
    }

    public bool PlacePlanet(PlanetBody planet, int orbitIndex)
    {
        if (orbitIndex < 0 || orbitIndex >= _orbits.Count) return false;
        if (_orbits[orbitIndex].occupant != null) return false;
        _orbits[orbitIndex].AssignPlanet(planet);
        return true;
    }

    public List<OrbitRing> Orbits => _orbits;

}
