using UnityEngine;

/// <summary>척력. 소스 반대 방향으로 밀어낸다.</summary>
[GravityTypeId("repulsive")]
public class RepulsiveGravity : IGravityType
{
    public Vector2 CalculateForce(Vector2 shipPos, Vector2 dirToSource, float forceMagnitude)
        => -dirToSource * forceMagnitude;
}
