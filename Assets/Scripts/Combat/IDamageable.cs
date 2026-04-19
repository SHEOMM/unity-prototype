using UnityEngine;

/// <summary>
/// 피해를 받을 수 있는 전투 개체. Enemy, AllyUnit, Structure가 구현.
/// Primitive(ExecuteApplicator 등)가 팀 무관하게 작동하기 위한 공통 계약.
/// </summary>
public interface IDamageable
{
    float CurrentHP { get; }
    float MaxHP { get; }
    Vector3 Position { get; }
    bool IsAlive { get; }

    void TakeDamage(float damage, Element element = Element.None);

    event System.Action<float, Element> OnDamaged;
    event System.Action OnDeath;
}
