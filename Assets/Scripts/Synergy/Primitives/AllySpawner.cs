using UnityEngine;

/// <summary>
/// 아군 유닛 생성 primitive. SynergyRuleSO의 spawnArea를 기준으로 랜덤 또는 고정 위치에 소환.
/// War/Mars 시너지가 호출해 유닛을 전개.
/// </summary>
public static class AllySpawner
{
    /// <summary>Rect 영역 내 랜덤 위치에 count마리 스폰.</summary>
    public static void Spawn(AllySO prefabData, Rect area, int count)
    {
        if (prefabData == null || count <= 0) return;
        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(area.xMin, area.xMax);
            float y = Random.Range(area.yMin, area.yMax);
            SpawnAt(prefabData, new Vector2(x, y));
        }
    }

    /// <summary>지정 위치에 1마리 스폰.</summary>
    public static AllyUnit SpawnAt(AllySO prefabData, Vector2 position)
    {
        if (prefabData == null) return null;
        var go = new GameObject("Ally_" + prefabData.allyName);
        go.transform.position = (Vector3)position;
        var ally = go.AddComponent<AllyUnit>();
        ally.Initialize(prefabData, UIFactory.MakePixel());
        go.AddComponent<AllyView>();
        go.AddComponent<StatusIconView>();
        return ally;
    }
}
