using UnityEngine;

/// <summary>접선력. 소스 방향의 수직으로 당겨 나선 궤적을 유도한다.</summary>
[GravityTypeId("orbital")]
public class OrbitalGravity : IGravityType
{
    public Color RangeColor => new Color(0.3f, 1f, 0.5f, 0.08f);

    public Vector2 CalculateForce(Vector2 shipPos, Vector2 dirToSource, float forceMagnitude)
    {
        Vector2 perpendicular = new Vector2(-dirToSource.y, dirToSource.x);
        return perpendicular * forceMagnitude;
    }
}
