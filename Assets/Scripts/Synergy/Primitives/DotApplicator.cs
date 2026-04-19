/// <summary>
/// Damage-over-time 부착. 지정 속성/데미지/인터벌/지속시간으로 적에게 StatusEffect 적용.
/// 독, 화상, 방사 등 공통 DoT에 사용.
/// </summary>
public static class DotApplicator
{
    public static void Apply(Enemy target, float damagePerTick, float tickInterval, float duration, Element element = Element.None)
    {
        if (target == null) return;
        var dot = new GenericDotEffect(damagePerTick, tickInterval, element);
        var status = new StatusEffect(dot, duration);
        target.ApplyStatus(status);
    }
}
