using UnityEngine;

/// <summary>
/// 직선 경로상의 적들을 관통 피격. 파도(Water) 시너지의 기본 primitive.
/// 선분 start→end 주변 width 반경 내 적에 대해 데미지 + 선택적 넉백.
/// </summary>
public static class SweepLine
{
    public static int Sweep(Vector2 start, Vector2 end, float width, float damage,
                            Element element = Element.None, bool piercing = true, float knockbackStrength = 0f)
    {
        if (EnemyRegistry.Instance == null) return 0;
        Vector2 seg = end - start;
        float segLenSq = seg.sqrMagnitude;
        if (segLenSq < 1e-6f) return 0;

        Vector2 segDir = seg / Mathf.Sqrt(segLenSq);
        var enemies = EnemyRegistry.Instance.GetAll();
        int hits = 0;
        foreach (var e in enemies)
        {
            if (e == null) continue;
            Vector2 ep = e.transform.position;
            Vector2 toEnemy = ep - start;
            float t = Mathf.Clamp(Vector2.Dot(toEnemy, seg) / segLenSq, 0f, 1f);
            Vector2 nearest = start + seg * t;
            if ((ep - nearest).sqrMagnitude <= width * width)
            {
                e.TakeDamage(damage, element);
                if (knockbackStrength > 0f)
                    KnockbackApplicator.Apply(e, segDir, knockbackStrength);
                hits++;
                if (!piercing) break;
            }
        }
        return hits;
    }
}
