using UnityEngine;

/// <summary>
/// 지점 중심 원형 AOE 데미지. EnemyRegistry.GetNearby를 이용해 반경 내 모든 적에게 피격.
/// </summary>
public static class AoeApplicator
{
    public static int Damage(Vector2 center, float radius, float damage, Element element = Element.None)
    {
        if (EnemyRegistry.Instance == null) return 0;
        var enemies = EnemyRegistry.Instance.GetNearby(center, radius);
        int n = 0;
        foreach (var e in enemies)
        {
            if (e == null) continue;
            e.TakeDamage(damage, element);
            n++;
        }
        return n;
    }
}
