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
        sr.color = data.bodyColor;
        sr.sortingOrder = 2;

        CreateLabel(data.bodyName, data.visualScale);

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

    void CreateLabel(string text, float parentScale)
    {
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(transform);
        labelGo.transform.localPosition = new Vector3(0, -1.5f, 0);
        float invScale = parentScale > 0.01f ? 1f / parentScale : 1f;
        labelGo.transform.localScale = Vector3.one * invScale;

        var tm = labelGo.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 64;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1f, 1f, 0.8f, 0.8f);
        tm.characterSize = 0.15f;

        var mr = labelGo.GetComponent<MeshRenderer>();
        mr.sortingOrder = 10;
    }
}
