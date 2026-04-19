using UnityEngine;

/// <summary>
/// War tier 1 — 모집. 아군 1명 소환.
/// 파라미터: allyToSpawn, spawnArea, spawnCount(=1 기본).
/// </summary>
[SynergyId("war_1")]
public class RecruitSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || rule.allyToSpawn == null)
        {
            Debug.LogWarning("[Synergy] Recruit: rule.allyToSpawn 미설정");
            return;
        }
        int count = rule.spawnCount > 0 ? rule.spawnCount : 1;
        AllySpawner.Spawn(rule.allyToSpawn, rule.spawnArea, count);
        Debug.Log($"[Synergy] Recruit: spawned {count} '{rule.allyToSpawn.allyName}' in {rule.spawnArea}");
    }
}
