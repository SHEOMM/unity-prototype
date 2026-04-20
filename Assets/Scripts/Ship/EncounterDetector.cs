using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 우주선 이동 경로와 행성 간 연속 충돌 감지. 터널링 방지.
/// CollisionGeometry 재사용. 중복 트리거 방지 내장.
/// </summary>
public class EncounterDetector
{
    private readonly HashSet<PlanetBody> _encounteredSet = new HashSet<PlanetBody>();
    private readonly List<PlanetBody> _encounterOrder = new List<PlanetBody>();

    public IReadOnlyList<PlanetBody> EncounterOrder => _encounterOrder;

    public void Reset()
    {
        _encounteredSet.Clear();
        _encounterOrder.Clear();
    }

    public void DetectEncounters(
        Vector2 oldPos, Vector2 newPos,
        IReadOnlyList<IGravitySource> sources, int count,
        System.Action<PlanetBody> onEncounter)
    {
        for (int i = 0; i < count; i++)
        {
            var src = sources[i];
            if (!src.IsActive) continue;
            if (!(src is PlanetBody planet)) continue;
            if (_encounteredSet.Contains(planet)) continue;

            if (CollisionGeometry.IntersectsLine(
                    src.Position, src.EncounterRadius,
                    oldPos, newPos,
                    GameConstants.ShipPhysics.ShipCollisionRadius))
            {
                _encounteredSet.Add(planet);
                _encounterOrder.Add(planet);
                onEncounter?.Invoke(planet);
            }
        }
    }
}
