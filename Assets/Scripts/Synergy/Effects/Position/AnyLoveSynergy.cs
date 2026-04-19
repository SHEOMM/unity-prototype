using UnityEngine;

/// <summary>
/// Position — 사랑의 은혜. 시퀀스에 heart 키워드 행성이 포함되면 발동.
/// 살아있는 모든 아군 Heal. 파라미터: damage(=heal 양으로 해석).
/// </summary>
[SynergyId("pos_any_love")]
public class AnyLoveSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;
        float amount = rule.damage > 0f ? rule.damage : 15f;
        int n = AllyHealApplicator.HealAll(amount);
        Debug.Log($"[Synergy] AnyLove: healed {n} allies by {amount}");
    }
}
