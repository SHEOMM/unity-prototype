using UnityEngine;

/// <summary>
/// Lightning tier 2 — 낙뢰. 타깃 AoE + 피격 적 각에 Stun.
/// 파라미터: damage, radius, duration(stun 시간).
/// </summary>
[SynergyId("lightning_2")]
public class ThunderboltSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || EnemyRegistry.Instance == null) return;

        var center = PickRandomEnemyPos(ctx, Vector2.zero);
        int hits = 0;
        foreach (var e in EnemyRegistry.Instance.GetNearby(center, rule.radius))
        {
            if (e == null || !e.IsAlive) continue;
            e.TakeDamage(rule.damage, rule.element);
            StunApplicator.Apply(e, rule.duration);
            hits++;
        }
        Debug.Log($"[Synergy] Thunderbolt: center={center} r={rule.radius} dmg={rule.damage} stun={rule.duration}s hit={hits}");
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
