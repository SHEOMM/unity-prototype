using UnityEngine;

/// <summary>
/// 바다별: 수파로 전체 적에게 감소 데미지.
/// 위상: 해일 (전체 풀뎀 + 둔화)
/// </summary>
[EffectId("ocean")]
public class OceanEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        int enemyCount = Mathf.Max(1, ctx.enemies.Count);

        if (ctx.isPhaseActive)
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Water,
                damage = dmg * 1.5f,
                hitCount = enemyCount,
                sourceName = ctx.source.Planet.bodyName + " (해일)",
                visualType = SpellVisualType.AreaOfEffect
            });
        }
        else
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Water,
                damage = dmg * 0.6f,
                hitCount = enemyCount + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.AreaOfEffect
            });
        }
    }
}
