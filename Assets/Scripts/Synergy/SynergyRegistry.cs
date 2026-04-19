using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// [SynergyId] 어트리뷰트 붙은 ISynergyEffect 구현체를 리플렉션으로 수집.
/// SynergyRuleSO의 synergyEffectId 문자열로 인스턴스 획득.
/// 패턴은 EffectRegistry/VisualRegistry/EnemyBehaviorRegistry와 동일.
/// </summary>
public static class SynergyRegistry
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
                var attr = type.GetCustomAttribute<SynergyIdAttribute>();
                if (attr != null && typeof(ISynergyEffect).IsAssignableFrom(type))
                    _reg[attr.Id] = type;
            }
        }
    }

    public static ISynergyEffect Get(string id)
    {
        EnsureInitialized();
        if (string.IsNullOrEmpty(id)) return null;
        if (_reg.TryGetValue(id, out var type))
            return (ISynergyEffect)Activator.CreateInstance(type);
        return null;
    }

    public static bool Has(string id)
    {
        EnsureInitialized();
        return !string.IsNullOrEmpty(id) && _reg.ContainsKey(id);
    }
}
