using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 모든 중력원의 힘을 합산한다. Zero-Alloc.
/// 거리 클램핑, 힘 상한, 풀오프 거리를 보장.
/// </summary>
public class GravityAccumulator
{
    private Vector2 _force;

    public Vector2 Calculate(Vector2 shipPos, IReadOnlyList<IGravitySource> sources, int count)
    {
        _force = Vector2.zero;
        for (int i = 0; i < count; i++)
        {
            var src = sources[i];
            if (!src.IsActive || src.GravityStrength <= 0f) continue;
            _force += CalculateSingleForce(shipPos, src);
        }
        return _force;
    }

    private Vector2 CalculateSingleForce(Vector2 shipPos, IGravitySource src)
    {
        Vector2 toSource = src.Position - shipPos;
        float distSq = toSource.sqrMagnitude;

        float minDistSq = GameConstants.ShipPhysics.MinGravityDistance
                         * GameConstants.ShipPhysics.MinGravityDistance;
        if (distSq < minDistSq) distSq = minDistSq;

        float dist = Mathf.Sqrt(distSq);
        if (dist > GameConstants.ShipPhysics.GravityFalloffStart) return Vector2.zero;

        float forceMag = Mathf.Min(
            src.GravityStrength / distSq,
            GameConstants.ShipPhysics.MaxGravityForce
        );

        return src.CachedGravityType.CalculateForce(shipPos, toSource / dist, forceMag);
    }
}
