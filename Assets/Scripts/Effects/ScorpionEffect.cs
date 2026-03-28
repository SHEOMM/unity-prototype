using UnityEngine;

/// <summary>
/// 전갈별: 독 DoT 부여. trailing 수만큼 중첩 (다수 타격으로 표현).
/// 위상: 맹독 (최대HP% 데미지 - 고데미지로 표현)
/// </summary>
[EffectId("scorpion")]
public class ScorpionEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        int stacks = 1 + ctx.trailing.Count;

        if (ctx.isPhaseActive)
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Earth,
                damage = dmg * 3f,
                hitCount = stacks,
                sourceName = ctx.source.Planet.bodyName + " (맹독)",
                visualType = SpellVisualType.AreaOfEffect
            });
        }
        else
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = dmg * 0.5f,
                hitCount = stacks * 2 + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Projectile
            });
        }
    }
}
