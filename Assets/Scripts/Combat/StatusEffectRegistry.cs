using System;
using System.Collections.Generic;
using System.Reflection;

public static class StatusEffectRegistry
{
    private static Dictionary<string, Type> _reg;

    public static void EnsureInitialized()
    {
        if (_reg != null) return;
        _reg = new Dictionary<string, Type>();
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in asm.GetTypes())
            {
                var attr = type.GetCustomAttribute<StatusEffectIdAttribute>();
                if (attr != null && typeof(IStatusEffect).IsAssignableFrom(type))
                    _reg[attr.Id] = type;
            }
    }

    public static IStatusEffect Get(string id)
    {
        EnsureInitialized();
        if (!string.IsNullOrEmpty(id) && _reg.TryGetValue(id, out var type))
            return (IStatusEffect)Activator.CreateInstance(type);
        return null;
    }
}
