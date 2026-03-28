using UnityEngine;

/// <summary>
/// 혜성 데이터. 이벤트성으로 등장하며 포착 시 선택지 보상을 준다.
/// </summary>
[CreateAssetMenu(fileName = "NewComet", menuName = "Celestial/Comet")]
public class CometSO : CelestialBodySO
{
    [Header("혜성 고유")]
    public float flySpeed = 3f;
    public float visualScale = 0.25f;

    [Header("보상 선택지")]
    public RewardOption[] rewards;
}

[System.Serializable]
public class RewardOption
{
    public string rewardName;
    [TextArea] public string description;
    public CometRewardType type;
    public float value;
    public RelicSO relicSO;
}

public enum CometRewardType
{
    DamageBoost,
    SpeedBoost,
    ExtraSlashWidth,
    HealPlayer,
    GrantSatellite,
    GrantRelic
}
