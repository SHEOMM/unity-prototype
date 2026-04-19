using UnityEngine;

/// <summary>
/// Wind tier 2 — 바람 오라. 플레이어에 Wind 속성 보너스 + 모든 아군 이동속도 증가.
/// 파라미터: secondary(dmgBonus / speedMultiplier 공용), duration.
/// </summary>
[SynergyId("wind_2")]
public class WindAuraSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;

        var element = rule.element == Element.None ? Element.Wind : rule.element;
        PlayerBuff.Apply(element, rule.secondary, rule.duration);

        var allies = AllyRegistry.Instance?.GetAll();
        int count = allies?.Count ?? 0;
        if (count > 0)
            AllyBuff.ApplyToAll(allies, damageMultiplier: 1f, speedMultiplier: 1f + rule.secondary, rule.duration);

        Debug.Log($"[Synergy] WindAura: Player+{rule.secondary}({element}), Allies spd x{1f + rule.secondary} on {count} for {rule.duration}s");
    }
}
