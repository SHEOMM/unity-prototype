using UnityEngine;

/// <summary>
/// 감시탑 behavior: 주기적으로 가장 가까운 적 1마리에게 데미지.
/// Civilization tier1(감시탑)에서 사용. 간단한 터렛 구현.
/// </summary>
[StructureBehaviorId("watchtower")]
public class WatchtowerBehavior : IStructureBehavior
{
    public float attackInterval = 1.5f;
    public float attackRange = 4f;
    public float attackDamage = 6f;
    public Element attackElement = Element.None;

    private float _cooldown;

    public void OnTick(Structure structure, float deltaTime)
    {
        if (structure == null || EnemyRegistry.Instance == null) return;
        _cooldown -= deltaTime;
        if (_cooldown > 0f) return;

        var center = structure.transform.position;
        Enemy target = null;
        float bestDistSq = float.MaxValue;
        foreach (var e in EnemyRegistry.Instance.GetAll())
        {
            if (e == null || !e.IsAlive) continue;
            float d = ((Vector2)(e.transform.position - center)).sqrMagnitude;
            if (d > attackRange * attackRange) continue;
            if (d < bestDistSq) { bestDistSq = d; target = e; }
        }
        if (target == null) return;

        target.TakeDamage(attackDamage, attackElement);
        _cooldown = attackInterval;
    }

    public void OnDestroyed(Structure structure) { }
}
