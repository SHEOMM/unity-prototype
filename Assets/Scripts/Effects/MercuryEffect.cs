using UnityEngine;

/// <summary>
/// 수성: 빠른 단타. 도킹 전 선행 행성 수만큼 데미지 증폭.
/// 위상: 연속 발사 — 적 전체 속사 타격.
/// </summary>
[EffectId("mercury")]
public class MercuryEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        int boost = 1 + ctx.leading.Count;

        if (ctx.isPhaseActive)
        {
            int enemyCount = Mathf.Max(1, ctx.enemies.Count);
            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = dmg * boost,
                hitCount = enemyCount,
                sourceName = ctx.source.Planet.bodyName + " (연속발사)",
                visualType = SpellVisualType.Projectile
            });
        }
        else
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = dmg * boost,
                hitCount = 1 + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Projectile
            });
        }
    }
}
