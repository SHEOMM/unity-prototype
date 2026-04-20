#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Assets/art/planet2/*.png 시트들을 순회하며 각 시트마다 PlanetAnimationClip 에셋을 자동 생성/갱신.
/// 슬라이싱된 서브 스프라이트를 번호순으로 정렬해 frames[] 배열에 주입.
///
/// 출력 경로: Assets/Data/Planets/Animations/{name}.asset
/// 카탈로그:  Assets/Data/PlanetAnimationLibrary.asset (없으면 생성)
///
/// 필터링: 프레임 최소 크기 MinFrameSize 미만은 제외 (항성 시트의 파티클/반짝임 스프라이트 방어).
/// </summary>
public static class PlanetAnimationClipGenerator
{
    const string SourceDir = "Assets/art/planet2";
    const string OutputDir = "Assets/Data/Planets/Animations";
    const string LibraryPath = "Assets/Data/PlanetAnimationLibrary.asset";
    const float DefaultFps = 10f;

    /// <summary>프레임 최소 크기 (너비 또는 높이). 항성 시트의 6×8 파편 등 제외.</summary>
    const int MinFrameSize = 50;

    static readonly Regex TrailingNumber = new Regex(@"_(\d+)$", RegexOptions.Compiled);

    [MenuItem("Tools/Art/Generate Planet Animation Clips")]
    public static void Generate()
    {
        if (!Directory.Exists(SourceDir))
        {
            Debug.LogError($"[PlanetAnimGenerator] 소스 폴더 없음: {SourceDir}");
            return;
        }

        Directory.CreateDirectory(OutputDir);

        var allClips = new List<PlanetAnimationClip>();
        int total = 0, skipped = 0;

        var pngs = Directory.GetFiles(SourceDir, "*.png");
        System.Array.Sort(pngs);

        foreach (var pngPath in pngs)
        {
            var assetPath = pngPath.Replace('\\', '/');
            var clipName = Path.GetFileNameWithoutExtension(assetPath);
            var frames = LoadSortedFrames(assetPath);

            if (frames.Length == 0)
            {
                Debug.LogWarning($"[PlanetAnimGenerator] 프레임 0개 (슬라이싱 확인): {clipName}");
                skipped++;
                continue;
            }

            var clipAssetPath = $"{OutputDir}/{clipName}.asset";
            var clip = AssetDatabase.LoadAssetAtPath<PlanetAnimationClip>(clipAssetPath);
            bool created = clip == null;
            if (created)
            {
                clip = ScriptableObject.CreateInstance<PlanetAnimationClip>();
                AssetDatabase.CreateAsset(clip, clipAssetPath);
            }

            Undo.RecordObject(clip, "Generate Planet Animation Clip");
            clip.clipName = clipName;
            clip.frames = frames;
            if (clip.fps <= 0.1f) clip.fps = DefaultFps;
            EditorUtility.SetDirty(clip);

            allClips.Add(clip);
            total++;
            Debug.Log($"[PlanetAnimGenerator] {(created ? "생성" : "갱신")}: {clipName} ({frames.Length} frames, {clip.fps} fps)");
        }

        // Library 갱신
        var lib = AssetDatabase.LoadAssetAtPath<PlanetAnimationLibrary>(LibraryPath);
        if (lib == null)
        {
            lib = ScriptableObject.CreateInstance<PlanetAnimationLibrary>();
            AssetDatabase.CreateAsset(lib, LibraryPath);
        }
        Undo.RecordObject(lib, "Update Planet Animation Library");
        allClips.Sort((a, b) => string.Compare(a.clipName, b.clipName, System.StringComparison.Ordinal));
        lib.clips = allClips.ToArray();
        EditorUtility.SetDirty(lib);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[PlanetAnimGenerator] 완료 — 클립 {total}개, 스킵 {skipped}개. 카탈로그: {LibraryPath}");
    }

    /// <summary>Unity가 기본 maxTextureSize=2048로 큰 시트를 축소시키는 걸 방지하기 위해 8192로 상향.</summary>
    const int TargetMaxTextureSize = 8192;

    static Sprite[] LoadSortedFrames(string pngAssetPath)
    {
        // 먼저 TextureImporter의 maxTextureSize를 올려 원본 해상도 보존 (필수: 자동 축소 방지)
        EnsureMaxTextureSize(pngAssetPath);

        // Asset import 보장 (maxTextureSize 변경 반영)
        AssetDatabase.ImportAsset(pngAssetPath, ImportAssetOptions.ForceSynchronousImport);

        var all = AssetDatabase.LoadAllAssetsAtPath(pngAssetPath);
        var list = new List<Sprite>();
        foreach (var a in all)
        {
            if (!(a is Sprite s)) continue;
            if (s.rect.width < MinFrameSize || s.rect.height < MinFrameSize) continue;
            list.Add(s);
        }
        // "earth_0, earth_1, ..., earth_9, earth_10" 자연수 정렬
        list.Sort((x, y) => CompareTrailingNumber(x.name, y.name));
        return list.ToArray();
    }

    static void EnsureMaxTextureSize(string pngAssetPath)
    {
        var importer = AssetImporter.GetAtPath(pngAssetPath) as TextureImporter;
        if (importer == null) return;
        if (importer.maxTextureSize >= TargetMaxTextureSize) return;
        importer.maxTextureSize = TargetMaxTextureSize;
        importer.SaveAndReimport();
        Debug.Log($"[PlanetAnimGenerator] {pngAssetPath}: maxTextureSize → {TargetMaxTextureSize}");
    }

    static int CompareTrailingNumber(string a, string b)
    {
        int na = ExtractTrailingNumber(a);
        int nb = ExtractTrailingNumber(b);
        if (na != int.MinValue && nb != int.MinValue && na != nb) return na.CompareTo(nb);
        return string.Compare(a, b, System.StringComparison.Ordinal);
    }

    static int ExtractTrailingNumber(string name)
    {
        var m = TrailingNumber.Match(name);
        return m.Success && int.TryParse(m.Groups[1].Value, out var n) ? n : int.MinValue;
    }
}
#endif
