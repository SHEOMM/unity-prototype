using UnityEngine;

/// <summary>
/// Dark tier 3 — 역병. 전체 적에 DoT + Weakness(받는 피해 증폭).
/// 파라미터: damage(per tick), duration, secondary(tickInterval), count(weakness amp×10 해석 — 기본 1.5).
/// count는 int라 그대로 쓰고, 증폭은 별도 기본값으로 처리.
/// </summary>
[SynergyId("dark_3")]
public class PlagueSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || ctx.Enemies == null) return;

        var element = rule.element == Element.None ? Element.Darkness : rule.element;
        float weaknessAmp = rule.count > 0 ? rule.count * 0.1f + 1f : 1.5f; // count=5 → x1.5

        int n = 0;
        foreach (var e in ctx.Enemies)
        {
            if (e == null || !e.IsAlive) continue;
            DotApplicator.Apply(e, rule.damage, rule.secondary, rule.duration, element);
            WeaknessApplicator.Apply(e, weaknessAmp, rule.duration);
            n++;
        }
        Debug.Log($"[Synergy] Plague: DoT {rule.damage}/tick + Weakness x{weaknessAmp} for {rule.duration}s on {n}");
    }
}
