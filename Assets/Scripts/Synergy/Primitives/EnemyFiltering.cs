using System.Collections.Generic;

/// <summary>
/// EnemyRegistry의 적들을 조건별로 필터링. Wind 시너지 등 공중/지상 구분용.
/// </summary>
public static class EnemyFiltering
{
    public static IEnumerable<Enemy> GetFlying()
    {
        if (EnemyRegistry.Instance == null) yield break;
        foreach (var e in EnemyRegistry.Instance.GetAll())
            if (e != null && e.Data != null && e.Data.isFlying) yield return e;
    }

    public static IEnumerable<Enemy> GetGround()
    {
        if (EnemyRegistry.Instance == null) yield break;
        foreach (var e in EnemyRegistry.Instance.GetAll())
            if (e != null && e.Data != null && !e.Data.isFlying) yield return e;
    }
}
