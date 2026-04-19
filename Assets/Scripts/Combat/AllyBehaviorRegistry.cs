using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// [AllyBehaviorId] 붙은 IAllyBehavior 구현체 리플렉션 등록.
/// EnemyBehaviorRegistry와 동일한 패턴.
/// </summary>
public static class AllyBehaviorRegistry
{
    private static Dictionary<string, Type> _reg;

    public static void EnsureInitialized()
    {
        if (_reg != null) return;
        _reg = new Dictionary<string, Type>();
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try { types = asm.GetTypes(); }
            catch (ReflectionTypeLoadException e) { types = e.Types; }
            foreach (var type in types)
            {
                if (type == null) continue;
                var attr = type.GetCustomAttribute<AllyBehaviorIdAttribute>();
                if (attr != null && typeof(IAllyBehavior).IsAssignableFrom(type))
                    _reg[attr.Id] = type;
            }
        }
    }

    public static IAllyBehavior Get(string id)
    {
        EnsureInitialized();
        if (string.IsNullOrEmpty(id)) return null;
        if (_reg.TryGetValue(id, out var type))
            return (IAllyBehavior)Activator.CreateInstance(type);
        return null;
    }

    public static Type GetBehaviorType(string id)
    {
        EnsureInitialized();
        if (string.IsNullOrEmpty(id)) return null;
        return _reg.TryGetValue(id, out var type) ? type : null;
    }
}
