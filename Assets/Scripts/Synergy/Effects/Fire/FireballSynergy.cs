using UnityEngine;

/// <summary>
/// Fire tier 1 — 소형 파이어볼. 랜덤 적 위치에 작은 원형 AoE.
/// 파라미터: damage, radius, element(기본 Fire).
/// </summary>
[SynergyId("fire_1")]
public class FireballSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;
        var center = PickRandomEnemyPos(ctx, Vector2.zero);
        var element = rule.element == Element.None ? Element.Fire : rule.element;
        int n = AoeApplicator.Damage(center, rule.radius, rule.damage, element);
        Debug.Log($"[Synergy] Fireball: center={center} r={rule.radius} dmg={rule.damage} hit={n}");
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
