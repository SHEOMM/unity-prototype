using UnityEngine;

/// <summary>
/// 폭풍별: 연쇄번개. 1적 타격 후 trailing 수만큼 체인.
/// 위상: 폭풍 지속 AoE
/// </summary>
[EffectId("storm")]
public class StormEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        int chainCount = 1 + ctx.trailing.Count;

        if (ctx.isPhaseActive)
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Wind,
                damage = dmg * 1.2f,
                hitCount = chainCount * 2,
                sourceName = ctx.source.Planet.bodyName + " (폭풍)",
                visualType = SpellVisualType.AreaOfEffect
            });
        }
        else
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Wind,
                damage = dmg,
                hitCount = chainCount + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Strike
            });
        }
    }
}
