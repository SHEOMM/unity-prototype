#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 행성 애니메이션 매핑 관리 Editor 메뉴 2종.
///
/// Seed: 12개 PlanetSO에 planet2 클립 추천 기본 매핑을 채운다 (기획 의도 반영).
/// Apply: 테이블의 entries를 순회하며 각 PlanetSO.icon에 clip.Icon(첫 프레임) 주입.
///        → 정적 사용처(Cosmos 토큰, Reward 카드) 호환.
///
/// 매핑 변경은 Inspector에서 BindingTable 편집 후 Apply.
/// </summary>
public static class PlanetAnimationBinder
{
    const string TablePath = "Assets/Data/PlanetAnimationBindingTable.asset";
    const string AnimationDir = "Assets/Data/Planets/Animations";

    /// <summary>Planet 에셋 경로 → 권장 clip 이름. 기획 의도 반영, Inspector에서 자유 변경 가능.</summary>
    static readonly (string planetPath, string clipName)[] DefaultMap =
    {
        ("Assets/Data/Planets/Planet_Aquarius.asset",  "blue"),          // 물병별 — 심해·정화
        ("Assets/Data/Planets/Planet_Archer.asset",    "blue_star"),     // 궁수별 — 청색 항성·원거리
        ("Assets/Data/Planets/Planet_Assassin.asset",  "black_hole"),    // 암살자 — 다크·은신
        ("Assets/Data/Planets/Planet_Emperor.asset",   "red_star"),      // 제왕별 — 적색 거성·번개
        ("Assets/Data/Planets/Planet_Love.asset",      "saturn"),        // 사랑별 — 고리·조화
        ("Assets/Data/Planets/Planet_Messenger.asset", "mars"),          // 전령별 — 기동
        ("Assets/Data/Planets/Planet_Ocean.asset",     "ice"),           // 바다별 — 빙하 대양
        ("Assets/Data/Planets/Planet_Scorpion.asset",  "asteroid"),      // 전갈별 — 작고 빠름
        ("Assets/Data/Planets/Planet_Smith.asset",     "fire"),          // 대장장이 — 용광로
        ("Assets/Data/Planets/Planet_Storm.asset",     "jupiter"),       // 폭풍별 — 가스 소용돌이
        ("Assets/Data/Planets/Planet_Twin.asset",      "island_civil"),  // 쌍둥이별 — 이원·문명
        ("Assets/Data/Planets/Planet_Warrior.asset",   "earth"),         // 전사별 — 대지·전장
    };

    [MenuItem("Tools/Art/Seed Planet Animation Bindings")]
    public static void Seed()
    {
        var table = LoadOrCreateTable();
        var entries = new List<PlanetAnimationBindingTable.Entry>();
        int missingPlanet = 0, missingClip = 0;

        foreach (var (planetPath, clipName) in DefaultMap)
        {
            var planet = AssetDatabase.LoadAssetAtPath<PlanetSO>(planetPath);
            if (planet == null) { Debug.LogWarning($"[PlanetAnimBinder] Planet 없음: {planetPath}"); missingPlanet++; continue; }

            var clip = AssetDatabase.LoadAssetAtPath<PlanetAnimationClip>($"{AnimationDir}/{clipName}.asset");
            if (clip == null) { Debug.LogWarning($"[PlanetAnimBinder] Clip 없음: {clipName} (Generate 먼저 실행)"); missingClip++; continue; }

            entries.Add(new PlanetAnimationBindingTable.Entry { planet = planet, clip = clip });
        }

        Undo.RecordObject(table, "Seed Planet Animation Bindings");
        table.entries = entries.ToArray();
        EditorUtility.SetDirty(table);
        AssetDatabase.SaveAssets();

        Debug.Log($"[PlanetAnimBinder] Seed 완료: {entries.Count}개 엔트리 " +
                  $"(Planet 누락 {missingPlanet}, Clip 누락 {missingClip}). Apply 실행해 icon 주입.");
    }

    [MenuItem("Tools/Art/Apply Planet Animation Bindings")]
    public static void Apply()
    {
        var table = AssetDatabase.LoadAssetAtPath<PlanetAnimationBindingTable>(TablePath);
        if (table == null || table.entries == null || table.entries.Length == 0)
        {
            Debug.LogError($"[PlanetAnimBinder] BindingTable 비어있음: {TablePath}. Seed 먼저 실행.");
            return;
        }

        int applied = 0, skipped = 0;
        foreach (var e in table.entries)
        {
            if (e.planet == null || e.clip == null || e.clip.Icon == null) { skipped++; continue; }
            if (e.planet.icon != e.clip.Icon)
            {
                Undo.RecordObject(e.planet, "Apply Planet Animation Icon");
                e.planet.icon = e.clip.Icon;
                EditorUtility.SetDirty(e.planet);
                applied++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[PlanetAnimBinder] Apply 완료: 적용 {applied}, 스킵 {skipped} / 총 {table.entries.Length}");
    }

    static PlanetAnimationBindingTable LoadOrCreateTable()
    {
        var table = AssetDatabase.LoadAssetAtPath<PlanetAnimationBindingTable>(TablePath);
        if (table == null)
        {
            table = ScriptableObject.CreateInstance<PlanetAnimationBindingTable>();
            System.IO.Directory.CreateDirectory("Assets/Data");
            AssetDatabase.CreateAsset(table, TablePath);
        }
        return table;
    }
}
#endif
