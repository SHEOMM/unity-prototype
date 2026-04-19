using UnityEngine;

/// <summary>
/// 아군 유닛 데이터. War/Mars 시너지가 소환하는 유닛의 정의.
/// </summary>
[CreateAssetMenu(fileName = "NewAlly", menuName = "Combat/Ally")]
public class AllySO : ScriptableObject
{
    [Header("기본 정보")]
    public string allyName;
    [TextArea] public string description;

    [Header("스탯")]
    public float baseHP = 50f;
    public float moveSpeed = 1.5f;
    public float scale = 0.5f;
    public float attackDamage = 8f;
    public float attackRange = 1.5f;
    public float attackInterval = 0.8f;

    [Header("행동")]
    [Tooltip("IAllyBehavior 구현체 ID")]
    public string behaviorId = "melee";

    [Header("비주얼")]
    public Color bodyColor = new Color(0.3f, 0.8f, 1f);
}
