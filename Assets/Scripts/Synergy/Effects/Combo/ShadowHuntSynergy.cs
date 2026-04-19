using UnityEngine;

/// <summary>
/// Combo — 그림자의 사냥. sequenceKeys=[shadow, venom] 포함 시 발동.
/// 전체 적에 DoT + HP 비율 이하는 처형 (sweep execute).
/// 파라미터: damage(per tick), duration, secondary(tickInterval), count=hpRatio*100 해석 안함.
/// hpRatio는 별도 rule 해석: 기본 0.3, rule.secondary를 hpRatio로 사용하려면 필드가 부족 →
/// 여기서는 count를 hpRatio*100 의미로 쓰지 않고, 편의상 hpRatio=0.3 고정 + tickInterval=secondary.
/// </summary>
[SynergyId("combo_shadow_hunt")]
public class ShadowHuntSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || ctx.Enemies == null) return;

        float hpRatio = 0.3f; // Combo는 강력 → 고정 30% 처형 임계
        float tickInterval = rule.secondary > 0f ? rule.secondary : 0.5f;

        int dotApplied = 0, executed = 0;
        foreach (var e in ctx.Enemies)
        {
            if (e == null || !e.IsAlive) continue;
            DotApplicator.Apply(e, rule.damage, tickInterval, rule.duration, Element.Darkness);
            dotApplied++;
            if (ExecuteApplicator.TryExecute(e, hpRatio)) executed++;
        }
        Debug.Log($"[Synergy] ShadowHunt: DoT on {dotApplied}, executed {executed} (hpRatio={hpRatio})");
    }
}
