#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Assets/art/space_background/*.png → SpaceBackgroundSet SO 자동 생성
/// Assets/art/ground_background/nature_N/*.png → GroundBackgroundSet SO 자동 생성
///
/// 출력: Assets/Data/Backgrounds/{Space_|Ground_}{name}.asset
/// 카탈로그: Assets/Data/BackgroundLibrary.asset
///
/// 각 PNG의 TextureImporter를 Sprite/Single 모드로 강제 전환 (기본 Multiple이면 조정).
/// Ground 폴더는 "1.png, 2.png, ..." 자연수 정렬해 layers[]에, "orig.png"는 compositeFallback에 주입.
/// "origbig.png"는 고해상도 소스라 무시.
/// </summary>
public static class BackgroundSetGenerator
{
    const string SpaceSourceDir = "Assets/art/space_background";
    const string GroundSourceDir = "Assets/art/ground_background";
    const string OutputDir = "Assets/Data/Backgrounds";
    const string LibraryPath = "Assets/Data/BackgroundLibrary.asset";

    static readonly Regex TrailingNumber = new Regex(@"(\d+)$", RegexOptions.Compiled);

    [MenuItem("Tools/Art/Generate Background Sets")]
    public static void Generate()
    {
        Directory.CreateDirectory(OutputDir);

        var spaceSets = GenerateSpaceSets();
        var groundSets = GenerateGroundSets();

        // Library 카탈로그 갱신
        var lib = AssetDatabase.LoadAssetAtPath<BackgroundLibrary>(LibraryPath);
        if (lib == null)
        {
            lib = ScriptableObject.CreateInstance<BackgroundLibrary>();
            AssetDatabase.CreateAsset(lib, LibraryPath);
        }
        Undo.RecordObject(lib, "Update Background Library");
        spaceSets.Sort((a, b) => string.Compare(a.setName, b.setName, System.StringComparison.Ordinal));
        groundSets.Sort((a, b) => string.Compare(a.setName, b.setName, System.StringComparison.Ordinal));
        lib.spaceSets = spaceSets.ToArray();
        lib.groundSets = groundSets.ToArray();
        EditorUtility.SetDirty(lib);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[BackgroundGenerator] 완료 — Space {spaceSets.Count}, Ground {groundSets.Count}. Library: {LibraryPath}");
    }

    // ── Space ───────────────────────────────────────────────────

    static List<SpaceBackgroundSet> GenerateSpaceSets()
    {
        var result = new List<SpaceBackgroundSet>();
        if (!Directory.Exists(SpaceSourceDir)) { Debug.LogWarning($"[BackgroundGenerator] 없음: {SpaceSourceDir}"); return result; }

        foreach (var pngPath in Directory.GetFiles(SpaceSourceDir, "*.png"))
        {
            var assetPath = pngPath.Replace('\\', '/');
            var sprite = EnsureSingleSprite(assetPath);
            if (sprite == null) { Debug.LogWarning($"[BackgroundGenerator] Sprite 로드 실패: {assetPath}"); continue; }

            // "Space Background Black" → "black"
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
            var parts = fileName.Split(' ');
            var key = parts.Length > 0 ? parts[parts.Length - 1].ToLowerInvariant() : fileName.ToLowerInvariant();

            var setAssetPath = $"{OutputDir}/Space_{key}.asset";
            var set = AssetDatabase.LoadAssetAtPath<SpaceBackgroundSet>(setAssetPath);
            bool created = set == null;
            if (created)
            {
                set = ScriptableObject.CreateInstance<SpaceBackgroundSet>();
                AssetDatabase.CreateAsset(set, setAssetPath);
            }
            Undo.RecordObject(set, "Generate Space Background Set");
            set.setName = key;
            set.sprite = sprite;
            if (set.tint == default) set.tint = Color.white;
            EditorUtility.SetDirty(set);

            result.Add(set);
            Debug.Log($"[BackgroundGenerator] {(created ? "생성" : "갱신")}: {setAssetPath}");
        }
        return result;
    }

    // ── Ground ──────────────────────────────────────────────────

    static List<GroundBackgroundSet> GenerateGroundSets()
    {
        var result = new List<GroundBackgroundSet>();
        if (!Directory.Exists(GroundSourceDir)) { Debug.LogWarning($"[BackgroundGenerator] 없음: {GroundSourceDir}"); return result; }

        foreach (var dir in Directory.GetDirectories(GroundSourceDir))
        {
            var folderName = Path.GetFileName(dir);
            if (!folderName.StartsWith("nature_")) continue;

            var layerSprites = new List<(int num, Sprite sprite)>();
            Sprite origFallback = null;

            foreach (var pngPath in Directory.GetFiles(dir, "*.png"))
            {
                var assetPath = pngPath.Replace('\\', '/');
                var name = Path.GetFileNameWithoutExtension(assetPath);
                if (name == "origbig") continue;  // 고해상도 소스 무시
                var sprite = EnsureSingleSprite(assetPath);
                if (sprite == null) continue;
                if (name == "orig") { origFallback = sprite; continue; }

                int num = ExtractTrailingNumber(name);
                layerSprites.Add((num, sprite));
            }
            layerSprites.Sort((a, b) => a.num.CompareTo(b.num));

            var layers = new List<Sprite>(layerSprites.Count);
            foreach (var (_, s) in layerSprites) layers.Add(s);

            var setAssetPath = $"{OutputDir}/Ground_{folderName}.asset";
            var set = AssetDatabase.LoadAssetAtPath<GroundBackgroundSet>(setAssetPath);
            bool created = set == null;
            if (created)
            {
                set = ScriptableObject.CreateInstance<GroundBackgroundSet>();
                AssetDatabase.CreateAsset(set, setAssetPath);
            }
            Undo.RecordObject(set, "Generate Ground Background Set");
            set.setName = folderName;
            set.layers = layers.ToArray();
            set.compositeFallback = origFallback;
            // parallaxFactors 길이 = layers 길이 (0으로 초기화, 정적 배경)
            if (set.parallaxFactors == null || set.parallaxFactors.Length != layers.Count)
                set.parallaxFactors = new float[layers.Count];
            EditorUtility.SetDirty(set);

            result.Add(set);
            Debug.Log($"[BackgroundGenerator] {(created ? "생성" : "갱신")}: {folderName} (layers={layers.Count}, fallback={(origFallback != null ? "O" : "X")})");
        }
        return result;
    }

    // ── 유틸 ────────────────────────────────────────────────────

    static Sprite EnsureSingleSprite(string pngAssetPath)
    {
        var importer = AssetImporter.GetAtPath(pngAssetPath) as TextureImporter;
        if (importer == null) return null;
        bool changed = false;
        if (importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            changed = true;
        }
        if (importer.spriteImportMode != SpriteImportMode.Single)
        {
            importer.spriteImportMode = SpriteImportMode.Single;
            changed = true;
        }
        if (changed) importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(pngAssetPath);
    }

    static int ExtractTrailingNumber(string name)
    {
        var m = TrailingNumber.Match(name);
        return m.Success && int.TryParse(m.Groups[1].Value, out var n) ? n : int.MaxValue;
    }
}
#endif
