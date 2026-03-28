using UnityEngine;

/// <summary>
/// 범용 데미지 효과. effectId 전용 구현이 없는 별들의 기본 동작.
/// </summary>
public class GenericDamageEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        int hits = 1 + ctx.trailing.Count;
        ctx.result.commands.Add(new SpellCommand
        {
            element = ctx.source.Planet.element,
            damage = dmg,
            hitCount = hits,
            sourceName = ctx.source.Planet.bodyName,
            visualType = SpellVisualType.Strike
        });
    }
}
