using UnityEngine;

/// <summary>
/// SynergyDispatcher.OnSynergyFired를 구독해 rule.visualId에 해당하는 ISynergyVisual을 실행.
/// 책임 분리: Host는 anchor 계산 + 코루틴 구동, Visual은 애니메이션 렌더만.
///
/// Anchor 규칙:
/// - PerHitPlanet: ctx.CurrentPlanet 위치 (터치 지점)
/// - 그 외: 첫 번째 살아있는 Enemy 위치, 없으면 (0,0)
///
/// Visual 코루틴은 Host에 붙어 있으므로 Host 파괴 시 자동 정리됨.
/// </summary>
public class SynergyVisualHost : MonoBehaviour
{
    private SynergyDispatcher _dispatcher;

    public void Bind(SynergyDispatcher dispatcher)
    {
        Unbind();
        _dispatcher = dispatcher;
        if (_dispatcher != null) _dispatcher.OnSynergyFired += HandleFired;
    }

    void Unbind()
    {
        if (_dispatcher != null) _dispatcher.OnSynergyFired -= HandleFired;
        _dispatcher = null;
    }

    void OnDestroy() { Unbind(); }

    void HandleFired(SynergyRuleSO rule, SynergyContext synergyCtx)
    {
        if (rule == null) return;
        string id = string.IsNullOrEmpty(rule.visualId) ? "default" : rule.visualId;
        var visual = SynergyVisualRegistry.Get(id);
        if (visual == null) return; // 미등록이고 default도 없으면 조용히 무시

        var ctx = new SynergyVisualContext
        {
            Rule = rule,
            Synergy = synergyCtx,
            Anchor = ResolveAnchor(rule, synergyCtx),
            ElementColor = SynergyVisualElementPalette.Resolve(rule.element),
        };
        StartCoroutine(visual.Play(ctx));
    }

    static Vector3 ResolveAnchor(SynergyRuleSO rule, SynergyContext ctx)
    {
        if (rule.triggerType == SynergyTriggerType.PerHitPlanet && ctx?.CurrentPlanet != null)
            return ctx.CurrentPlanet.transform.position;

        if (ctx?.Enemies != null)
        {
            foreach (var e in ctx.Enemies)
            {
                if (e != null && e.IsAlive) return e.transform.position;
            }
        }
        return Vector3.zero;
    }
}
