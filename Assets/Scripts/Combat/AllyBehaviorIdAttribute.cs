using System;

/// <summary>
/// IAllyBehavior 구현체 식별자. AllyBehaviorRegistry가 리플렉션으로 수집.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AllyBehaviorIdAttribute : Attribute
{
    public string Id { get; }
    public AllyBehaviorIdAttribute(string id) { Id = id; }
}
