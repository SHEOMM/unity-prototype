using UnityEngine;

/// <summary>
/// 제왕별: 맨 앞에 있으면 낙뢰를 떨군다.
/// 뒤에 있는 별 개수만큼 낙뢰가 추가된다.
/// </summary>
[EffectId("emperor")]
public class EmperorEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        bool isFront = ctx.positionIndex == 0;
        int trailCount = ctx.trailing.Count;

        if (isFront)
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = dmg * 1.5f,
                hitCount = 1 + trailCount + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Strike,
                visualId = "emperor"
            });
        }
        else
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = dmg,
                hitCount = 1 + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Strike,
                visualId = "emperor"
            });
        }
    }
}
