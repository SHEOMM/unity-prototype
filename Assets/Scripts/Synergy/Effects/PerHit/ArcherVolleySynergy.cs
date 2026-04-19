using UnityEngine;

/// <summary>
/// PerHit — 궁수의 일제사격. marksman 키워드 행성 터치 시 즉시 발화.
/// 터치 지점 주변 소형 AoE. 파라미터: damage, radius, element(기본 Wind).
/// </summary>
[SynergyId("hit_archer_volley")]
public class ArcherVolleySynergy : SynergyEffectBase
{
    public override void OnHit(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || ctx.CurrentPlanet == null) return;

        var element = rule.element == Element.None ? Element.Wind : rule.element;
        Vector2 center = ctx.CurrentPlanet.transform.position;
        int n = AoeApplicator.Damage(center, rule.radius, rule.damage, element);
        Debug.Log($"[Synergy] ArcherVolley: hit={ctx.CurrentPlanet.Planet.bodyName} center={center} r={rule.radius} dmg={rule.damage} targets={n}");
    }
}
