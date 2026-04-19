using UnityEngine;

/// <summary>
/// PerHit — 대장장이의 단련. forge 키워드 행성 터치 시 즉시 발화.
/// 모든 아군에 AllyBuff (damage×secondary, speed 유지) 짧은 duration.
/// 파라미터: secondary(dmg 배율 — 기본 1.2), duration.
/// </summary>
[SynergyId("hit_smith_reinforce")]
public class SmithReinforceSynergy : SynergyEffectBase
{
    public override void OnHit(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;

        var allies = AllyRegistry.Instance?.GetAll();
        int count = allies?.Count ?? 0;
        if (count == 0) { Debug.Log("[Synergy] SmithReinforce: 아군 없음 — skip"); return; }

        float mult = rule.secondary > 0f ? rule.secondary : 1.2f;
        AllyBuff.ApplyToAll(allies, damageMultiplier: mult, speedMultiplier: 1f, rule.duration);
        Debug.Log($"[Synergy] SmithReinforce: buffed {count} allies dmg x{mult} for {rule.duration}s");
    }
}
