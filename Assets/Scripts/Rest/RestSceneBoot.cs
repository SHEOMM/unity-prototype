using UnityEngine;

/// <summary>
/// RestScene 진입 시 자동 초기화. 체력 회복 후 맵으로 복귀.
/// </summary>
public class RestSceneBoot : SceneBootBase
{
    protected override void OnBoot()
    {
        EnsureHud();
        if (PlayerState.Instance == null) return;

        float healAmount = PlayerState.Instance.maxHP * 0.3f;
        PlayerState.Instance.Heal(healAmount);
        Debug.Log($"[휴식] HP {Mathf.CeilToInt(healAmount)} 회복");

        Invoke(nameof(ReturnToMap), 1f);
    }

    void ReturnToMap()
    {
        GameManager.Instance?.EnterPhase(GamePhase.Map);
    }
}
