/// <summary>
/// 아군 회복 primitive. AllyRegistry의 살아있는 아군 전체에 같은 양만큼 Heal.
/// 사랑별(AnyLove) 및 향후 Heal 계열 시너지에서 재사용.
/// </summary>
public static class AllyHealApplicator
{
    public static int HealAll(float amount)
    {
        if (AllyRegistry.Instance == null || amount <= 0f) return 0;
        int n = 0;
        foreach (var a in AllyRegistry.Instance.GetAll())
        {
            if (a == null || !a.IsAlive) continue;
            a.Heal(amount);
            n++;
        }
        return n;
    }
}
