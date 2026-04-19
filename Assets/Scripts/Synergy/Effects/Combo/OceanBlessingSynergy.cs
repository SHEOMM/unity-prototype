using UnityEngine;

/// <summary>
/// Combo — 바다의 축복. sequenceKeys=[tide, depth, heart] 모두 포함 시 발동.
/// 전체 아군 Heal (damage 값) + 전체 적 Slow (secondary 값 배율, duration 지속).
/// </summary>
[SynergyId("combo_ocean_blessing")]
public class OceanBlessingSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;

        int healed = AllyHealApplicator.HealAll(rule.damage);

        int slowed = 0;
        if (ctx.Enemies != null)
        {
            foreach (var e in ctx.Enemies)
            {
                if (e == null || !e.IsAlive) continue;
                SlowApplicator.Apply(e, rule.secondary, rule.duration);
                slowed++;
            }
        }
        Debug.Log($"[Synergy] OceanBlessing: healed={healed} slowed={slowed} (slow={rule.secondary} {rule.duration}s)");
    }
}
