using System;

[AttributeUsage(AttributeTargets.Class)]
public class RelicEffectIdAttribute : Attribute
{
    public string Id { get; }
    public RelicEffectIdAttribute(string id) { Id = id; }
}
