#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Phase 0–1 primitive 스모크 테스트. Play 모드에서 적이 살아있는 상태에서 메뉴로 실행.
/// Phase 2 진입 시 삭제 예정.
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
        Debug.Log($"[Smoke] Knockback: currentKnockback={e.currentKnockback}");
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
