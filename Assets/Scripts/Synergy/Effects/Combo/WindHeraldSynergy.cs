using UnityEngine;

/// <summary>
/// Combo — 바람의 전령. sequenceKeys=[swift, herald, marksman] 모두 포함 시 발동.
/// 전체 적 넉백 + 플레이어 Wind 속성 보너스 (duration).
/// 파라미터: secondary(knockback 강도 / PlayerBuff 보너스 공용), duration, radius(넉백 반경 가드).
/// </summary>
[SynergyId("combo_wind_herald")]
public class WindHeraldSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;

        int knocked = 0;
        if (ctx.Enemies != null)
        {
            Vector2 center = Vector2.zero;
            foreach (var e in ctx.Enemies)
            {
                if (e == null || !e.IsAlive) continue;
                Vector2 dir = (Vector2)e.transform.position - center;
                if (dir.sqrMagnitude < 1e-4f) dir = Vector2.right;
                KnockbackApplicator.Apply(e, dir, rule.secondary);
                knocked++;
            }
        }

        PlayerBuff.Apply(Element.Wind, rule.secondary, rule.duration);
        Debug.Log($"[Synergy] WindHerald: knocked={knocked} kb={rule.secondary}, Player+Wind={rule.secondary} {rule.duration}s");
    }
}
