using UnityEngine;

/// <summary>
/// Position — 제왕의 낙뢰. 시퀀스 맨 앞이 authority 키워드 행성일 때 발동.
/// 랜덤 적 위치에 중형 AoE + 피격 적 Stun. 파라미터: damage, radius, duration(stun).
/// </summary>
[SynergyId("pos_leading_emperor")]
public class LeadingEmperorSynergy : SynergyEffectBase
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
        Debug.Log($"[Synergy] LeadingEmperor: center={center} r={rule.radius} dmg={rule.damage} stun={rule.duration}s hit={hits}");
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
