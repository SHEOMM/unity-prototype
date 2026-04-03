using UnityEngine;

/// <summary>
/// 천체 공통 베이스 ScriptableObject.
/// 항성, 행성, 위성, 혜성 모두 이것을 상속한다.
/// </summary>
public abstract class CelestialBodySO : ScriptableObject, IRewardApplicable
{
    public virtual void ApplyAsReward(PlayerState player, RunState run)
    {
        run?.AddToDeck(this);
    }

    [Header("기본 정보")]
    public string bodyName;
    [TextArea] public string description;
    public Sprite icon;
    public Color bodyColor = Color.white;

    [Header("속성")]
    public Element element = Element.None;

    [Header("키워드 (시저지용)")]
    public string[] keywords;

    [Header("중력")]
    [Tooltip("중력 세기 (0이면 중력 없음)")]
    public float gravityStrength = 0f;

    [Tooltip("중력 타입 ID (GravityTypeRegistry에서 매핑)")]
    public string gravityTypeId = "standard";

    [Tooltip("우주선 조우 반경")]
    public float encounterRadius = 0.8f;
}
