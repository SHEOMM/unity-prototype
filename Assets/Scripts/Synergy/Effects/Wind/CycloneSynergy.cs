using UnityEngine;

/// <summary>
/// Wind tier 3 — 태풍. 전체 적에 강넉백 + 공중 적에 추가 데미지.
/// 파라미터: damage(공중 추뎀), secondary(넉백 강도), radius(기준점 반경 — 벗어난 적 제외).
/// </summary>
[SynergyId("wind_3")]
public class CycloneSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || ctx.Enemies == null) return;

        var center = PickRandomEnemyPos(ctx, Vector2.zero);
        var element = rule.element == Element.None ? Element.Wind : rule.element;

        int knocked = 0, flyingHit = 0;
        foreach (var e in ctx.Enemies)
        {
            if (e == null || !e.IsAlive) continue;
            Vector2 dir = (Vector2)e.transform.position - center;
            if (dir.sqrMagnitude > rule.radius * rule.radius) continue;
            if (dir.sqrMagnitude < 1e-4f) dir = Vector2.right;
            KnockbackApplicator.Apply(e, dir, rule.secondary);
            knocked++;
        }

        foreach (var e in EnemyFiltering.GetFlying())
        {
            if (e == null || !e.IsAlive) continue;
            e.TakeDamage(rule.damage, element);
            flyingHit++;
        }

        Debug.Log($"[Synergy] Cyclone: center={center} r={rule.radius} kb={rule.secondary} knocked={knocked}, flyingExtra={rule.damage} on {flyingHit}");
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
