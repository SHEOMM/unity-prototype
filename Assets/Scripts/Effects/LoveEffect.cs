using UnityEngine;

/// <summary>
/// 사랑별: 이번 슬래시 내 전체 데미지 배율 증가 (서포트 역할).
/// 위상: 적 하나 매혹 (큰 데미지로 표현)
/// </summary>
[EffectId("love")]
public class LoveEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;

        if (ctx.isPhaseActive)
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Water,
                damage = dmg * 5f,
                hitCount = 1,
                sourceName = ctx.source.Planet.bodyName + " (매혹)",
                visualType = SpellVisualType.Strike
            });
        }
        else
        {
            // 기존 모든 명령의 데미지를 30% 증가
            foreach (var cmd in ctx.result.commands)
                cmd.damage *= 1.3f;

            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = dmg,
                hitCount = 1 + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Strike
            });
        }
    }
}
