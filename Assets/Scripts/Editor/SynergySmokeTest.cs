#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Phase 0–2 primitive/확장 스모크 테스트. Play 모드에서 메뉴로 실행.
/// Phase 3 진입 시 실제 시너지가 나오면 이 메뉴는 삭제하거나 축소.
/// </summary>
public static class SynergySmokeTest
{
    [MenuItem("Tools/Synergy/Smoke — Dot (5×6 Fire)")]
    static void SmokeDot()
    {
        var e = GetFirstEnemy();
        if (e == null) return;
        DotApplicator.Apply(e, 5f, 0.5f, 3f, Element.Fire);
        Debug.Log($"[Smoke] DoT 부착: target={e.name}, 5dmg × {3/0.5:F0}tick");
    }

    [MenuItem("Tools/Synergy/Smoke — Slow (0.5 for 3s)")]
    static void SmokeSlow()
    {
        var e = GetFirstEnemy();
        if (e == null) return;
        SlowApplicator.Apply(e, 0.5f, 3f);
        Debug.Log($"[Smoke] Slow 부착: baseSpeed={e.BaseSpeed}, moveSpeed={e.moveSpeed}");
    }

    [MenuItem("Tools/Synergy/Smoke — Knockback (left 5)")]
    static void SmokeKnockback()
    {
        var e = GetFirstEnemy();
        if (e == null) return;
        KnockbackApplicator.Apply(e, Vector2.left, 5f);
        Debug.Log($"[Smoke] Knockback applied to {e.name} (IMoveable)");
    }

    [MenuItem("Tools/Synergy/Smoke — AoE (center origin r=5 dmg=20)")]
    static void SmokeAoe()
    {
        int n = AoeApplicator.Damage(Vector2.zero, 5f, 20f, Element.Fire);
        Debug.Log($"[Smoke] AoE: {n}마리 피격");
    }

    [MenuItem("Tools/Synergy/Smoke — Chain (origin jumps=3 dmg=10 r=3)")]
    static void SmokeChain()
    {
        int n = ChainLightning.Chain(Vector2.zero, 3, 10f, 3f, Element.Wind);
        Debug.Log($"[Smoke] Chain: {n}마리 히트");
    }

    [MenuItem("Tools/Synergy/Smoke — Sweep (L→R y=0 width=1 dmg=10 push=3)")]
    static void SmokeSweep()
    {
        int n = SweepLine.Sweep(new Vector2(-10, 0), new Vector2(10, 0), 1f, 10f, Element.Water, true, 3f);
        Debug.Log($"[Smoke] Sweep: {n}마리 히트");
    }

    [MenuItem("Tools/Synergy/Smoke — Execute (hpRatio 0.5)")]
    static void SmokeExecute()
    {
        var e = GetFirstEnemy();
        if (e == null) return;
        bool ok = ExecuteApplicator.TryExecute(e, 0.5f);
        Debug.Log($"[Smoke] Execute: {(ok ? "처형" : "조건 미충족")} (HP={e.currentHP}/{e.maxHP})");
    }

    [MenuItem("Tools/Synergy/Smoke — PlayerBuff (Fire +0.5 for 5s)")]
    static void SmokePlayerBuff()
    {
        var scope = PlayerBuff.Apply(Element.Fire, 0.5f, 5f);
        Debug.Log($"[Smoke] PlayerBuff: Fire +0.5 (현재 bonus={PlayerState.Instance?.GetElementBonus(Element.Fire)})");
    }

    [MenuItem("Tools/Synergy/Smoke — Filter flying/ground count")]
    static void SmokeFilter()
    {
        int flying = 0, ground = 0;
        foreach (var _ in EnemyFiltering.GetFlying()) flying++;
        foreach (var _ in EnemyFiltering.GetGround()) ground++;
        Debug.Log($"[Smoke] flying={flying}, ground={ground}");
    }

    // ── Phase 2: Ally/Structure ──────────────────────────────

    [MenuItem("Tools/Synergy/Smoke — Spawn test Ally at (0,-2)")]
    static void SmokeSpawnAlly()
    {
        var so = ScriptableObject.CreateInstance<AllySO>();
        so.allyName = "TestAlly";
        so.baseHP = 50f; so.moveSpeed = 2f; so.attackDamage = 8f;
        so.attackRange = 1.5f; so.attackInterval = 0.8f; so.scale = 0.4f;
        so.bodyColor = new Color(0.3f, 0.8f, 1f);
        var ally = AllySpawner.SpawnAt(so, new Vector2(0, -2));
        Debug.Log($"[Smoke] Ally 스폰: {ally?.name}, HP={ally?.CurrentHP}, IsAlive={ally?.IsAlive}");
    }

    [MenuItem("Tools/Synergy/Smoke — Spawn test Structure at (2,-2)")]
    static void SmokeSpawnStructure()
    {
        var so = ScriptableObject.CreateInstance<StructureSO>();
        so.structureName = "TestBarrier";
        so.baseHP = 100f; so.scale = 0.6f;
        so.bodyColor = new Color(0.6f, 0.8f, 0.3f);
        var s = StructureSpawner.SpawnAt(so, new Vector2(2, -2));
        Debug.Log($"[Smoke] Structure 스폰: {s?.name}, HP={s?.CurrentHP}, IsAlive={s?.IsAlive}");
    }

    [MenuItem("Tools/Synergy/Smoke — Dot on first Ally")]
    static void SmokeDotOnAlly()
    {
        var a = AllyRegistry.Instance?.GetAll();
        if (a == null || a.Count == 0) { Debug.LogWarning("[Smoke] 아군 없음"); return; }
        DotApplicator.Apply(a[0], 5f, 0.5f, 3f, Element.Darkness);
        Debug.Log($"[Smoke] Ally DoT 부착: {a[0].name}");
    }

    [MenuItem("Tools/Synergy/Smoke — Damage first Structure (30)")]
    static void SmokeDamageStructure()
    {
        var s = StructureRegistry.Instance?.GetAll();
        if (s == null || s.Count == 0) { Debug.LogWarning("[Smoke] 구조물 없음"); return; }
        s[0].TakeDamage(30f);
        Debug.Log($"[Smoke] Structure 피격: {s[0].name} HP={s[0].CurrentHP}/{s[0].MaxHP}");
    }

    // ── Phase 2.5: Stun/Weakness/AllyBuff + concrete behaviors ───────────

    [MenuItem("Tools/Synergy/Smoke — Stun first enemy (3s)")]
    static void SmokeStun()
    {
        var e = GetFirstEnemy();
        if (e == null) return;
        StunApplicator.Apply(e, 3f);
        Debug.Log($"[Smoke] Stun 부착: {e.name}, 3초간 moveSpeed=0");
    }

    [MenuItem("Tools/Synergy/Smoke — Weakness first enemy (x1.5, 5s)")]
    static void SmokeWeakness()
    {
        var e = GetFirstEnemy();
        if (e == null) return;
        WeaknessApplicator.Apply(e, 1.5f, 5f);
        Debug.Log($"[Smoke] Weakness 부착: {e.name}, 받는 피해 ×1.5");
    }

    [MenuItem("Tools/Synergy/Smoke — Ally buff all (dmg×1.5 spd×1.3 4s)")]
    static void SmokeAllyBuff()
    {
        var allies = AllyRegistry.Instance?.GetAll();
        if (allies == null || allies.Count == 0) { Debug.LogWarning("[Smoke] 아군 없음"); return; }
        AllyBuff.ApplyToAll(allies, 1.5f, 1.3f, 4f);
        Debug.Log($"[Smoke] AllyBuff: {allies.Count}마리에 dmg×1.5, spd×1.3, 4초");
    }

    [MenuItem("Tools/Synergy/Smoke — Spawn Defender Ally at (0,-2)")]
    static void SmokeDefender()
    {
        var so = ScriptableObject.CreateInstance<AllySO>();
        so.allyName = "DefenderTest";
        so.baseHP = 60f; so.moveSpeed = 2f; so.attackDamage = 10f;
        so.attackRange = 1.5f; so.attackInterval = 0.8f; so.scale = 0.4f;
        so.behaviorId = "defender";
        so.bodyColor = new Color(0.2f, 0.5f, 1f);
        var ally = AllySpawner.SpawnAt(so, new Vector2(0, -2));
        Debug.Log($"[Smoke] Defender 스폰: {ally?.name} (behavior=defender, patrolRadius=3)");
    }

    [MenuItem("Tools/Synergy/Smoke — Spawn Barrier at (0,-1)")]
    static void SmokeBarrier()
    {
        var so = ScriptableObject.CreateInstance<StructureSO>();
        so.structureName = "BarrierTest";
        so.baseHP = 150f; so.scale = 0.6f;
        so.behaviorId = "barrier";
        so.bodyColor = new Color(0.6f, 0.8f, 0.3f);
        var s = StructureSpawner.SpawnAt(so, new Vector2(0, -1));
        Debug.Log($"[Smoke] Barrier 스폰: {s?.name} (behavior=barrier, pushRadius=1.5)");
    }

    [MenuItem("Tools/Synergy/Smoke — Spawn Watchtower at (-3,-2)")]
    static void SmokeWatchtower()
    {
        var so = ScriptableObject.CreateInstance<StructureSO>();
        so.structureName = "WatchtowerTest";
        so.baseHP = 80f; so.scale = 0.5f;
        so.behaviorId = "watchtower";
        so.bodyColor = new Color(0.8f, 0.8f, 0.4f);
        var s = StructureSpawner.SpawnAt(so, new Vector2(-3, -2));
        Debug.Log($"[Smoke] Watchtower 스폰: {s?.name} (1.5s마다 가장 가까운 적 6dmg)");
    }

    [MenuItem("Tools/Synergy/Smoke — Registry counts (Enemy/Ally/Structure)")]
    static void SmokeRegistryCounts()
    {
        int e = EnemyRegistry.Instance?.GetAll().Count ?? -1;
        int a = AllyRegistry.Instance?.GetAll().Count ?? -1;
        int s = StructureRegistry.Instance?.GetAll().Count ?? -1;
        Debug.Log($"[Smoke] Registry counts: Enemy={e}, Ally={a}, Structure={s}");
    }

    [MenuItem("Tools/Synergy/Smoke — Dispatcher state")]
    static void SmokeDispatcher()
    {
        var d = SynergyDispatcher.Instance;
        if (d == null) { Debug.LogWarning("[Smoke] SynergyDispatcher.Instance == null"); return; }
        var f = d.Families;
        Debug.Log($"[Smoke] Dispatcher: seq.Count={d.Sequence.Count}, " +
            $"Fire={f.Count(SynergyFamily.Fire)}, Water={f.Count(SynergyFamily.Water)}, " +
            $"Wind={f.Count(SynergyFamily.Wind)}, Earth={f.Count(SynergyFamily.Earth)}, " +
            $"Lightning={f.Count(SynergyFamily.Lightning)}, War={f.Count(SynergyFamily.War)}, " +
            $"Dark={f.Count(SynergyFamily.Dark)}, Civ={f.Count(SynergyFamily.Civilization)}");
    }

    [MenuItem("Tools/Synergy/Smoke — Force FlightEnded (current seq)")]
    static void SmokeForceFlightEnded()
    {
        var d = SynergyDispatcher.Instance;
        if (d == null) { Debug.LogWarning("[Smoke] SynergyDispatcher.Instance == null"); return; }
        if (d.Sequence.Count == 0) { Debug.LogWarning("[Smoke] 현재 시퀀스 비어있음 — 행성 한번 hit 시뮬 후 재시도"); return; }

        // HandleFlightEnded는 private이므로 리플렉션으로 호출 (Position 시너지 매칭 수동 검증용).
        var mi = typeof(SynergyDispatcher).GetMethod("HandleFlightEnded",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (mi == null) { Debug.LogError("[Smoke] HandleFlightEnded 찾을 수 없음 — 리플렉션 실패"); return; }
        mi.Invoke(d, new object[] { d.Sequence });
        Debug.Log($"[Smoke] ForceFlightEnded 호출 (seq.Count={d.Sequence.Count})");
    }

    static Enemy GetFirstEnemy()
    {
        if (EnemyRegistry.Instance == null)
        {
            Debug.LogWarning("[Smoke] EnemyRegistry.Instance == null (Play 모드에서 CombatScene이 로드돼 있어야 함)");
            return null;
        }
        var list = EnemyRegistry.Instance.GetAll();
        if (list.Count == 0) { Debug.LogWarning("[Smoke] 살아있는 적 없음"); return null; }
        return list[0];
    }
}
#endif
