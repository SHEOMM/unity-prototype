using UnityEngine;

/// <summary>
/// 넉백 적용. Enemy.currentKnockback에 누적하며, Enemy.Update에서 지수적으로 감쇠하면서 위치 이동.
/// 동일 프레임 다중 호출 시 벡터 합산.
/// </summary>
public static class KnockbackApplicator
{
    public static void Apply(Enemy target, Vector2 direction, float strength)
    {
        if (target == null || direction.sqrMagnitude < 1e-6f) return;
        target.currentKnockback += direction.normalized * strength;
    }
}
