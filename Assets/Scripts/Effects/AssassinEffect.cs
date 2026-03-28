using UnityEngine;

/// <summary>
/// 자객별: 단일 대상 고데미지. 슬래시 내 별 1-2개면 보너스.
/// 위상: 처형 (HP% 즉사 판정 - 높은 고정 데미지로 표현)
/// </summary>
[EffectId("assassin")]
public class AssassinEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        int totalHits = ctx.allHits.Count;

        if (ctx.isPhaseActive)
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Darkness,
                damage = dmg * 4f,
                hitCount = 1,
                sourceName = ctx.source.Planet.bodyName + " (처형)",
                visualType = SpellVisualType.Strike
            });
        }
        else
        {
            float bonus = totalHits <= 2 ? 2.5f : 1f;
            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = dmg * bonus,
                hitCount = 1 + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Strike
            });
        }
    }
}
