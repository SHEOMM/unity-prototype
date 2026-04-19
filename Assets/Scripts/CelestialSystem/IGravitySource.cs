using UnityEngine;

/// <summary>
/// 중력을 방출하는 천체 인터페이스. Phase 9 기준 PlanetBody가 구현.
/// CachedGravityType으로 서브스텝마다 Registry lookup을 피한다.
/// </summary>
public interface IGravitySource
{
    Vector2 Position { get; }
    float GravityStrength { get; }
    float EncounterRadius { get; }
    float GravityRange { get; }
    bool IsActive { get; }
    IGravityType CachedGravityType { get; }
}
