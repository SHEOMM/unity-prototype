using UnityEngine;

/// <summary>
/// RestScene 진입 시 자동 초기화. 체력 회복 후 맵으로 복귀.
/// </summary>
public class RestSceneBoot : MonoBehaviour
{
    void Start()
    {
        if (PlayerState.Instance == null) return;

        float healAmount = PlayerState.Instance.maxHP * 0.3f;
        PlayerState.Instance.Heal(healAmount);
        Debug.Log($"[휴식] HP {Mathf.CeilToInt(healAmount)} 회복");

        // 잠시 후 맵으로 복귀
        Invoke(nameof(ReturnToMap), 1f);
    }

    void ReturnToMap()
    {
        GameManager.Instance?.EnterPhase(GamePhase.Map);
    }
}
