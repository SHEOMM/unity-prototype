using UnityEngine;

/// <summary>
/// RewardScene 진입 시 자동 초기화. 보상 선택 후 맵으로 복귀.
/// </summary>
public class RewardSceneBoot : MonoBehaviour
{
    void Start()
    {
        var reward = gameObject.AddComponent<RewardManager>();
        reward.OnRewardChosen += () => GameManager.Instance?.EnterPhase(GamePhase.Map);

        // 프로토타입: 즉시 보상 적용
        reward.ShowRewards(null);
    }
}
