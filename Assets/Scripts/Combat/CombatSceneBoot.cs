using UnityEngine;

/// <summary>
/// CombatScene 진입 시 자동 초기화. 전투 시스템을 설정하고 전투를 시작.
/// 활성 씬 설정 + 환경 적용은 SceneBootBase가 처리.
/// </summary>
public class CombatSceneBoot : SceneBootBase
{
    protected override void OnBoot()
    {
        var gm = GameManager.Instance;
        if (gm == null) { Debug.LogError("[CombatScene] GameManager not found"); return; }

        var combatGo = new GameObject("CombatManager");
        var combat = combatGo.AddComponent<CombatManager>();
        combat.synergies = gm.Synergies;
        combat.cometPool = gm.CometPool;
        combat.Initialize();

        combat.OnCombatComplete += () => gm.EnterPhase(GamePhase.Reward);

        WaveDefinitionSO[] waves = gm.DefaultWaves;

        combat.StartCombat(
            waves,
            RunState.Instance?.starDeck?.ToArray(),
            RunState.Instance?.planetDeck?.ToArray()
        );

        Debug.Log($"[CombatScene] 전투 시작 — 웨이브 {(waves != null ? waves.Length : 0)}개");

        combatGo.AddComponent<PlayerHPBar>();
        combatGo.AddComponent<PlayerDamageView>();
    }
}
