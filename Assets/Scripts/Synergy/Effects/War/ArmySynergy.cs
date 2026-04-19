using UnityEngine;

/// <summary>
/// War tier 3 — 군대. 신규 아군 3명 소환 + 전체 아군 damage/speed 버프.
/// 파라미터: allyToSpawn, spawnArea, spawnCount(=3), secondary(버프 배율), duration.
/// </summary>
[SynergyId("war_3")]
public class ArmySynergy : SynergyEffectBase
{
    public override void OnFlightEnd(SynergyContext ctx)
    {
        var rule = ctx.CurrentRule;
        if (rule == null) return;

        if (rule.allyToSpawn != null)
        {
            int count = rule.spawnCount > 0 ? rule.spawnCount : 3;
            AllySpawner.Spawn(rule.allyToSpawn, rule.spawnArea, count);
        }

        var allies = AllyRegistry.Instance?.GetAll();
        int buffed = allies?.Count ?? 0;
        if (buffed > 0)
        {
            float mult = rule.secondary > 0f ? rule.secondary : 1.5f;
            AllyBuff.ApplyToAll(allies, damageMultiplier: mult, speedMultiplier: mult, rule.duration);
        }
        Debug.Log($"[Synergy] Army: spawned + buffed {buffed} allies x{rule.secondary} for {rule.duration}s");
    }
}
