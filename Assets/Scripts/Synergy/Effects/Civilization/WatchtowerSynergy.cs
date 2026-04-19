using UnityEngine;

/// <summary>
/// Civilization tier 1 — 감시탑. 공격 구조물 1개 설치.
/// 파라미터: structureToSpawn(watchtower behavior), spawnArea, spawnCount(=1).
/// </summary>
[SynergyId("civ_1")]
public class WatchtowerSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || rule.structureToSpawn == null)
        {
            Debug.LogWarning("[Synergy] Watchtower: rule.structureToSpawn 미설정");
            return;
        }
        int count = rule.spawnCount > 0 ? rule.spawnCount : 1;
        StructureSpawner.Spawn(rule.structureToSpawn, rule.spawnArea, count);
        Debug.Log($"[Synergy] Watchtower: spawned {count} '{rule.structureToSpawn.structureName}' in {rule.spawnArea}");
    }
}
