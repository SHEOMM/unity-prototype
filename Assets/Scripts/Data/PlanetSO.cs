using UnityEngine;

/// <summary>
/// 행성(Planet) = 카드. 궤도에 배치되어 공전하며, 슬래시 상호작용 효과를 가진다.
/// effectId로 IStarEffect 구현체를 찾아 실행한다.
/// </summary>
[CreateAssetMenu(fileName = "NewPlanet", menuName = "Celestial/Planet")]
public class PlanetSO : CelestialBodySO
{
    [Header("행성 고유")]
    [Tooltip("효과 구현체 ID (EffectRegistry에서 매핑)")]
    public string effectId;

    [Tooltip("기본 데미지")]
    public float baseDamage = 10f;

    [Tooltip("시각적 크기")]
    public float visualScale = 0.35f;

    [Tooltip("시너지 계열 — 발사체 비행 중 누적되어 시너지 발동 기준이 됨. Element(데미지 속성)와 별개.")]
    public SynergyFamily synergyFamily = SynergyFamily.Fire;

    [Header("위상(Phase) 조건")]
    [Tooltip("위상 발동에 필요한 최소 뒤쪽 별 개수")]
    public int phaseThreshold = 3;

    [Tooltip("위상 효과 ID (없으면 위상 없음)")]
    public string phaseEffectId;
}
