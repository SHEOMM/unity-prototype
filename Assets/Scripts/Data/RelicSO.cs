using UnityEngine;

[CreateAssetMenu(fileName = "NewRelic", menuName = "Data/Relic")]
public class RelicSO : ScriptableObject
{
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
