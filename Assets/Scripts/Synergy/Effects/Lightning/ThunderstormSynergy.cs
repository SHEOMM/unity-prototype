using UnityEngine;

/// <summary>
/// Lightning tier 3 — 뇌우. 다중 체인 점프 (기본 6회) — 넓은 반경에 연쇄 피해.
/// 파라미터: damage, radius, count (점프 수, 기본 6).
/// </summary>
[SynergyId("lightning_3")]
public class ThunderstormSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;
        var origin = PickRandomEnemyPos(ctx, Vector2.zero);
        int jumps = rule.count > 0 ? rule.count : 6;
        int n = ChainLightning.Chain(origin, jumps, rule.damage, rule.radius, rule.element);
        Debug.Log($"[Synergy] Thunderstorm: origin={origin} jumps={jumps} dmg={rule.damage} r={rule.radius} hit={n}");
    }

    static Vector2 PickRandomEnemyPos(SynergyContext ctx, Vector2 fallback)
    {
        var enemies = ctx.Enemies;
        if (enemies == null || enemies.Count == 0) return fallback;
        int idx = ctx.Rng != null ? ctx.Rng.Next(enemies.Count) : 0;
        var e = enemies[idx];
        return e != null ? (Vector2)e.transform.position : fallback;
    }
}
