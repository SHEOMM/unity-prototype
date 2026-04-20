using System.Collections;
using UnityEngine;

/// <summary>
/// 궁수별 비주얼: 화살 투사체가 위에서 적에게 포물선으로 날아간다.
/// </summary>
[VisualId("archer")]
public class ArcherVisual : ISpellVisual
{
    public IEnumerator Play(SpellVisualContext ctx)
    {
        if (ctx.target == null) yield break;

        Vector3 start = ctx.targetPosition + new Vector3(Random.Range(-1f, 1f), 5f, 0);
        Vector3 end = ctx.targetPosition;
        Vector3 peak = (start + end) / 2f + Vector3.up * 2f;

        var fx = new GameObject("FX_Arrow");
        var lr = fx.AddComponent<LineRenderer>();
        lr.startWidth = 0.08f;
        lr.endWidth = 0.03f;
        lr.material = GameConstants.SpriteMaterial;
        lr.sortingOrder = GameConstants.SortingOrder.SpellEffect;
        lr.startColor = ctx.elementColor;
        lr.endColor = ctx.elementColor * 0.6f;
        lr.positionCount = 2;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // 베지어 포물선
            Vector3 a = Vector3.Lerp(start, peak, t);
            Vector3 b = Vector3.Lerp(peak, end, t);
            Vector3 pos = Vector3.Lerp(a, b, t);

            fx.transform.position = pos;
            lr.SetPosition(0, pos);
            lr.SetPosition(1, pos + (pos - Vector3.Lerp(
                Vector3.Lerp(start, peak, Mathf.Max(0, t - 0.1f)),
                Vector3.Lerp(peak, end, Mathf.Max(0, t - 0.1f)),
                Mathf.Max(0, t - 0.1f)
            )).normalized * 0.3f);
            yield return null;
        }

        if (ctx.target != null)
            ctx.target.TakeDamage(ctx.command.damage, ctx.command.element);
        Object.Destroy(fx);
    }
}
