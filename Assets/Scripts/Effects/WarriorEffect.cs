using UnityEngine;

/// <summary>
/// 전사별: 근접 강타. 맨 앞이면 2배 데미지.
/// 위상: 회전참격 AoE (전체 적 타격)
/// </summary>
[EffectId("warrior")]
public class WarriorEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        bool isFront = ctx.positionIndex == 0;

        if (ctx.isPhaseActive)
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = dmg * 1.8f,
                hitCount = Mathf.Max(1, ctx.enemies.Count),
                sourceName = ctx.source.Planet.bodyName + " (회전참격)",
                visualType = SpellVisualType.AreaOfEffect
            });
        }
        else
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = isFront ? dmg * 2f : dmg,
                hitCount = 1 + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Strike
            });
        }
    }
}
