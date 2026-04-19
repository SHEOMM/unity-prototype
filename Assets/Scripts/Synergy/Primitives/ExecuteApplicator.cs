/// <summary>
/// 처형 — HP 비율이 임계 이하면 즉시 사망.
/// 사용 예: 어둠 시너지 "HP 25% 이하 적 처형", Pluto 행성 시너지.
/// </summary>
public static class ExecuteApplicator
{
    /// <summary>target.CurrentHP / target.MaxHP &lt;= hpRatio면 처형. true 반환 시 성공.</summary>
    public static bool TryExecute(IDamageable target, float hpRatio)
    {
        if (target == null || !target.IsAlive || target.MaxHP <= 0f) return false;
        if (target.CurrentHP <= target.MaxHP * hpRatio)
        {
            target.TakeDamage(target.CurrentHP + 1f, Element.None);
            return true;
        }
        return false;
    }
}
