using UnityEngine;

/// <summary>
/// 구조물 설치 primitive. Civilization/Saturn 시너지가 호출해 구조물을 배치.
/// Ally와 달리 정적이라 Rect 내 랜덤 위치가 덜 자연스럽지만 API 일관성 위해 동일 시그니처 유지.
/// </summary>
public static class StructureSpawner
{
    public static void Spawn(StructureSO prefabData, Rect area, int count)
    {
        if (prefabData == null || count <= 0) return;
        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(area.xMin, area.xMax);
            float y = Random.Range(area.yMin, area.yMax);
            SpawnAt(prefabData, new Vector2(x, y));
        }
    }

    public static Structure SpawnAt(StructureSO prefabData, Vector2 position)
    {
        if (prefabData == null) return null;
        var go = new GameObject("Structure_" + prefabData.structureName);
        go.transform.position = (Vector3)position;
        var s = go.AddComponent<Structure>();
        s.Initialize(prefabData, UIFactory.MakePixel());
        return s;
    }
}
