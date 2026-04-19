using UnityEngine;

/// <summary>
/// PerHit — 전령의 축복. herald 키워드 행성 터치 시 즉시 발화.
/// 플레이어에게 Wind 속성 짧은 보너스.
/// 파라미터: secondary(보너스 크기), duration.
/// </summary>
[SynergyId("hit_messenger_boost")]
public class MessengerBoostSynergy : SynergyEffectBase
{
    public override void OnHit(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;

        var element = rule.element == Element.None ? Element.Wind : rule.element;
        PlayerBuff.Apply(element, rule.secondary, rule.duration);
        Debug.Log($"[Synergy] MessengerBoost: Player+{element} +{rule.secondary} for {rule.duration}s");
    }
}
