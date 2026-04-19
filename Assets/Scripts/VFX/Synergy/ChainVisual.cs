using System.Collections;
using UnityEngine;

/// <summary>
/// 지그재그 번개 — Chain 계열 시너지 공용.
/// Anchor에서 시작해 rule.count만큼 무작위 방향으로 세그먼트 이어가며 번쩍이는 라인 하나를 그린다.
/// Visual은 실제 ChainLightning 논리의 타깃과 일치하지 않을 수 있음 — 시각적 대리 표현.
/// </summary>
[SynergyVisualId("chain")]
public class ChainVisual : ISynergyVisual
{
    public IEnumerator Play(SynergyVisualContext ctx)
    {
        int jumps = Mathf.Max(2, ctx.Rule?.count ?? 3);
        float segLen = Mathf.Max(1.5f, (ctx.Rule?.radius ?? 3f) * 0.8f);
        float duration = 0.4f;

        var fx = new GameObject("FX_Chain");
        var lr = fx.AddComponent<LineRenderer>();
        lr.material = GameConstants.SpriteMaterial;
        lr.sortingOrder = 16;
        lr.startWidth = 0.12f;
        lr.endWidth = 0.06f;

        int points = jumps + 1;
        lr.positionCount = points;

        Vector3 p = ctx.Anchor;
        lr.SetPosition(0, p);
        var rng = ctx.Synergy?.Rng ?? new System.Random();
        for (int i = 1; i < points; i++)
        {
            float a = (float)(rng.NextDouble() * Mathf.PI * 2f);
            p += new Vector3(Mathf.Cos(a) * segLen, Mathf.Sin(a) * segLen, 0);
            lr.SetPosition(i, p);
        }

        float elapsed = 0f;
        var baseColor = ctx.ElementColor;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float flicker = Mathf.PingPong(elapsed * 30f, 1f);
            float alpha = (1f - t) * (0.6f + 0.4f * flicker);
            lr.startColor = new Color(1f, 1f, 1f, alpha);
            lr.endColor = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }
        Object.Destroy(fx);
    }
}
