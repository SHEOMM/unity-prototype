using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// ISynergyVisual을 visualId로 조회. VisualRegistry와 동일 패턴 (리플렉션 자동 수집).
/// 인스턴스는 stateless라고 가정하고 매번 new (빈 생성자). 수명은 coroutine 동안만.
/// </summary>
public static class SynergyVisualRegistry
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
                var attr = type.GetCustomAttribute<SynergyVisualIdAttribute>();
                if (attr != null && typeof(ISynergyVisual).IsAssignableFrom(type))
                    _reg[attr.Id] = type;
            }
        }
    }

    public static ISynergyVisual Get(string id)
    {
        EnsureInitialized();
        if (!string.IsNullOrEmpty(id) && _reg.TryGetValue(id, out var type))
            return (ISynergyVisual)Activator.CreateInstance(type);
        return null;
    }
}
