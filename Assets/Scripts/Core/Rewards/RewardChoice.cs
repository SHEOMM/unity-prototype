using UnityEngine;

/// <summary>
/// 보상 선택지 하나. RewardManager가 풀에서 뽑아 RewardSceneBoot에 전달,
/// RewardCardView가 이것으로 카드를 렌더.
/// 다형성을 위해 payload는 IRewardApplicable로 유지.
/// </summary>
public class RewardChoice
{
    public IRewardApplicable Payload;
    public string DisplayName;
    public string Description;
    public Sprite Icon;
    public string TypeLabel;   // "궤도" / "행성" / "유물"
    public Color TypeColor;    // 카드 뱃지 색상
}
