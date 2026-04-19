using UnityEngine;

/// <summary>
/// CombatScene 진입 시 자동 초기화. 전투 시스템을 설정하고 전투를 시작.
/// Phase 9b: floor × roomType 기반 난이도 승수 계산 + EnemySpawner에 주입.
/// </summary>
public class CombatSceneBoot : SceneBootBase
{
    const float FloorGrowth = 0.15f;      // 층당 +15% 적 수
    const float EliteMultiplier = 1.5f;
    const float BossMultiplier = 2.0f;

    protected override void OnBoot()
    {
        var gm = GameManager.Instance;
        if (gm == null) { Debug.LogError("[CombatScene] GameManager not found"); return; }

        var combatGo = new GameObject("CombatManager");
        var combat = combatGo.AddComponent<CombatManager>();
        combat.synergies = gm.Synergies;
        combat.synergyRules = gm.SynergyRules;
        combat.cometPool = gm.CometPool;
        combat.Initialize();

        combat.OnCombatComplete += () => gm.EnterPhase(GamePhase.Reward);

        WaveDefinitionSO[] waves = gm.DefaultWaves;
        float difficulty = ComputeDifficulty(RunState.Instance?.currentFloor ?? 0, gm.CurrentNode?.roomType);

        combat.StartCombat(
            waves,
            RunState.Instance?.unlockedOrbits,
            RunState.Instance?.orbitAssignments,
            RunState.Instance?.planetDeck,
            difficulty
        );

        Debug.Log($"[CombatScene] 전투 시작 — 웨이브 {(waves != null ? waves.Length : 0)}개, 난이도 ×{difficulty:F2}");

        combatGo.AddComponent<PlayerHPBar>();
        combatGo.AddComponent<PlayerDamageView>();
        combatGo.AddComponent<SynergyToastView>().Bind(combat.SynergyDispatcher);
        combatGo.AddComponent<SynergyVisualHost>().Bind(combat.SynergyDispatcher);
    }

    static float ComputeDifficulty(int floor, RoomType? roomType)
    {
        float mult = 1f + floor * FloorGrowth;
        if (roomType == RoomType.Elite) mult *= EliteMultiplier;
        else if (roomType == RoomType.Boss) mult *= BossMultiplier;
        return mult;
    }
}
