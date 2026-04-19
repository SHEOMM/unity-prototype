using UnityEngine;

/// <summary>
/// Position — 뒤따르는 폭풍. 시퀀스 맨 뒤가 storm 키워드 행성일 때 발동.
/// 랜덤 적에서 시작해 count회 ChainLightning. 파라미터: damage, radius, count.
/// </summary>
[SynergyId("pos_trailing_storm")]
public class TrailingStormSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;
        var origin = PickRandomEnemyPos(ctx, Vector2.zero);
        int jumps = rule.count > 0 ? rule.count : 4;
        int n = ChainLightning.Chain(origin, jumps, rule.damage, rule.radius, rule.element);
        Debug.Log($"[Synergy] TrailingStorm: origin={origin} jumps={jumps} dmg={rule.damage} r={rule.radius} hit={n}");
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
