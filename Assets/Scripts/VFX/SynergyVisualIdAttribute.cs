using System;

/// <summary>
/// ISynergyVisual 구현체 식별자. SynergyVisualRegistry가 리플렉션으로 수집.
/// 예: [SynergyVisualId("area_pulse")] class AreaPulseVisual : ISynergyVisual { ... }
/// SynergyRuleSO.visualId로 참조.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SynergyVisualIdAttribute : Attribute
{
    public string Id { get; }
    public SynergyVisualIdAttribute(string id) { Id = id; }
}
