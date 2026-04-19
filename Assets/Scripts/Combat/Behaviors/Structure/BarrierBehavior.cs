using UnityEngine;

/// <summary>
/// 방벽 구조물 behavior: 일정 반경 내 적을 지속 넉백하여 진입 차단.
/// Earth tier3(벽) / Saturn 등이 사용. 넉백 방향은 구조물 중심에서 적으로.
/// </summary>
[StructureBehaviorId("barrier")]
public class BarrierBehavior : IStructureBehavior
{
    public float pushRadius = 1.5f;
    public float pushStrength = 2f;

    public void OnTick(Structure structure, float deltaTime)
    {
        if (structure == null || EnemyRegistry.Instance == null) return;
        var center = structure.transform.position;
        var enemies = EnemyRegistry.Instance.GetNearby(center, pushRadius);
        foreach (var e in enemies)
        {
            if (e == null) continue;
            Vector2 dir = (Vector2)(e.transform.position - center);
            if (dir.sqrMagnitude < 1e-6f) dir = Vector2.left;
            KnockbackApplicator.Apply(e, dir, pushStrength * deltaTime);
        }
    }

    public void OnDestroyed(Structure structure) { /* 파괴 시 이펙트 없음 */ }
}
