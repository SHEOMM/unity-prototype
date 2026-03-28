using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// ISpellVisual 구현체를 visualId로 조회하는 레지스트리.
/// EffectRegistry와 동일한 리플렉션 자동 등록 패턴.
/// </summary>
public static class VisualRegistry
{
    private static Dictionary<string, Type> _reg;
    private static readonly DefaultVisual _fallback = new DefaultVisual();

    public static void EnsureInitialized()
    {
        if (_reg != null) return;
        _reg = new Dictionary<string, Type>();

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in asm.GetTypes())
            {
                var attr = type.GetCustomAttribute<VisualIdAttribute>();
                if (attr != null && typeof(ISpellVisual).IsAssignableFrom(type))
                    _reg[attr.Id] = type;
            }
        }
    }

    public static ISpellVisual Get(string id)
    {
        EnsureInitialized();
        if (!string.IsNullOrEmpty(id) && _reg.TryGetValue(id, out var type))
            return (ISpellVisual)Activator.CreateInstance(type);
        return _fallback;
    }
}
