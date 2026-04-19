using System.Collections;
using UnityEngine;

/// <summary>
/// 원형 펄스 — AoE 시너지 공용. Rule.radius를 최대 반경으로 삼아 확장하며 페이드.
/// Fire/Wind/Earth/Lightning 대부분의 AoE 시너지가 이것을 공유. Element별 색상은 Palette가 결정.
/// </summary>
[SynergyVisualId("area_pulse")]
public class AreaPulseVisual : ISynergyVisual
{
    public IEnumerator Play(SynergyVisualContext ctx)
    {
        float maxR = Mathf.Max(0.5f, ctx.Rule?.radius ?? 2f);
        float duration = Mathf.Clamp(maxR * 0.12f, 0.25f, 0.6f);

        var fx = new GameObject("FX_AreaPulse");
        fx.transform.position = ctx.Anchor;
        var lr = fx.AddComponent<LineRenderer>();
        lr.material = GameConstants.SpriteMaterial;
        lr.sortingOrder = 15;
        lr.loop = true;
        lr.useWorldSpace = false;

        const int segs = 40;
        lr.positionCount = segs;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float r = Mathf.Lerp(0.1f, maxR, t);
            float width = Mathf.Lerp(0.22f, 0.04f, t);
            float alpha = 1f - t;

            for (int i = 0; i < segs; i++)
            {
                float a = i * Mathf.PI * 2f / segs;
                lr.SetPosition(i, new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r, 0));
            }
            lr.startWidth = lr.endWidth = width;
            var c = ctx.ElementColor;
            lr.startColor = lr.endColor = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        Object.Destroy(fx);
    }
}
