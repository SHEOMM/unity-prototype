using UnityEngine;

/// <summary>
/// Earth tier 3 — 벽. rule.structureToSpawn(=BarrierBehavior 탑재)을 spawnArea에 count마리 설치.
/// 적을 지속 넉백해 진입 차단. 파라미터: structureToSpawn, spawnArea, spawnCount.
/// </summary>
[SynergyId("earth_3")]
public class WallSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || rule.structureToSpawn == null)
        {
            Debug.LogWarning("[Synergy] Wall: rule.structureToSpawn 미설정");
            return;
        }
        StructureSpawner.Spawn(rule.structureToSpawn, rule.spawnArea, rule.spawnCount);
        Debug.Log($"[Synergy] Wall: spawned {rule.spawnCount} '{rule.structureToSpawn.structureName}' in {rule.spawnArea}");
    }
}
