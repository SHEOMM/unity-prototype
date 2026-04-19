/// <summary>
/// 상태이상 전략 인터페이스. 새 상태이상은 이것을 구현하고 [StatusEffectId]를 붙이면 된다.
/// 타겟은 IStatusHost (Enemy, AllyUnit, Structure 공통 계약).
/// </summary>
public interface IStatusEffect
{
    void OnApplied(IStatusHost target) { }
    void Tick(IStatusHost target, float deltaTime);
    void OnExpired(IStatusHost target) { }

    /// <summary>
    /// StatusIconRegistry 조회용 식별자. null이면 UI 아이콘 표시 안 함.
    /// 구현체가 오버라이드해 "stun"/"weakness"/"dot"/"slow" 등의 키를 반환.
    /// </summary>
    string IconId => null;
}
