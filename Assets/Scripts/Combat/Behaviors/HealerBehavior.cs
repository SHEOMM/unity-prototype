using UnityEngine;

/// <summary>치유사: 3초마다 반경 3 내 아군을 회복시킨다.</summary>
[EnemyBehaviorId("healer")]
[EnemyState(typeof(HealerState))]
public class HealerBehavior : IEnemyBehavior
{
    public bool Tick(Enemy enemy, float dt)
    {
        var state = enemy.GetComponent<HealerState>();
        if (state != null && state.CanHeal())
        {
            state.ResetCooldown();
            var nearby = EnemyRegistry.Instance.GetNearby(enemy.transform.position, 3f);
            foreach (var ally in nearby)
                if (ally != enemy) ally.Heal(state.healAmount);
        }
        return true;
    }

    public float ModifyIncomingDamage(Enemy enemy, float dmg, Element el) => dmg;
    public bool OnDeath(Enemy enemy) => true;
}
