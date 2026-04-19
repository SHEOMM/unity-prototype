/// <summary>
/// 약화 부착. IStatusHost에 WeaknessEffect(받는 피해 amplifier 배율)를 적용.
/// 어둠 tier3 (역병) 시너지 등이 사용.
/// </summary>
public static class WeaknessApplicator
{
    public static void Apply(IStatusHost target, float amplifier, float duration)
    {
        if (target == null) return;
        var effect = new WeaknessEffect { amplifier = amplifier };
        var status = new StatusEffect(effect, duration);
        target.ApplyStatus(status);
    }
}
