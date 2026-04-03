using UnityEngine;

/// <summary>
/// CombatScene 진입 시 자동 초기화. 전투 시스템을 설정하고 전투를 시작.
/// </summary>
public class CombatSceneBoot : MonoBehaviour
{
    void Start()
    {
        // 카메라 설정
        var cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(0, 1, -10);
            cam.orthographicSize = 6;
        }

        // CombatManager를 이 씬에 생성
        var combatGo = new GameObject("CombatManager");
        var combat = combatGo.AddComponent<CombatManager>();
        combat.synergies = GameManager.Instance?.Synergies;
        combat.cometPool = GameManager.Instance?.CometPool;
        combat.Initialize();

        // 전투 완료 시 → Reward 페이즈
        combat.OnCombatComplete += () => GameManager.Instance?.EnterPhase(GamePhase.Reward);

        // 전투 시작
        combat.StartCombat(
            null,
            RunState.Instance?.starDeck?.ToArray(),
            RunState.Instance?.planetDeck?.ToArray()
        );

        // UI
        combatGo.AddComponent<PlayerHPBar>();
        combatGo.AddComponent<PlayerDamageView>();
    }
}
