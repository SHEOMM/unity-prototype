using System.Collections;
using UnityEngine;

/// <summary>
/// 스폰 버스트 — Ally/Structure 소환 계열 공용. rule.spawnArea 중심에 펄스.
/// 스폰 개수(spawnCount) 만큼 지연해 작은 펄스 연달아 발생.
/// </summary>
[SynergyVisualId("spawn_burst")]
public class SpawnBurstVisual : ISynergyVisual
{
    public IEnumerator Play(SynergyVisualContext ctx)
    {
        int bursts = Mathf.Clamp(ctx.Rule?.spawnCount ?? 1, 1,
                                 GameConstants.SynergyVisuals.SpawnBurstCountMax);
        Vector3 center = new Vector3(ctx.Rule?.spawnArea.center.x ?? ctx.Anchor.x,
                                     ctx.Rule?.spawnArea.center.y ?? ctx.Anchor.y, 0f);
        float span = Mathf.Max(
            GameConstants.SynergyVisuals.SpawnBurstSpanMin,
            (ctx.Rule?.spawnArea.width ?? GameConstants.SynergyVisuals.SpawnBurstSpawnAreaFallback)
                * GameConstants.SynergyVisuals.SpawnBurstSpawnAreaScale);

        for (int i = 0; i < bursts; i++)
        {
            float ox = Random.Range(-span, span);
            var pos = center + new Vector3(ox, 0, 0);
            var fx = new GameObject("FX_SpawnBurst");
            fx.transform.position = pos;
            var lr = fx.AddComponent<LineRenderer>();
            lr.material = GameConstants.SpriteMaterial;
            lr.sortingOrder = GameConstants.SortingOrder.SpellEffect;
            lr.loop = true;
            lr.useWorldSpace = false;
            int segs = GameConstants.SynergyVisuals.SpawnBurstSegments;
            lr.positionCount = segs;

            float duration = GameConstants.SynergyVisuals.SpawnBurstDuration;
            float elapsed = 0f;
            float maxR = GameConstants.SynergyVisuals.SpawnBurstMaxRadius;
            var c = ctx.ElementColor;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float r = Mathf.Lerp(GameConstants.SynergyVisuals.PulseInitialRadius, maxR, t);
                float width = Mathf.Lerp(GameConstants.SynergyVisuals.SpawnBurstStartWidth,
                                         GameConstants.SynergyVisuals.SpawnBurstEndWidth, t);
                float alpha = 1f - t;
                for (int s = 0; s < segs; s++)
                {
                    float a = s * Mathf.PI * 2f / segs;
                    lr.SetPosition(s, new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r, 0));
                }
                lr.startWidth = lr.endWidth = width;
                lr.startColor = lr.endColor = new Color(c.r, c.g, c.b, alpha);
                yield return null;
            }
            Object.Destroy(fx);
            yield return new WaitForSeconds(GameConstants.SynergyVisuals.SpawnBurstDelayBetween);
        }
    }
}
