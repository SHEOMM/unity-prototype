using System.Collections;
using UnityEngine;

/// <summary>
/// 가로 파동 — Sweep 계열 (TidalWave 등). 기준점 y 라인에서 좌→우로 물결 이동.
/// Aquarius 행성 visual을 참고한 패턴을 시너지 컨텍스트로 재구성.
/// </summary>
[SynergyVisualId("sweep")]
public class SweepVisual : ISynergyVisual
{
    public IEnumerator Play(SynergyVisualContext ctx)
    {
        float waveY = ctx.Anchor.y;
        float spanHalf = Mathf.Max(3f, (ctx.Rule?.radius ?? 3f) * 2f);
        float startX = ctx.Anchor.x - spanHalf;
        float endX = ctx.Anchor.x + spanHalf;
        float duration = 0.45f;

        var fx = new GameObject("FX_Sweep");
        var lr = fx.AddComponent<LineRenderer>();
        lr.material = GameConstants.SpriteMaterial;
        lr.sortingOrder = GameConstants.SortingOrder.SpellEffect;
        lr.startWidth = 0.4f;
        lr.endWidth = 0.12f;

        const int wavePoints = 16;
        lr.positionCount = wavePoints;

        float elapsed = 0f;
        var c = ctx.ElementColor;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float headX = Mathf.Lerp(startX, endX, t);

            for (int i = 0; i < wavePoints; i++)
            {
                float segT = (float)i / (wavePoints - 1);
                float x = headX - 2.5f * segT;
                float y = waveY + Mathf.Sin(segT * Mathf.PI * 3f + elapsed * 18f) * 0.35f;
                lr.SetPosition(i, new Vector3(x, y, 0));
            }
            float alpha = 1f - t * 0.6f;
            lr.startColor = new Color(c.r, c.g, c.b, alpha);
            lr.endColor = new Color(c.r, c.g, c.b, alpha * 0.3f);
            yield return null;
        }
        Object.Destroy(fx);
    }
}
