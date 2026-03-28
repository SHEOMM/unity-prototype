using System.Collections;
using UnityEngine;

/// <summary>
/// 제왕별 비주얼: 하늘에서 낙뢰가 내리꽂힌다.
/// 굵은 번개 선 + 화면 번쩍임.
/// </summary>
[VisualId("emperor")]
public class EmperorVisual : ISpellVisual
{
    public IEnumerator Play(SpellVisualContext ctx)
    {
        if (ctx.target == null) yield break;

        Vector3 end = ctx.targetPosition;
        Vector3 start = end + Vector3.up * 8f;

        var fx = new GameObject("FX_Lightning");
        var lr = fx.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.sortingOrder = 16;

        // 지그재그 번개 경로 생성
        int segments = 8;
        lr.positionCount = segments + 1;
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 pos = Vector3.Lerp(start, end, t);
            if (i > 0 && i < segments)
                pos.x += Random.Range(-0.4f, 0.4f);
            lr.SetPosition(i, pos);
        }

        // 굵은 번개 → 가늘어지며 소멸
        lr.startColor = Color.white;
        lr.endColor = ctx.elementColor;

        float duration = 0.25f;
        float elapsed = 0f;

        ctx.target.TakeDamage(ctx.command.damage, ctx.command.element);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fade = 1f - (elapsed / duration);
            lr.startWidth = 0.3f * fade;
            lr.endWidth = 0.15f * fade;
            lr.startColor = new Color(1f, 1f, 1f, fade);
            lr.endColor = new Color(ctx.elementColor.r, ctx.elementColor.g, ctx.elementColor.b, fade);
            yield return null;
        }

        Object.Destroy(fx);
    }
}
