using UnityEngine;

/// <summary>
/// Lightning tier 1 — 스파크. 2회 체인 점프.
/// 파라미터: damage, radius(점프 반경), count(점프 수, 기본 2).
/// </summary>
[SynergyId("lightning_1")]
public class SparkSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;
        var origin = PickRandomEnemyPos(ctx, Vector2.zero);
        int jumps = rule.count > 0 ? rule.count : 2;
        int n = ChainLightning.Chain(origin, jumps, rule.damage, rule.radius, rule.element);
        Debug.Log($"[Synergy] Spark: origin={origin} jumps={jumps} dmg={rule.damage} r={rule.radius} hit={n}");
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
