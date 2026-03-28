using System;
using System.Collections.Generic;
using System.Reflection;

public static class EnemyBehaviorRegistry
{
    private static Dictionary<string, Type> _reg;

    public static void EnsureInitialized()
    {
        if (_reg != null) return;
        _reg = new Dictionary<string, Type>();

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in asm.GetTypes())
            {
                var attr = type.GetCustomAttribute<EnemyBehaviorIdAttribute>();
                if (attr != null && typeof(IEnemyBehavior).IsAssignableFrom(type))
                    _reg[attr.Id] = type;
            }
    }

    public static IEnemyBehavior Get(string id)
    {
        EnsureInitialized();
        if (!string.IsNullOrEmpty(id) && _reg.TryGetValue(id, out var type))
            return (IEnemyBehavior)Activator.CreateInstance(type);
        return new GruntBehavior();
    }

    public static Type GetBehaviorType(string id)
    {
        EnsureInitialized();
        if (!string.IsNullOrEmpty(id) && _reg.TryGetValue(id, out var type))
            return type;
        return null;
    }
}
