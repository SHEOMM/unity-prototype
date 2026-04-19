/// <summary>
/// StatusEffect를 받을 수 있는 개체. 피해 가능(IDamageable)이 전제.
/// DoT/Slow 등의 primitive가 타겟팅.
/// </summary>
public interface IStatusHost : IDamageable
{
    void ApplyStatus(StatusEffect status);
}
