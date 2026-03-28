/// <summary>벌떼: 기본 행동. SO에서 낮은 HP, 빠른 속도, 작은 크기로 차별화.</summary>
[EnemyBehaviorId("swarm")]
public class SwarmBehavior : IEnemyBehavior
{
    public bool Tick(Enemy enemy, float dt) => true;
    public float ModifyIncomingDamage(Enemy enemy, float dmg, Element el) => dmg;
    public bool OnDeath(Enemy enemy) => true;
}
