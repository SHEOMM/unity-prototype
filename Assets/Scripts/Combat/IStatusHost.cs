using System.Collections.Generic;

/// <summary>
/// StatusEffect를 받을 수 있는 개체. 피해 가능(IDamageable)이 전제.
/// DoT/Slow 등의 primitive가 타겟팅.
/// </summary>
public interface IStatusHost : IDamageable
{
    void ApplyStatus(StatusEffect status);

    /// <summary>현재 부착 중인 상태이상 목록 (읽기 전용). StatusIconView 등 UI가 조회.</summary>
    IReadOnlyList<StatusEffect> ActiveStatuses { get; }
}
