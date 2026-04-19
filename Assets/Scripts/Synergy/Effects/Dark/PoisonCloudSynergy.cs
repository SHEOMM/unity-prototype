using UnityEngine;

/// <summary>
/// Dark tier 1 — 독 구름. 반경 내 적에게 DoT 부착.
/// 파라미터: damage(per tick), radius, duration, secondary(tickInterval).
/// </summary>
[SynergyId("dark_1")]
public class PoisonCloudSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || EnemyRegistry.Instance == null) return;

        var center = PickRandomEnemyPos(ctx, Vector2.zero);
        var element = rule.element == Element.None ? Element.Darkness : rule.element;

        int n = 0;
        foreach (var e in EnemyRegistry.Instance.GetNearby(center, rule.radius))
        {
            if (e == null || !e.IsAlive) continue;
            DotApplicator.Apply(e, rule.damage, rule.secondary, rule.duration, element);
            n++;
        }
        Debug.Log($"[Synergy] PoisonCloud: center={center} r={rule.radius} dmg={rule.damage}/tick({rule.secondary}s) for {rule.duration}s on {n}");
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
