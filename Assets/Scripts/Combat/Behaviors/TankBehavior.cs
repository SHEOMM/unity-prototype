/// <summary>중갑병: 받는 데미지 40% 감소.</summary>
[EnemyBehaviorId("tank")]
public class TankBehavior : IEnemyBehavior
{
    public bool Tick(Enemy enemy, float dt) => true;
    public float ModifyIncomingDamage(Enemy enemy, float dmg, Element el) => dmg * 0.6f;
    public bool OnDeath(Enemy enemy) => true;
}
