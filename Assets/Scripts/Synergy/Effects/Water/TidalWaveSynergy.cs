using UnityEngine;

/// <summary>
/// Water tier 3 — 해일. 좌→우 (또는 반대) 직선 관통 피격 + 강넉백.
/// 파라미터: damage, radius(=폭), secondary(넉백 강도), spawnArea(시작/끝 x 범위 해석).
/// </summary>
[SynergyId("water_3")]
public class TidalWaveSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;

        var element = rule.element == Element.None ? Element.Water : rule.element;

        // spawnArea를 해일 경로로 재활용: y=중심y, x=왼쪽 끝 → 오른쪽 끝, 폭=radius
        float y = rule.spawnArea.center.y;
        var start = new Vector2(rule.spawnArea.xMin, y);
        var end = new Vector2(rule.spawnArea.xMax, y);

        int n = SweepLine.Sweep(start, end, rule.radius, rule.damage, element, piercing: true, knockbackStrength: rule.secondary);
        Debug.Log($"[Synergy] TidalWave: {start}→{end} w={rule.radius} dmg={rule.damage} kb={rule.secondary} hit={n}");
    }
}
