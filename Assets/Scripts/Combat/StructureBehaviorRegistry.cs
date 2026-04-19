using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// [StructureBehaviorId] 붙은 IStructureBehavior 구현체 리플렉션 등록.
/// </summary>
public static class StructureBehaviorRegistry
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
                var attr = type.GetCustomAttribute<StructureBehaviorIdAttribute>();
                if (attr != null && typeof(IStructureBehavior).IsAssignableFrom(type))
                    _reg[attr.Id] = type;
            }
        }
    }

    public static IStructureBehavior Get(string id)
    {
        EnsureInitialized();
        if (string.IsNullOrEmpty(id)) return null;
        if (_reg.TryGetValue(id, out var type))
            return (IStructureBehavior)Activator.CreateInstance(type);
        return null;
    }
}
