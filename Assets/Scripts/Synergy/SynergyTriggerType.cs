/// <summary>
/// 시너지 발동 조건 분류.
/// 각 타입은 SynergyRuleMatcher가 해석해 bool로 판정한다.
/// </summary>
public enum SynergyTriggerType
{
    /// <summary>지정한 family가 threshold 이상 누적되면 발동.</summary>
    FamilyAccumulation,

    /// <summary>시퀀스의 특정 위치(맨 앞/뒤/어디든)에 지정 행성이 있으면 end-of-flight 발동.</summary>
    SequencePosition,

    /// <summary>시퀀스에 지정한 키워드/행성명 조합이 모두 포함되면 end-of-flight 발동 (별자리 조합).</summary>
    PlanetCombo,

    /// <summary>비행 중 특정 키워드/행성명 행성을 터치하는 순간 OnHit 발화 (per-hit).</summary>
    PerHitPlanet
}
