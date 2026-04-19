using UnityEngine;

/// <summary>
/// Fire tier 3 — 불바다. 큰 AoE 즉시 피해 + 반경 내 각 적에게 DoT.
/// 파라미터: damage(즉시/tick 공용), radius, duration, secondary(tickInterval), count(미사용).
/// </summary>
[SynergyId("fire_3")]
public class InfernoSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || EnemyRegistry.Instance == null) return;

        var center = PickRandomEnemyPos(ctx, Vector2.zero);
        var element = rule.element == Element.None ? Element.Fire : rule.element;

        int hits = 0;
        foreach (var e in EnemyRegistry.Instance.GetNearby(center, rule.radius))
        {
            if (e == null || !e.IsAlive) continue;
            e.TakeDamage(rule.damage, element);
            DotApplicator.Apply(e, rule.damage, rule.secondary, rule.duration, element);
            hits++;
        }
        Debug.Log($"[Synergy] Inferno: center={center} r={rule.radius} dmg={rule.damage} tick={rule.secondary}s for {rule.duration}s hit={hits}");
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
