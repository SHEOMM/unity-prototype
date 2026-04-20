#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// BackgroundBindingTable에 기본 매핑 Seed. Inspector에서 자유롭게 조정 가능.
///
/// 기본 분위기:
///  - Map: blue (평화로운 별밭)
///  - Combat: blue + nature_1 (큰 나무, 기본)
///  - CombatElite: red + nature_3 (설산, 긴장감)
///  - CombatBoss: black + nature_6 (오로라 야경, 절정)
///  - Rest: blue + nature_5 (여름 들판, 휴식)
///  - Shop: blue + nature_2 (열린 초원)
///  - Reward: blue + nature_4 (숲, 보상)
/// </summary>
public static class BackgroundBinder
{
    const string TablePath = "Assets/Data/BackgroundBindingTable.asset";
    const string SetDir = "Assets/Data/Backgrounds";

    /// <summary>Key → (spaceName, groundName). groundName=null이면 지상 미사용.</summary>
    static readonly (BackgroundKey key, string space, string ground)[] DefaultMap =
    {
        (BackgroundKey.Map,          "blue",  null),
        (BackgroundKey.Combat,       "blue",  "nature_1"),
        (BackgroundKey.CombatElite,  "red",   "nature_3"),
        (BackgroundKey.CombatBoss,   "black", "nature_6"),
        (BackgroundKey.Rest,         "blue",  "nature_5"),
        (BackgroundKey.Shop,         "blue",  "nature_2"),
        (BackgroundKey.Reward,       "blue",  "nature_4"),
    };

    [MenuItem("Tools/Art/Seed Background Bindings")]
    public static void Seed()
    {
        var table = LoadOrCreateTable();
        var spaceList = new List<BackgroundBindingTable.SpaceEntry>();
        var groundList = new List<BackgroundBindingTable.GroundEntry>();
        int missingSpace = 0, missingGround = 0;

        foreach (var (key, spaceName, groundName) in DefaultMap)
        {
            var spaceSet = LoadSet<SpaceBackgroundSet>($"{SetDir}/Space_{spaceName}.asset");
            if (spaceSet == null) { Debug.LogWarning($"[BackgroundBinder] Space 없음: {spaceName} (Generate 먼저)"); missingSpace++; }
            else spaceList.Add(new BackgroundBindingTable.SpaceEntry { key = key, set = spaceSet });

            if (!string.IsNullOrEmpty(groundName))
            {
                var groundSet = LoadSet<GroundBackgroundSet>($"{SetDir}/Ground_{groundName}.asset");
                if (groundSet == null) { Debug.LogWarning($"[BackgroundBinder] Ground 없음: {groundName}"); missingGround++; }
                else groundList.Add(new BackgroundBindingTable.GroundEntry { key = key, set = groundSet });
            }
        }

        Undo.RecordObject(table, "Seed Background Bindings");
        table.spaceEntries = spaceList.ToArray();
        table.groundEntries = groundList.ToArray();
        // 폴백: Map 매핑과 동일 (첫 번째 엔트리)
        if (table.defaultSpace == null && spaceList.Count > 0) table.defaultSpace = spaceList[0].set;
        if (table.defaultGround == null && groundList.Count > 0) table.defaultGround = groundList[0].set;
        EditorUtility.SetDirty(table);
        AssetDatabase.SaveAssets();

        Debug.Log($"[BackgroundBinder] Seed 완료: Space {spaceList.Count}, Ground {groundList.Count} (누락 Space {missingSpace}, Ground {missingGround}).");
    }

    static T LoadSet<T>(string path) where T : ScriptableObject
    {
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }

    static BackgroundBindingTable LoadOrCreateTable()
    {
        var table = AssetDatabase.LoadAssetAtPath<BackgroundBindingTable>(TablePath);
        if (table == null)
        {
            table = ScriptableObject.CreateInstance<BackgroundBindingTable>();
            System.IO.Directory.CreateDirectory("Assets/Data");
            AssetDatabase.CreateAsset(table, TablePath);
        }
        return table;
    }
}
#endif
