using UnityEngine;

/// <summary>
/// 달: 조류 파동. 물 데미지를 (1 + 선행 수)회 반복 타격.
/// 위상: 만조 — 전체 적에게 다중 물 타격.
/// </summary>
[EffectId("moon")]
public class MoonEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        int waveCount = 1 + ctx.leading.Count;

        if (ctx.isPhaseActive)
        {
            int enemyCount = Mathf.Max(1, ctx.enemies.Count);
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Water,
                damage = dmg * 1.5f,
                hitCount = enemyCount * waveCount,
                sourceName = ctx.source.Planet.bodyName + " (만조)",
                visualType = SpellVisualType.AreaOfEffect
            });
        }
        else
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Water,
                damage = dmg,
                hitCount = waveCount + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Strike
            });
        }
    }
}
