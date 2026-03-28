using System;
using System.Collections.Generic;
using System.Reflection;

public static class EffectRegistry
{
    private static Dictionary<string, Type> _reg;

    public static void EnsureInitialized()
    {
        if (_reg != null) return;
        _reg = new Dictionary<string, Type>();

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in asm.GetTypes())
            {
                var attr = type.GetCustomAttribute<EffectIdAttribute>();
                if (attr != null && typeof(IStarEffect).IsAssignableFrom(type))
                    _reg[attr.Id] = type;
            }
        }
    }

    public static IStarEffect Get(string id)
    {
        EnsureInitialized();
        if (!string.IsNullOrEmpty(id) && _reg.TryGetValue(id, out var type))
            return (IStarEffect)Activator.CreateInstance(type);
        return new GenericDamageEffect();
    }

    /// <summary>effectId에 대응하는 IStarEffect 구현 타입을 반환한다. 없으면 null.</summary>
    public static Type GetEffectType(string id)
    {
        EnsureInitialized();
        if (!string.IsNullOrEmpty(id) && _reg.TryGetValue(id, out var type))
            return type;
        return null;
    }
}
