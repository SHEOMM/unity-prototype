/// <summary>속성체: 기본 행동. 속성 내성/약점은 EnemySO.resistances에서 자동 처리.</summary>
[EnemyBehaviorId("elemental")]
public class ElementalBehavior : IEnemyBehavior
{
    public bool Tick(Enemy enemy, float dt) => true;
    public float ModifyIncomingDamage(Enemy enemy, float dmg, Element el) => dmg;
    public bool OnDeath(Enemy enemy) => true;
}
