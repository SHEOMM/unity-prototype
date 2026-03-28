using System;

[AttributeUsage(AttributeTargets.Class)]
public class EnemyBehaviorIdAttribute : Attribute
{
    public string Id { get; }
    public EnemyBehaviorIdAttribute(string id) { Id = id; }
}
