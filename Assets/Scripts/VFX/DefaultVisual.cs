using System.Collections;
using UnityEngine;

/// <summary>
/// 기본 마법 비주얼. visualId가 없거나 매핑되지 않은 마법의 fallback.
/// 기존 SpellEffectManager의 LineRenderer 로직을 그대로 유지한다.
/// </summary>
public class DefaultVisual : ISpellVisual
{
    public IEnumerator Play(SpellVisualContext ctx)
    {
        if (ctx.target == null) yield break;

        Vector3 origin = ctx.targetPosition + Vector3.up * 4f;
        var fx = new GameObject("FX_Default");
        fx.transform.position = origin;

        var lr = fx.AddComponent<LineRenderer>();
        lr.startWidth = 0.12f;
        lr.endWidth = 0.04f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.sortingOrder = 15;
        lr.startColor = ctx.elementColor;
        lr.endColor = Color.white;
        lr.positionCount = 2;
        lr.SetPosition(0, origin);
        lr.SetPosition(1, ctx.targetPosition);

        if (ctx.target != null)
            ctx.target.TakeDamage(ctx.command.damage, ctx.command.element);

        yield return new WaitForSeconds(0.2f);
        Object.Destroy(fx);
    }
}
