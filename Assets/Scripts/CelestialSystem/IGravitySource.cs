using UnityEngine;

/// <summary>
/// 중력을 방출하는 천체 인터페이스. PlanetBody와 StarSystem이 구현한다.
/// CachedGravityType으로 서브스텝마다 Registry lookup을 피한다.
/// </summary>
public interface IGravitySource
{
    Vector2 Position { get; }
    float GravityStrength { get; }
    float EncounterRadius { get; }
    bool IsActive { get; }
    IGravityType CachedGravityType { get; }
}
