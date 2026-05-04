using UnityEngine;

/// <summary>
/// 천체 공통 베이스 ScriptableObject.
/// 항성, 행성, 위성, 혜성 모두 이것을 상속한다.
/// </summary>
public abstract class CelestialBodySO : ScriptableObject, IRewardApplicable
{
    public virtual void ApplyAsReward(PlayerState player, RunState run)
    {
        if (run == null) return;
        run.AddToDeck(this);
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
    [Tooltip("중력 세기 (0이면 중력 없음). 만유인력 공식에서 중력가속도로 사용됨.")]
    public float gravityStrength = 0f;

    [Tooltip("중력 타입 ID (GravityTypeRegistry에서 매핑)")]
    public string gravityTypeId = "standard";

    [Tooltip("우주선 조우 반경 (행성 표면 반경)")]
    public float encounterRadius = 0.54f;

    [Tooltip("중력장 반지름 비율. 중력 영향 범위 = encounterRadius × 이 값")]
    [Range(1f, 20f)]
    public float gravityFieldRangeRatio = 4f;

[Header("자전")]
    [Tooltip("자전 속도 (도/초). 투사체 착지 시 표면 회전 속도. 음수=시계 방향.")]
    [Range(-1800f, 1800f)]
    public float spinSpeed = 1350f;
}
