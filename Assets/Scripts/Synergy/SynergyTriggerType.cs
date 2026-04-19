/// <summary>
/// 시너지 발동 조건 분류.
/// 각 타입은 SynergyRuleMatcher가 해석해 bool로 판정한다.
/// </summary>
public enum SynergyTriggerType
{
    /// <summary>지정한 family가 threshold 이상 누적되면 발동.</summary>
    FamilyAccumulation,

    /// <summary>시퀀스의 특정 위치(맨 앞/뒤)에 지정 행성이 있으면 발동. (Phase 4에서 Jupiter 등이 사용 예정)</summary>
    SequencePosition,

    /// <summary>시퀀스에 지정한 키워드/행성명 조합이 모두 포함되면 발동. (Phase 5에서 별자리 조합)</summary>
    PlanetCombo
}
