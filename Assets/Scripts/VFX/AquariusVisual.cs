using System.Collections;
using UnityEngine;

/// <summary>
/// 물병별 비주얼: 파도가 좌→우로 쓸고 지나가며 적을 관통한다.
/// </summary>
[VisualId("aquarius")]
public class AquariusVisual : ISpellVisual
{
    public IEnumerator Play(SpellVisualContext ctx)
    {
        if (ctx.target == null) yield break;

        float waveY = ctx.targetPosition.y;
        float startX = ctx.targetPosition.x - 3f;
        float endX = ctx.targetPosition.x + 3f;

        var fx = new GameObject("FX_Wave");
        var lr = fx.AddComponent<LineRenderer>();
        lr.material = GameConstants.SpriteMaterial;
        lr.sortingOrder = 15;
        lr.startColor = ctx.elementColor;
        lr.endColor = new Color(ctx.elementColor.r, ctx.elementColor.g, ctx.elementColor.b, 0.3f);
        lr.startWidth = 0.4f;
        lr.endWidth = 0.1f;

        float duration = 0.3f;
        float elapsed = 0f;

        int wavePoints = 12;
        lr.positionCount = wavePoints;

        if (ctx.target != null)
            ctx.target.TakeDamage(ctx.command.damage, ctx.command.element);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentX = Mathf.Lerp(startX, endX, t);

            for (int i = 0; i < wavePoints; i++)
            {
                float segT = (float)i / (wavePoints - 1);
                float x = currentX - 2f * segT;
                float y = waveY + Mathf.Sin(segT * Mathf.PI * 3f + elapsed * 20f) * 0.3f;
                lr.SetPosition(i, new Vector3(x, y, 0));
            }

            float fade = 1f - t * 0.5f;
            lr.startColor = new Color(ctx.elementColor.r, ctx.elementColor.g, ctx.elementColor.b, fade);
            yield return null;
        }

        Object.Destroy(fx);
    }
}
