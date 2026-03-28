using System;

[AttributeUsage(AttributeTargets.Class)]
public class StatusEffectIdAttribute : Attribute
{
    public string Id { get; }
    public StatusEffectIdAttribute(string id) { Id = id; }
}
