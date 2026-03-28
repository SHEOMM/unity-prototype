[EnemyBehaviorId("grunt")]
public class GruntBehavior : IEnemyBehavior
{
    public bool Tick(Enemy enemy, float dt) => true;
    public float ModifyIncomingDamage(Enemy enemy, float dmg, Element el) => dmg;
    public bool OnDeath(Enemy enemy) => true;
}
