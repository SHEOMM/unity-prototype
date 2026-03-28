/// <summary>
/// 상태이상 전략 인터페이스. 새 상태이상은 이것을 구현하고 [StatusEffectId]를 붙이면 된다.
/// </summary>
public interface IStatusEffect
{
    void OnApplied(Enemy target) { }
    void Tick(Enemy target, float deltaTime);
    void OnExpired(Enemy target) { }
}
