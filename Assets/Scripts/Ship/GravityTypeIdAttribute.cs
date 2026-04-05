using System;

[AttributeUsage(AttributeTargets.Class)]
public class GravityTypeIdAttribute : Attribute
{
    public string Id { get; }
    public GravityTypeIdAttribute(string id) { Id = id; }
}
