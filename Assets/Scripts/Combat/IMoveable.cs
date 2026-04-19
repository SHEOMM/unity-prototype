using UnityEngine;

/// <summary>
/// 이동 가능한 전투 개체. Enemy, AllyUnit 구현 (Structure는 미구현).
/// Knockback/Slow 관련 primitive가 타겟팅.
/// </summary>
public interface IMoveable
{
    /// <summary>원본 이동속도. SlowEffect 등이 복원용으로 사용.</summary>
    float BaseSpeed { get; }

    /// <summary>넉백 누적. Update에서 감쇠 이동으로 소비.</summary>
    void ApplyKnockback(Vector2 delta);
}
