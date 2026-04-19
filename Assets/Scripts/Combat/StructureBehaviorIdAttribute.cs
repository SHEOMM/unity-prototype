using System;

/// <summary>
/// IStructureBehavior 구현체 식별자.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StructureBehaviorIdAttribute : Attribute
{
    public string Id { get; }
    public StructureBehaviorIdAttribute(string id) { Id = id; }
}
