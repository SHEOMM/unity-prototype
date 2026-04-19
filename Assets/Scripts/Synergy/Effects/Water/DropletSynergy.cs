using UnityEngine;

/// <summary>
/// Water tier 1 — 물방울. 임의의 적 1명에게 소량 데미지 + 짧은 slow.
/// 파라미터: damage, secondary(slowFactor), duration.
/// </summary>
[SynergyId("water_1")]
public class DropletSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;
        var target = PickRandomEnemy(ctx);
        if (target == null) return;

        var element = rule.element == Element.None ? Element.Water : rule.element;
        target.TakeDamage(rule.damage, element);
        SlowApplicator.Apply(target, rule.secondary, rule.duration);
        Debug.Log($"[Synergy] Droplet: target={target.name} dmg={rule.damage} slow={rule.secondary} for {rule.duration}s");
    }

    static Enemy PickRandomEnemy(SynergyContext ctx)
    {
        var enemies = ctx.Enemies;
        if (enemies == null || enemies.Count == 0) return null;
        int idx = ctx.Rng != null ? ctx.Rng.Next(enemies.Count) : 0;
        return enemies[idx];
    }
}
