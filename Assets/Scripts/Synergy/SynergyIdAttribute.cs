using System;

/// <summary>
/// 시너지 효과 클래스에 부여하는 식별자.
/// SynergyRegistry가 리플렉션으로 수집, 문자열 ID로 인스턴스 획득.
/// 예: [SynergyId("fire_accum_3")] public class InfernoSynergy : SynergyEffectBase { ... }
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SynergyIdAttribute : Attribute
{
    public string Id { get; }
    public SynergyIdAttribute(string id) { Id = id; }
}
