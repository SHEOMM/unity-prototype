using System;
using System.Collections.Generic;
using System.Reflection;

public static class GravityTypeRegistry
{
    private static Dictionary<string, Type> _reg;
    private static readonly StandardGravity _fallback = new StandardGravity();

    public static void EnsureInitialized()
    {
        if (_reg != null) return;
        _reg = new Dictionary<string, Type>();
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in asm.GetTypes())
            {
                var attr = type.GetCustomAttribute<GravityTypeIdAttribute>();
                if (attr != null && typeof(IGravityType).IsAssignableFrom(type))
                    _reg[attr.Id] = type;
            }
    }

    public static IGravityType Get(string id)
    {
        EnsureInitialized();
        if (!string.IsNullOrEmpty(id) && _reg.TryGetValue(id, out var type))
            return (IGravityType)Activator.CreateInstance(type);
        return _fallback;
    }
}
