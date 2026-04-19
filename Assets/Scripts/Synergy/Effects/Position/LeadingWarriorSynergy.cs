using UnityEngine;

/// <summary>
/// Position — 선봉의 전사. 시퀀스 맨 앞이 valor 키워드 행성일 때 발동.
/// 전역 큰 반경 AoE (Fire 속성). 데이터: damage, radius.
/// </summary>
[SynergyId("pos_leading_warrior")]
public class LeadingWarriorSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;
        var element = rule.element == Element.None ? Element.Fire : rule.element;
        int n = AoeApplicator.Damage(Vector2.zero, rule.radius, rule.damage, element);
        Debug.Log($"[Synergy] LeadingWarrior: center=0,0 r={rule.radius} dmg={rule.damage} hit={n}");
    }
}
