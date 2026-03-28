using UnityEngine;

/// <summary>
/// 궁수별: 천문탑에서 3개의 직사형 화살을 발사한다.
/// 시저지에 포함된 별 2개당 화살이 1개씩 증가한다.
/// </summary>
[EffectId("archer")]
public class ArcherEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        int baseArrows = 3;
        int bonusArrows = ctx.allHits.Count / 2;

        ctx.result.commands.Add(new SpellCommand
        {
            element = ctx.source.Planet.element,
            damage = dmg,
            hitCount = baseArrows + bonusArrows + ctx.extraHits,
            sourceName = ctx.source.Planet.bodyName,
            visualType = SpellVisualType.Projectile,
            visualId = "archer"
        });
    }
}
