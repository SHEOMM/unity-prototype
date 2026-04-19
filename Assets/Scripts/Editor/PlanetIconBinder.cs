#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 행성 아이콘 매핑 관리 Editor 메뉴 2종.
///
/// Seed: PlanetIconBindingTable에 12개 행성의 추천 기본 매핑을 채운다.
///   (Planet asset 경로 ↔ 스프라이트 이름 하드코딩)
/// Apply: BindingTable의 entries를 순회하며 각 PlanetSO.icon에 주입 + SetDirty.
///
/// 매핑 변경은 Inspector에서 BindingTable 편집 후 Apply만 실행하면 됨.
/// </summary>
public static class PlanetIconBinder
{
    const string TablePath = "Assets/Data/PlanetIconBindingTable.asset";

    /// <summary>행성 에셋 경로 → 권장 스프라이트 이름. 변경 시 이 딕셔너리만 편집.</summary>
    static readonly (string planetPath, string spriteName)[] DefaultMap =
    {
        // Big — 보스/강력 행성
        ("Assets/Data/Planets/Planet_Warrior.asset",   "big_planet_001"),
        ("Assets/Data/Planets/Planet_Ocean.asset",     "big_planet_005"),
        ("Assets/Data/Planets/Planet_Storm.asset",     "big_planet_002"),
        ("Assets/Data/Planets/Planet_Archer.asset",    "big_planet_009"),
        ("Assets/Data/Planets/Planet_Love.asset",      "big_planet_004"),
        // Medium — 중간 티어 행성
        ("Assets/Data/Planets/Planet_Emperor.asset",   "medium_planet_018"),
        ("Assets/Data/Planets/Planet_Smith.asset",     "medium_planet_007"),
        ("Assets/Data/Planets/Planet_Assassin.asset",  "medium_planet_006"),
        ("Assets/Data/Planets/Planet_Messenger.asset", "medium_planet_004"),
        ("Assets/Data/Planets/Planet_Aquarius.asset",  "medium_planet_024"),
        ("Assets/Data/Planets/Planet_Twin.asset",      "medium_planet_014"),
        // Very Small — 특수 테마
        ("Assets/Data/Planets/Planet_Scorpion.asset",  "very_small_planet_001"),
    };

    [MenuItem("Tools/Art/Seed Planet Icon Bindings")]
    public static void Seed()
    {
        var table = LoadOrCreateTable();
        var entries = new List<PlanetIconBindingTable.Entry>();
        int missingPlanet = 0, missingSprite = 0;

        foreach (var (planetPath, spriteName) in DefaultMap)
        {
            var planet = AssetDatabase.LoadAssetAtPath<PlanetSO>(planetPath);
            if (planet == null) { Debug.LogWarning($"[IconBinder] Planet 못 찾음: {planetPath}"); missingPlanet++; continue; }

            var sprite = FindSpriteByName(spriteName);
            if (sprite == null) { Debug.LogWarning($"[IconBinder] Sprite 못 찾음: {spriteName} (슬라이싱 확인)"); missingSprite++; continue; }

            entries.Add(new PlanetIconBindingTable.Entry { planet = planet, sprite = sprite });
        }

        table.entries = entries.ToArray();
        EditorUtility.SetDirty(table);
        AssetDatabase.SaveAssets();

        Debug.Log($"[IconBinder] Seed 완료: {entries.Count}개 엔트리 기록 " +
                  $"(Planet 누락 {missingPlanet}, Sprite 누락 {missingSprite}). 편집 후 Apply 실행.");
    }

    [MenuItem("Tools/Art/Apply Planet Icon Bindings")]
    public static void Apply()
    {
        var table = AssetDatabase.LoadAssetAtPath<PlanetIconBindingTable>(TablePath);
        if (table == null || table.entries == null || table.entries.Length == 0)
        {
            Debug.LogError($"[IconBinder] BindingTable 비어있음: {TablePath}. Seed 먼저 실행.");
            return;
        }

        int applied = 0, skipped = 0;
        foreach (var e in table.entries)
        {
            if (e.planet == null || e.sprite == null) { skipped++; continue; }
            if (e.planet.icon != e.sprite)
            {
                Undo.RecordObject(e.planet, "Apply Planet Icon");
                e.planet.icon = e.sprite;
                EditorUtility.SetDirty(e.planet);
                applied++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[IconBinder] Apply 완료: 적용 {applied}, 스킵 {skipped} / 총 {table.entries.Length}");
    }

    static PlanetIconBindingTable LoadOrCreateTable()
    {
        var table = AssetDatabase.LoadAssetAtPath<PlanetIconBindingTable>(TablePath);
        if (table == null)
        {
            table = ScriptableObject.CreateInstance<PlanetIconBindingTable>();
            System.IO.Directory.CreateDirectory("Assets/Data");
            AssetDatabase.CreateAsset(table, TablePath);
        }
        return table;
    }

    static Sprite FindSpriteByName(string spriteName)
    {
        // 라이브러리 있으면 먼저 조회, 없으면 PNG에서 직접 탐색.
        var lib = AssetDatabase.LoadAssetAtPath<PlanetSpriteLibrary>("Assets/Data/PlanetSpriteLibrary.asset");
        if (lib != null)
        {
            var s = lib.FindByName(spriteName);
            if (s != null) return s;
        }

        foreach (var path in new[] {
            "Assets/art/planet/big_planet.png",
            "Assets/art/planet/medium_planet.png",
            "Assets/art/planet/small_planet.png",
            "Assets/art/planet/very_small_planet.png"})
        {
            foreach (var a in AssetDatabase.LoadAllAssetsAtPath(path))
                if (a is Sprite sp && sp.name == spriteName) return sp;
        }
        return null;
    }
}
#endif
