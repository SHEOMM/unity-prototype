using UnityEngine;

/// <summary>
/// 발사체 궤적 전방 시뮬레이션. 순수 함수로 부작용 없이 예측 경로를 계산.
/// ShipIntegrator + GravityAccumulator를 재사용하여 실제 비행과 동일한 공식을 사용.
/// 핫패스 — GravityAccumulator 인스턴스를 static으로 재사용하여 zero-alloc.
/// </summary>
public static class TrajectorySimulator
{
    // GravityAccumulator는 Calculate 호출마다 내부 _force를 리셋하므로 재사용 안전.
    private static readonly GravityAccumulator _gravity = new GravityAccumulator();

    // 화면 밖 체크 — ShipModel.IsOutOfBounds와 동일
    private const float BoundsX = 9f;
    private const float BoundsY = 7f;

    /// <summary>
    /// stepCount만큼 순방향 시뮬레이션. 매 스텝 위치를 outBuffer에 채운다.
    /// 에너지 소진 / 화면 밖 도달 시 조기 종료.
    /// </summary>
    /// <returns>기록된 샘플 개수 (≤ stepCount)</returns>
    public static int Simulate(
        Vector2 origin, Vector2 initialVelocity, float energy,
        float drag, float energyDrain,
        Vector2[] outBuffer, int stepCount, float dt)
    {
        if (outBuffer == null || outBuffer.Length == 0) return 0;

        var reg = GravitySourceRegistry.Instance;
        var sources = reg?.Sources;
        int srcCount = reg?.Count ?? 0;

        Vector2 pos = origin;
        Vector2 vel = initialVelocity;
        float e = energy;
        float margin = GameConstants.ShipPhysics.BoundsMargin;
        int limit = Mathf.Min(stepCount, outBuffer.Length);

        int count = 0;
        for (int i = 0; i < limit; i++)
        {
            if (e <= 0f) break;

            Vector2 force = sources != null
                ? _gravity.Calculate(pos, sources, srcCount)
                : Vector2.zero;
            ShipIntegrator.Integrate(ref pos, ref vel, force, drag, dt);
            e -= energyDrain * dt;

            outBuffer[count++] = pos;

            if (pos.x < -BoundsX - margin || pos.x > BoundsX + margin
                || pos.y < -BoundsY - margin || pos.y > BoundsY + margin)
                break;
        }
        return count;
    }
}
