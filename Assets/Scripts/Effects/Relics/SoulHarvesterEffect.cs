/// <summary>영혼 수확자: 적 처치 시 HP 1 회복.</summary>
[RelicEffectId("soul_harvester")]
public class SoulHarvesterEffect : RelicEffectBase
{
    public override void OnEnemyKilled(Enemy enemy, PlayerState player)
    {
        player.Heal(1f);
    }
}
