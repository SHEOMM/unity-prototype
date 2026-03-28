using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 천칭별: 앞/뒤 중 개수가 적은 쪽 별들의 효과를 1번 더 발동.
/// 개수가 같으면 양쪽 다 3번씩 발동.
/// </summary>
[EffectId("libra")]
public class LibraEffect : IStarEffect
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

        bool isEqual = ctx.leading.Count == ctx.trailing.Count;
        int repeatCount = isEqual ? 3 : 1;

        List<PlanetBody> toRepeat;
        if (isEqual)
        {
            toRepeat = new List<PlanetBody>();
            toRepeat.AddRange(ctx.leading);
            toRepeat.AddRange(ctx.trailing);
        }
        else if (ctx.leading.Count < ctx.trailing.Count)
            toRepeat = ctx.leading;
        else
            toRepeat = ctx.trailing;

        for (int r = 0; r < repeatCount; r++)
        {
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
                    extraHits = 0,
                    areaMultiplier = ctx.areaMultiplier,
                    isPhaseActive = false,
                    result = ctx.result
                };
                effect.Execute(subCtx);
            }
        }
    }
}
