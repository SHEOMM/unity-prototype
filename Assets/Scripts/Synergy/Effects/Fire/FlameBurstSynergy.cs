using UnityEngine;

/// <summary>
/// Fire tier 2 — 플레임 버스트. 중형 AoE (Fireball보다 큰 반경/데미지).
/// 동일 primitive 재사용 — 데이터(rule.radius/damage)로만 차별화.
/// </summary>
[SynergyId("fire_2")]
public class FlameBurstSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;
        var center = PickRandomEnemyPos(ctx, Vector2.zero);
        var element = rule.element == Element.None ? Element.Fire : rule.element;
        int n = AoeApplicator.Damage(center, rule.radius, rule.damage, element);
        Debug.Log($"[Synergy] FlameBurst: center={center} r={rule.radius} dmg={rule.damage} hit={n}");
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
