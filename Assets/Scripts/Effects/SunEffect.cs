using UnityEngine;

/// <summary>
/// 태양: 태양 폭발. 모든 적에게 강력한 화염 데미지.
/// 위상: 플레어 — 전체 화염 AoE + 배율 증폭.
/// </summary>
[EffectId("sun")]
public class SunEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        int enemyCount = Mathf.Max(1, ctx.enemies.Count);

        if (ctx.isPhaseActive)
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Fire,
                damage = dmg * 2f,
                hitCount = enemyCount,
                sourceName = ctx.source.Planet.bodyName + " (플레어)",
                visualType = SpellVisualType.AreaOfEffect
            });
        }
        else
        {
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Fire,
                damage = dmg,
                hitCount = enemyCount + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.AreaOfEffect
            });
        }
    }
}
