using System.Collections;
using UnityEngine;

/// <summary>
/// 시너지 비주얼 기본/폴백. Anchor 위치에 작은 원형 펄스 0.3초.
/// visualId가 "default"이거나, 미지정일 때 Host가 이것으로 대체.
/// </summary>
[SynergyVisualId("default")]
public class DefaultSynergyVisual : ISynergyVisual
{
    public IEnumerator Play(SynergyVisualContext ctx)
    {
        var fx = new GameObject("FX_SynergyDefault");
        fx.transform.position = ctx.Anchor;
        var lr = fx.AddComponent<LineRenderer>();
        lr.material = GameConstants.SpriteMaterial;
        lr.sortingOrder = GameConstants.SortingOrder.SpellEffect;
        lr.loop = true;
        lr.useWorldSpace = false;

        const int segs = 24;
        lr.positionCount = segs;

        float duration = 0.3f;
        float elapsed = 0f;
        float maxR = 1.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float r = Mathf.Lerp(0.1f, maxR, t);
            float width = Mathf.Lerp(0.1f, 0.02f, t);
            float alpha = 1f - t;

            for (int i = 0; i < segs; i++)
            {
                float a = i * Mathf.PI * 2f / segs;
                lr.SetPosition(i, new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r, 0));
            }
            lr.startWidth = lr.endWidth = width;
            var c = ctx.ElementColor;
            var fade = new Color(c.r, c.g, c.b, alpha);
            lr.startColor = lr.endColor = fade;
            yield return null;
        }
        Object.Destroy(fx);
    }
}
