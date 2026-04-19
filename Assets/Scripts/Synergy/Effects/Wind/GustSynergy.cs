using UnityEngine;

/// <summary>
/// Wind tier 1 — 돌풍. 소형 AoE + 피격 적 각각에 넉백.
/// 파라미터: damage, radius, secondary(넉백 강도).
/// </summary>
[SynergyId("wind_1")]
public class GustSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || EnemyRegistry.Instance == null) return;

        var center = PickRandomEnemyPos(ctx, Vector2.zero);
        var element = rule.element == Element.None ? Element.Wind : rule.element;

        int n = 0;
        foreach (var e in EnemyRegistry.Instance.GetNearby(center, rule.radius))
        {
            if (e == null || !e.IsAlive) continue;
            e.TakeDamage(rule.damage, element);
            Vector2 dir = (Vector2)e.transform.position - center;
            if (dir.sqrMagnitude < 1e-4f) dir = Vector2.right;
            KnockbackApplicator.Apply(e, dir, rule.secondary);
            n++;
        }
        Debug.Log($"[Synergy] Gust: center={center} r={rule.radius} dmg={rule.damage} kb={rule.secondary} hit={n}");
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
