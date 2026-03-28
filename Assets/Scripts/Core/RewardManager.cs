using UnityEngine;

/// <summary>
/// 전투 후 보상 선택을 관리한다.
/// 프로토타입: 자동으로 첫 번째 보상을 적용. TODO: 선택 UI 구현.
/// </summary>
public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }

    public System.Action OnRewardChosen;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void ShowRewards(ScriptableObject[] pool)
    {
        if (pool == null || pool.Length == 0)
        {
            Debug.Log("[보상] 보상 풀이 비어있음. 스킵.");
            OnRewardChosen?.Invoke();
            return;
        }

        // 프로토타입: 랜덤 1개 자동 선택
        var reward = pool[Random.Range(0, pool.Length)];
        Debug.Log($"[보상] {reward.name} 획득!");

        if (reward is IRewardApplicable applicable)
            applicable.ApplyAsReward(PlayerState.Instance, RunState.Instance);

        OnRewardChosen?.Invoke();
    }

    public void ApplyReward(RewardOption reward)
    {
        switch (reward.type)
        {
            case CometRewardType.DamageBoost:
                if (PlayerState.Instance != null)
                    PlayerState.Instance.bonusDamageMultiplier += reward.value;
                break;
            case CometRewardType.ExtraSlashWidth:
                var detector = GetComponent<SlashDetector>();
                if (detector != null) detector.slashWidth += reward.value;
                break;
            case CometRewardType.HealPlayer:
                PlayerState.Instance?.Heal(reward.value);
                break;
            case CometRewardType.GrantRelic:
                if (reward.relicSO != null)
                    PlayerState.Instance?.AddRelic(reward.relicSO);
                break;
        }
    }
}
