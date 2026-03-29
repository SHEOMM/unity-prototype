using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 원형 범위 내 행성/혜성 탐지.
/// 중심점과 반지름으로 범위 내 천체를 수집한다. 순서는 중심에서 가까운 순.
/// </summary>
public class ScopeDetector : MonoBehaviour
{
    [Tooltip("스코프 반지름")]
    public float scopeRadius = 2f;

    public List<PlanetBody> DetectPlanets(Vector2 center)
    {
        var planets = PlanetRegistry.Instance.GetAll();
        var hits = new List<(PlanetBody p, float dist)>();
        for (int i = 0; i < planets.Count; i++)
        {
            float dist = Vector2.Distance(center, (Vector2)planets[i].transform.position);
            if (dist <= scopeRadius)
                hits.Add((planets[i], dist));
        }
        hits.Sort((a, b) => a.dist.CompareTo(b.dist));

        var sorted = new List<PlanetBody>();
        for (int i = 0; i < hits.Count; i++)
            sorted.Add(hits[i].p);
        return sorted;
    }

    public void HighlightInScope(Vector2 center)
    {
        var planets = PlanetRegistry.Instance.GetAll();
        for (int i = 0; i < planets.Count; i++)
        {
            float dist = Vector2.Distance(center, (Vector2)planets[i].transform.position);
            planets[i].Highlight(dist <= scopeRadius);
        }
    }

    public void ClearHighlights()
    {
        var planets = PlanetRegistry.Instance.GetAll();
        for (int i = 0; i < planets.Count; i++)
            planets[i].Highlight(false);
    }

    public List<CometBody> DetectComets(Vector2 center)
    {
        var hits = new List<CometBody>();
        foreach (var c in CometBody.ActiveComets)
        {
            if (Vector2.Distance(center, (Vector2)c.transform.position) <= scopeRadius)
                hits.Add(c);
        }
        return hits;
    }
}
