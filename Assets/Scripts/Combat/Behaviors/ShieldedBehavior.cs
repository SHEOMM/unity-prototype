using UnityEngine;

/// <summary>방패병: 방어막이 데미지를 흡수한다. 소진 후 일반 데미지를 받는다.</summary>
[EnemyBehaviorId("shielded")]
[EnemyState(typeof(ShieldState))]
public class ShieldedBehavior : IEnemyBehavior
{
    public bool Tick(Enemy enemy, float dt) => true;

    public float ModifyIncomingDamage(Enemy enemy, float dmg, Element el)
    {
        var shield = enemy.GetComponent<ShieldState>();
        if (shield != null && shield.shieldHP > 0)
        {
            float absorbed = Mathf.Min(dmg, shield.shieldHP);
            shield.shieldHP -= absorbed;
            return dmg - absorbed;
        }
        return dmg;
    }

    public bool OnDeath(Enemy enemy) => true;
}
