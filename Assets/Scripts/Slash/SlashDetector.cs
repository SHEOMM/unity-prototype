using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 선분과 행성의 충돌 판정 + 순서 정렬.
/// SlashInput에서 받은 드래그 데이터로 관통된 행성을 수집한다.
/// </summary>
public class SlashDetector : MonoBehaviour
{
    public float slashWidth = 0.8f;

    public List<PlanetBody> DetectHits(Vector2 start, Vector2 end)
    {
        var planets = PlanetRegistry.Instance.GetAll();
        var hits = new List<(PlanetBody p, float t)>();
        for (int i = 0; i < planets.Count; i++)
        {
            if (planets[i].IntersectsLine(start, end, slashWidth))
                hits.Add((planets[i], planets[i].ProjectionT(start, end)));
        }
        hits.Sort((a, b) => a.t.CompareTo(b.t));

        var sorted = new List<PlanetBody>();
        for (int i = 0; i < hits.Count; i++)
            sorted.Add(hits[i].p);
        return sorted;
    }

    public void HighlightHits(Vector2 start, Vector2 end)
    {
        var planets = PlanetRegistry.Instance.GetAll();
        for (int i = 0; i < planets.Count; i++)
            planets[i].Highlight(planets[i].IntersectsLine(start, end, slashWidth));
    }

    public void ClearHighlights()
    {
        var planets = PlanetRegistry.Instance.GetAll();
        for (int i = 0; i < planets.Count; i++)
            planets[i].Highlight(false);
    }

    public List<CometBody> DetectComets(Vector2 start, Vector2 end)
    {
        var comets = FindObjectsByType<CometBody>(FindObjectsSortMode.None);
        var hits = new List<CometBody>();
        foreach (var c in comets)
        {
            if (c.IntersectsLine(start, end, slashWidth))
                hits.Add(c);
        }
        return hits;
    }
}
