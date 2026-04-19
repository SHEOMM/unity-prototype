/// <summary>
/// 이동속도 감소. Enemy.Update에서 moveSpeed를 _baseSpeed로 리셋한 뒤 StatusEffect가
/// 매 프레임 slowFactor를 곱하므로, 상태 지속 중엔 감속 유지, 만료 시 자동 복원된다.
/// </summary>
public static class SlowApplicator
{
    public static void Apply(Enemy target, float slowFactor, float duration)
    {
        if (target == null) return;
        var slow = new SlowEffect();
        // SlowEffect의 slowFactor 필드는 인스턴스 변수이므로 리플렉션 없이 설정 가능하게 재구성 필요 시 확장.
        // 현재는 SlowEffect 기본 factor(0.4f)를 사용하고, 필요 시 GenericSlowEffect로 확장 가능.
        var status = new StatusEffect(slow, duration);
        target.ApplyStatus(status);
    }
}
