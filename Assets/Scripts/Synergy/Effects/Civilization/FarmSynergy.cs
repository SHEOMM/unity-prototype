using UnityEngine;

/// <summary>
/// Civilization tier 2 — 농장. 구조물 다수 설치 (기본 3개) — 감시탑 공격 네트워크.
/// WatchtowerSynergy와 primitive 동일, 데이터(spawnCount)로만 차별화.
/// </summary>
[SynergyId("civ_2")]
public class FarmSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || rule.structureToSpawn == null)
        {
            Debug.LogWarning("[Synergy] Farm: rule.structureToSpawn 미설정");
            return;
        }
        int count = rule.spawnCount > 0 ? rule.spawnCount : 3;
        StructureSpawner.Spawn(rule.structureToSpawn, rule.spawnArea, count);
        Debug.Log($"[Synergy] Farm: spawned {count} '{rule.structureToSpawn.structureName}' in {rule.spawnArea}");
    }
}
