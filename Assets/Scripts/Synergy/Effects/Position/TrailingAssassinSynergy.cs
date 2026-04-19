using UnityEngine;

/// <summary>
/// Position — 암살자의 마무리. 시퀀스 맨 뒤가 shadow 키워드 행성일 때 발동.
/// 전체 적 순회 처형 시도 (hpRatio 이하만). 파라미터: secondary(hpRatio).
/// </summary>
[SynergyId("pos_trailing_assassin")]
public class TrailingAssassinSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || ctx.Enemies == null) return;

        float hpRatio = rule.secondary > 0f ? rule.secondary : 0.3f;
        int executed = 0, attempts = 0;
        foreach (var e in ctx.Enemies)
        {
            if (e == null || !e.IsAlive) continue;
            attempts++;
            if (ExecuteApplicator.TryExecute(e, hpRatio)) executed++;
        }
        Debug.Log($"[Synergy] TrailingAssassin: hpRatio={hpRatio} executed={executed}/{attempts}");
    }
}
