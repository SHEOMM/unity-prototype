using UnityEngine;

/// <summary>
/// Dark tier 2 — 저주. 반경 내 각 적 대상 처형 시도 (HP 비율 이하).
/// 파라미터: radius, secondary(hpRatio — 기본 0.25).
/// </summary>
[SynergyId("dark_2")]
public class CurseSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || EnemyRegistry.Instance == null) return;

        var center = PickRandomEnemyPos(ctx, Vector2.zero);
        float hpRatio = rule.secondary > 0f ? rule.secondary : 0.25f;

        int executed = 0, attempts = 0;
        foreach (var e in EnemyRegistry.Instance.GetNearby(center, rule.radius))
        {
            if (e == null || !e.IsAlive) continue;
            attempts++;
            if (ExecuteApplicator.TryExecute(e, hpRatio)) executed++;
        }
        Debug.Log($"[Synergy] Curse: center={center} r={rule.radius} hpRatio={hpRatio} executed={executed}/{attempts}");
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
