using UnityEngine;

/// <summary>
/// 구조물 데이터. Civilization/Saturn 시너지가 설치하는 정적 개체의 정의.
/// </summary>
[CreateAssetMenu(fileName = "NewStructure", menuName = "Combat/Structure")]
public class StructureSO : ScriptableObject
{
    [Header("기본 정보")]
    public string structureName;
    [TextArea] public string description;

    [Header("스탯")]
    public float baseHP = 100f;
    public float scale = 0.7f;

    [Header("행동")]
    [Tooltip("IStructureBehavior 구현체 ID")]
    public string behaviorId;

    [Header("비주얼")]
    public Color bodyColor = new Color(0.6f, 0.8f, 0.3f);
}
