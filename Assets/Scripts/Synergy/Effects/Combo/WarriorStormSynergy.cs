using UnityEngine;

/// <summary>
/// Combo — 전사의 폭풍. sequenceKeys=[blade, valor, storm] 모두 포함 시 발동.
/// 전역 Fire AoE + 랜덤 적에서 ChainLightning count회.
/// 파라미터: damage (AoE+chain 공용), radius (AoE 반경), secondary (chain jumpRadius), count (chain jumps).
/// </summary>
[SynergyId("combo_warrior_storm")]
public class WarriorStormSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;

        int aoeHits = AoeApplicator.Damage(Vector2.zero, rule.radius, rule.damage, Element.Fire);

        var origin = PickRandomEnemyPos(ctx, Vector2.zero);
        int jumps = rule.count > 0 ? rule.count : 4;
        float jumpR = rule.secondary > 0f ? rule.secondary : 3f;
        int chainHits = ChainLightning.Chain(origin, jumps, rule.damage, jumpR, Element.Wind);

        Debug.Log($"[Synergy] WarriorStorm: AoE hit={aoeHits} + Chain jumps={jumps} hit={chainHits}");
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
