using UnityEngine;

/// <summary>
/// Position — 독의 추격. 시퀀스 맨 뒤가 venom 키워드 행성일 때 발동.
/// 전체 적에 DoT (Darkness). 파라미터: damage(per tick), duration, secondary(tickInterval).
/// </summary>
[SynergyId("pos_trailing_scorpion")]
public class TrailingScorpionSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || ctx.Enemies == null) return;

        var element = rule.element == Element.None ? Element.Darkness : rule.element;
        int n = 0;
        foreach (var e in ctx.Enemies)
        {
            if (e == null || !e.IsAlive) continue;
            DotApplicator.Apply(e, rule.damage, rule.secondary, rule.duration, element);
            n++;
        }
        Debug.Log($"[Synergy] TrailingScorpion: DoT {rule.damage}/tick({rule.secondary}s) for {rule.duration}s on {n}");
    }
}
