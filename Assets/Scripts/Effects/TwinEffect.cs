using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 쌍둥이별: 앞/뒤 중 개수가 적은 쪽 별들의 효과를 1번 더 발동.
/// 개수가 같으면 양쪽 다.
/// </summary>
[EffectId("twin")]
public class TwinEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        ctx.result.commands.Add(new SpellCommand
        {
            element = ctx.source.Planet.element,
            damage = dmg,
            hitCount = 1,
            sourceName = ctx.source.Planet.bodyName,
            visualType = SpellVisualType.Strike
        });

        List<PlanetBody> toRepeat;
        if (ctx.leading.Count < ctx.trailing.Count)
            toRepeat = ctx.leading;
        else if (ctx.trailing.Count < ctx.leading.Count)
            toRepeat = ctx.trailing;
        else
        {
            toRepeat = new List<PlanetBody>();
            toRepeat.AddRange(ctx.leading);
            toRepeat.AddRange(ctx.trailing);
        }

        foreach (var planet in toRepeat)
        {
            var effect = EffectRegistry.Get(planet.Planet.effectId);
            var subCtx = new EffectContext
            {
                source = planet,
                positionIndex = ctx.allHits.IndexOf(planet),
                allHits = ctx.allHits,
                leading = ctx.leading,
                trailing = ctx.trailing,
                enemies = ctx.enemies,
                damageMultiplier = ctx.damageMultiplier * 0.5f,
                isPhaseActive = false,
                result = ctx.result
            };
            effect.Execute(subCtx);
        }
    }
}
