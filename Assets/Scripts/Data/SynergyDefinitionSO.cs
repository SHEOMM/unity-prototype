using UnityEngine;

/// <summary>
/// 키워드 시저지 정의. 슬래시 내 동일 키워드가 N개 이상 모이면 발동.
/// </summary>
[CreateAssetMenu(fileName = "NewSynergy", menuName = "Celestial/Synergy")]
public class SynergyDefinitionSO : ScriptableObject
{
    public string synergyName;
    [TextArea] public string description;
    public string requiredKeyword;
    public int requiredCount = 2;

    [Header("보너스")]
    public float damageMultiplier = 1.5f;
    public Element bonusElement = Element.None;
}
