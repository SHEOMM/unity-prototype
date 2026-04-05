using UnityEngine;

/// <summary>
/// 휴식 방 로직. HP 30% 회복 후 맵으로 복귀.
/// Core만 참조 — Combat/Shop/Map을 모른다.
/// </summary>
public class RestManager : MonoBehaviour
{
    public static RestManager Instance { get; private set; }

    public System.Action OnRestComplete;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void StartRest()
    {
        if (PlayerState.Instance == null) return;

        float healAmount = PlayerState.Instance.maxHP * 0.3f;
        PlayerState.Instance.Heal(healAmount);
        Debug.Log($"[휴식] HP {Mathf.CeilToInt(healAmount)} 회복 → {Mathf.CeilToInt(PlayerState.Instance.currentHP)}/{Mathf.CeilToInt(PlayerState.Instance.maxHP)}");

        // 프로토타입: 즉시 완료. TODO: 휴식/업그레이드 선택 UI
        OnRestComplete?.Invoke();
    }
}
