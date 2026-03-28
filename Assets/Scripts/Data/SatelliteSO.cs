using UnityEngine;

/// <summary>
/// 위성(Satellite). 행성에 장착하여 패시브 버프를 제공한다.
/// </summary>
[CreateAssetMenu(fileName = "NewSatellite", menuName = "Celestial/Satellite")]
public class SatelliteSO : CelestialBodySO
{
    [Header("위성 패시브")]
    public PassiveType passiveType;
    public float passiveValue = 1f;

    [Tooltip("위성의 시각적 크기")]
    public float visualScale = 0.15f;

    [Tooltip("행성 주위 공전 반지름")]
    public float orbitRadius = 0.3f;

    [Tooltip("행성 주위 공전 속도")]
    public float orbitSpeed = 90f;
}

public enum PassiveType
{
    SpeedBoost,         // 공전 속도 증가
    DamageBoost,        // 공격력 강화
    ElementalAffinity,  // 속성 강화
    CooldownReduction,  // 쿨다운 감소
    ExtraHit,           // 추가 타격
    AreaExpansion        // 범위 확대
}
