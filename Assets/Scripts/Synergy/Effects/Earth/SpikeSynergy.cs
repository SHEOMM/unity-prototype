using UnityEngine;

/// <summary>
/// Earth tier 1 — 가시. 가장 가까운 적 1명에게 단일 데미지. 근접 강한 타격.
/// 파라미터: damage.
/// </summary>
[SynergyId("earth_1")]
public class SpikeSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || ctx.Enemies == null) return;

        // 화면 중앙(원점) 기준 최근접 1마리 — 기준점은 Ship 위치가 더 적절하지만 스냅샷 기반이므로 원점 채택.
        Vector2 origin = Vector2.zero;
        Enemy best = null;
        float bestDistSq = float.MaxValue;
        foreach (var e in ctx.Enemies)
        {
            if (e == null || !e.IsAlive) continue;
            float d = ((Vector2)e.transform.position - origin).sqrMagnitude;
            if (d < bestDistSq) { bestDistSq = d; best = e; }
        }
        if (best == null) return;

        var element = rule.element == Element.None ? Element.Earth : rule.element;
        best.TakeDamage(rule.damage, element);
        Debug.Log($"[Synergy] Spike: target={best.name} dmg={rule.damage}");
    }
}
