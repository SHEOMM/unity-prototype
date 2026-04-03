using UnityEngine;

/// <summary>
/// 중력 방향 전략 인터페이스. 힘의 크기는 GravityAccumulator가 계산하고,
/// 이 인터페이스는 방향만 결정한다.
/// </summary>
public interface IGravityType
{
    /// <param name="shipPos">우주선 현재 위치</param>
    /// <param name="dirToSource">소스 방향 단위 벡터 (정규화됨)</param>
    /// <param name="forceMagnitude">거리/클램핑이 적용된 힘 크기</param>
    Vector2 CalculateForce(Vector2 shipPos, Vector2 dirToSource, float forceMagnitude);
}
