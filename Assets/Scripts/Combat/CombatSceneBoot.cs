using UnityEngine;

/// <summary>
/// CombatScene 진입 시 자동 초기화. 전투 시스템을 설정하고 전투를 시작.
/// </summary>
public class CombatSceneBoot : MonoBehaviour
{
    void Start()
    {
        var gm = GameManager.Instance;
        if (gm == null) { Debug.LogError("[CombatScene] GameManager not found"); return; }

        // CombatManager를 이 씬에 생성
        var combatGo = new GameObject("CombatManager");
        var combat = combatGo.AddComponent<CombatManager>();
        combat.synergies = gm.Synergies;
        combat.cometPool = gm.CometPool;
        combat.Initialize();

        // 전투 완료 시 → Reward 페이즈
        combat.OnCombatComplete += () => gm.EnterPhase(GamePhase.Reward);

        // 웨이브 결정
        WaveDefinitionSO[] waves = gm.DefaultWaves;

        // 전투 시작
        combat.StartCombat(
            waves,
            RunState.Instance?.starDeck?.ToArray(),
            RunState.Instance?.planetDeck?.ToArray()
        );

        Debug.Log($"[CombatScene] 전투 시작 — 웨이브 {(waves != null ? waves.Length : 0)}개");

        // UI
        combatGo.AddComponent<PlayerHPBar>();
        combatGo.AddComponent<PlayerDamageView>();
    }
}
