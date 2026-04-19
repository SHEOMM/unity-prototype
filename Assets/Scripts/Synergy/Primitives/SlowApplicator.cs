/// <summary>
/// 이동속도 감소. Enemy.Update에서 moveSpeed를 _baseSpeed로 리셋한 뒤 StatusEffect가
/// 매 프레임 slowFactor를 곱하므로, 상태 지속 중엔 감속 유지, 만료 시 자동 복원된다.
/// </summary>
public static class SlowApplicator
{
    public static void Apply(IStatusHost target, float slowFactor, float duration)
    {
        if (target == null) return;
        var slow = new SlowEffect { slowFactor = slowFactor };
        var status = new StatusEffect(slow, duration);
        target.ApplyStatus(status);
    }
}
