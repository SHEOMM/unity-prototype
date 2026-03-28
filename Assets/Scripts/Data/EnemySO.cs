using UnityEngine;

/// <summary>
/// 적 데이터 정의. 행성의 PlanetSO에 대응.
/// </summary>
[CreateAssetMenu(fileName = "NewEnemy", menuName = "Combat/Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName;
    [TextArea] public string description;

    [Header("스탯")]
    public float baseHP = 100f;
    public float moveSpeed = 1f;
    public float scale = 0.6f;
    public float attackDamage = 10f;

    [Header("속성 상호작용")]
    public Element element = Element.None;
    public ElementResistance[] resistances;

    [Header("행동")]
    [Tooltip("IEnemyBehavior 구현체 ID")]
    public string behaviorId;

    [Header("비주얼")]
    public Color bodyColor = new Color(0.8f, 0.2f, 0.2f);
    public EnemyShape shape = EnemyShape.Square;

    [Header("보상")]
    public int scoreValue = 10;
}
