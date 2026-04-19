using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체인 라이트닝: 시작 지점에서 가장 가까운 적부터 jumps번 전파하며 데미지.
/// 이미 맞은 적은 재선택하지 않음.
/// </summary>
public static class ChainLightning
{
    public static int Chain(Vector2 origin, int jumps, float damage, float jumpRadius, Element element = Element.None)
    {
        if (EnemyRegistry.Instance == null || jumps <= 0) return 0;
        var visited = new HashSet<Enemy>();
        Vector2 pos = origin;
        int hits = 0;
        for (int i = 0; i < jumps; i++)
        {
            Enemy next = FindNearest(pos, jumpRadius, visited);
            if (next == null) break;
            next.TakeDamage(damage, element);
            visited.Add(next);
            pos = next.transform.position;
            hits++;
        }
        return hits;
    }

    static Enemy FindNearest(Vector2 pos, float radius, HashSet<Enemy> excluded)
    {
        var candidates = EnemyRegistry.Instance.GetNearby(pos, radius);
        Enemy best = null;
        float bestDistSq = float.MaxValue;
        foreach (var e in candidates)
        {
            if (e == null || excluded.Contains(e)) continue;
            float d = ((Vector2)e.transform.position - pos).sqrMagnitude;
            if (d < bestDistSq) { bestDistSq = d; best = e; }
        }
        return best;
    }
}
