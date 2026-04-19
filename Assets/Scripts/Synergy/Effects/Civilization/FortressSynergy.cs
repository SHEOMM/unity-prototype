using UnityEngine;

/// <summary>
/// Civilization tier 3 — 요새. 강력한 구조물 1개 설치 (Fortress behavior 필요 — Barrier 재사용 가능).
/// 데이터(structureToSpawn)에 원하는 behavior/스탯 SO를 바인딩해 차별화.
/// </summary>
[SynergyId("civ_3")]
public class FortressSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || rule.structureToSpawn == null)
        {
            Debug.LogWarning("[Synergy] Fortress: rule.structureToSpawn 미설정");
            return;
        }
        int count = rule.spawnCount > 0 ? rule.spawnCount : 1;
        StructureSpawner.Spawn(rule.structureToSpawn, rule.spawnArea, count);
        Debug.Log($"[Synergy] Fortress: spawned {count} '{rule.structureToSpawn.structureName}' in {rule.spawnArea}");
    }
}
