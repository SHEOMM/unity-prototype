/// <summary>
/// 처형 — HP 비율이 임계 이하면 즉시 사망.
/// 사용 예: 어둠 시너지 "HP 25% 이하 적 처형", Pluto 행성 시너지.
/// </summary>
public static class ExecuteApplicator
{
    /// <summary>target.currentHP / target.maxHP &lt;= hpRatio면 처형. true 반환 시 성공.</summary>
    public static bool TryExecute(Enemy target, float hpRatio)
    {
        if (target == null || target.maxHP <= 0f) return false;
        if (target.currentHP <= target.maxHP * hpRatio)
        {
            target.TakeDamage(target.currentHP + 1f, Element.None);
            return true;
        }
        return false;
    }
}
