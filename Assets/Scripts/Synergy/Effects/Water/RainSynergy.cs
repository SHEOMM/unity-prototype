using UnityEngine;

/// <summary>
/// Water tier 2 — 비. 전체 적에게 slow 적용.
/// 파라미터: secondary(slowFactor), duration.
/// </summary>
[SynergyId("water_2")]
public class RainSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || ctx.Enemies == null) return;

        int n = 0;
        foreach (var e in ctx.Enemies)
        {
            if (e == null || !e.IsAlive) continue;
            SlowApplicator.Apply(e, rule.secondary, rule.duration);
            n++;
        }
        Debug.Log($"[Synergy] Rain: slow={rule.secondary} for {rule.duration}s on {n} enemies");
    }
}
