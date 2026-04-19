/// <summary>
/// 시너지 분류. 천체가 속한 시너지 계열.
/// Element(데미지 속성)와는 별개 축 — 한 행성은 Fire 계열 시너지에 기여하면서
/// Darkness 속성 데미지를 낼 수도 있음.
/// 각 계열은 FamilyAccumulator에서 독립 카운트되며, 누적 시너지 발동 기준.
/// </summary>
public enum SynergyFamily
{
    Fire,
    Water,
    Wind,
    Earth,
    Lightning,
    War,
    Dark,
    Civilization
}
