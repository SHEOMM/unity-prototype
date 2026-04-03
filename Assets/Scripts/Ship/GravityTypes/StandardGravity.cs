using UnityEngine;

/// <summary>표준 인력. 소스 방향으로 끌어당긴다.</summary>
[GravityTypeId("standard")]
public class StandardGravity : IGravityType
{
    public Vector2 CalculateForce(Vector2 shipPos, Vector2 dirToSource, float forceMagnitude)
        => dirToSource * forceMagnitude;
}
