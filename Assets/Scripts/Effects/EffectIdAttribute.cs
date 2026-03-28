using System;

[AttributeUsage(AttributeTargets.Class)]
public class EffectIdAttribute : Attribute
{
    public string Id { get; }
    public EffectIdAttribute(string id) { Id = id; }
}
