using UnityEngine;

/// <summary>
/// War tier 2 — 분대. 아군 3명 소환. Recruit와 primitive 동일, 데이터로만 차별화.
/// </summary>
[SynergyId("war_2")]
public class SquadSynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null || rule.allyToSpawn == null)
        {
            Debug.LogWarning("[Synergy] Squad: rule.allyToSpawn 미설정");
            return;
        }
        int count = rule.spawnCount > 0 ? rule.spawnCount : 3;
        AllySpawner.Spawn(rule.allyToSpawn, rule.spawnArea, count);
        Debug.Log($"[Synergy] Squad: spawned {count} '{rule.allyToSpawn.allyName}' in {rule.spawnArea}");
    }
}
