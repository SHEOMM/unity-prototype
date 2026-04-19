using UnityEngine;

/// <summary>
/// RewardScene 진입 시 자동 초기화. 보상 선택 후 맵으로 복귀.
/// </summary>
public class RewardSceneBoot : SceneBootBase
{
    protected override void OnBoot()
    {
        var reward = gameObject.AddComponent<RewardManager>();
        reward.OnRewardChosen += () => GameManager.Instance?.EnterPhase(GamePhase.Map);

        reward.ShowRewards(null);
    }
}
