using UnityEngine;

/// <summary>
/// 심플렉틱 오일러 적분. velocity-first로 에너지 보존 우수.
/// 프레임 독립 드래그, 속도 상한, NaN 보호 포함.
/// static — 상태 없이 재사용 가능.
/// </summary>
public static class ShipIntegrator
{
    public static void Integrate(
        ref Vector2 position, ref Vector2 velocity,
        Vector2 totalForce, float drag, float dt)
    {
        velocity += totalForce * dt;
        velocity *= Mathf.Exp(-drag * dt);
        velocity = Vector2.ClampMagnitude(velocity, GameConstants.ShipPhysics.MaxSpeed);

        if (float.IsNaN(velocity.x) || float.IsInfinity(velocity.x) ||
            float.IsNaN(velocity.y) || float.IsInfinity(velocity.y))
            velocity = Vector2.zero;

        position += velocity * dt;
    }
}
