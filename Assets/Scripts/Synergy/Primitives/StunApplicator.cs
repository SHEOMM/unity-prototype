/// <summary>
/// 기절 부착. IStatusHost에 StunEffect를 지정 duration만큼 적용.
/// 번개 tier2 (낙뢰) 시너지 등이 사용.
/// </summary>
public static class StunApplicator
{
    public static void Apply(IStatusHost target, float duration)
    {
        if (target == null) return;
        var effect = new StunEffect();
        var status = new StatusEffect(effect, duration);
        target.ApplyStatus(status);
    }
}
