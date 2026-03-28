using UnityEngine;

/// <summary>
/// 슬래시 충돌 대상 인터페이스. PlanetBody와 CometBody 모두 구현한다.
/// </summary>
public interface ISlashTarget
{
    bool IntersectsLine(Vector2 a, Vector2 b, float width);
    float ProjectionT(Vector2 a, Vector2 b);
    Transform transform { get; }
}
