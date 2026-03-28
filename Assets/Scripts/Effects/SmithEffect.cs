using UnityEngine;

/// <summary>
/// 대장장이별: 같은 슬래시 내 다른 SpellCommand에 고정 데미지 추가.
/// 위상: 원소무기 단조 (전체 명령 데미지 대폭 강화)
/// </summary>
[EffectId("smith")]
public class SmithEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;

        if (ctx.isPhaseActive)
        {
            float forgeBonus = dmg * 2f;
            foreach (var cmd in ctx.result.commands)
                cmd.damage += forgeBonus;

            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Earth,
                damage = dmg,
                hitCount = 1,
                sourceName = ctx.source.Planet.bodyName + " (단조)",
                visualType = SpellVisualType.Strike
            });
        }
        else
        {
            float flatBonus = dmg;
            foreach (var cmd in ctx.result.commands)
                cmd.damage += flatBonus;

            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = dmg * 0.5f,
                hitCount = 1 + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Strike
            });
        }
    }
}
