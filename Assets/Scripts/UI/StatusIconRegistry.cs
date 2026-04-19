using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// [StatusIconId] 붙은 IStatusIconMeta 구현체를 리플렉션으로 수집.
/// SynergyRegistry / StatusEffectRegistry와 동일 패턴.
/// Registry는 싱글턴 인스턴스 캐시 (meta는 stateless라 재사용 안전).
/// </summary>
public static class StatusIconRegistry
{
    private static Dictionary<string, IStatusIconMeta> _reg;

    public static void EnsureInitialized()
    {
        if (_reg != null) return;
        _reg = new Dictionary<string, IStatusIconMeta>();
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try { types = asm.GetTypes(); }
            catch (ReflectionTypeLoadException e) { types = e.Types; }
            foreach (var type in types)
            {
                if (type == null) continue;
                var attr = type.GetCustomAttribute<StatusIconIdAttribute>();
                if (attr == null) continue;
                if (!typeof(IStatusIconMeta).IsAssignableFrom(type)) continue;
                try { _reg[attr.Id] = (IStatusIconMeta)Activator.CreateInstance(type); }
                catch { /* 생성 실패한 meta는 무시 */ }
            }
        }
    }

    public static IStatusIconMeta Get(string id)
    {
        EnsureInitialized();
        if (string.IsNullOrEmpty(id)) return null;
        return _reg.TryGetValue(id, out var meta) ? meta : null;
    }
}
