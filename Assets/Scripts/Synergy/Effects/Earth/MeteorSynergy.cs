using UnityEngine;

/// <summary>
/// Earth tier 2 — 운석. 랜덤 타깃 위치에 중형 AoE (Earth 속성, 넉백 없음).
/// FlameBurst와 primitive는 동일, Element와 파라미터만 차별화.
/// </summary>
[SynergyId("earth_2")]
public class MeteorSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;
        var center = PickRandomEnemyPos(ctx, Vector2.zero);
        var element = rule.element == Element.None ? Element.Earth : rule.element;
        int n = AoeApplicator.Damage(center, rule.radius, rule.damage, element);
        Debug.Log($"[Synergy] Meteor: center={center} r={rule.radius} dmg={rule.damage} hit={n}");
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
