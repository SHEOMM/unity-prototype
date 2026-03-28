using UnityEngine;

[CreateAssetMenu(fileName = "NewRelic", menuName = "Data/Relic")]
public class RelicSO : ScriptableObject, IRewardApplicable
{
    public void ApplyAsReward(PlayerState player, RunState run)
    {
        player?.AddRelic(this);
    }

    public string relicName;
    [TextArea] public string description;
    public Sprite icon;
    public RelicRarity rarity;
    [Tooltip("IRelicEffect 구현체 ID")]
    public string effectId;
}

public enum RelicRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}
