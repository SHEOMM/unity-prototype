#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Assets/art/character/{characterFolder}/ 폴더를 순회하여 각 *.png 시트(수직 필름스트립)을
/// CharacterAnimationClip 에셋으로 변환하고, 상태별 매핑된 CharacterAnimationSet으로 모은다.
///
/// 파일명 규칙: "{prefix}_{state}.png" 또는 "{prefix}_{state1}_{state2}.png" (예: take_damage)
/// 인식 상태: idle / run / charge / attack / take_damage / death
///
/// 출력:
///   Assets/Data/Characters/{folderName}/{stateName}.asset   ← clip
///   Assets/Data/Characters/{folderName}.asset               ← Set (상태별 clip 참조)
/// </summary>
public static class CharacterAnimationGenerator
{
    const string SourceDir = "Assets/art/character";
    const string OutputDir = "Assets/Data/Characters";
    const float DefaultFps = 10f;

    /// <summary>캐릭터 시트의 최소 프레임 크기 (픽셀). 투사체·파편 스프라이트 제외용.</summary>
    const int MinFrameSize = 15;

    static readonly Regex TrailingNumber = new Regex(@"(\d+)$", RegexOptions.Compiled);

    [MenuItem("Tools/Art/Generate Character Animation Sets")]
    public static void Generate()
    {
        if (!Directory.Exists(SourceDir))
        {
            Debug.LogError($"[CharAnimGenerator] 없음: {SourceDir}");
            return;
        }

        Directory.CreateDirectory(OutputDir);
        int totalSets = 0, totalClips = 0;

        foreach (var dir in Directory.GetDirectories(SourceDir))
        {
            var folderName = Path.GetFileName(dir);
            var setOutDir = $"{OutputDir}/{folderName}";
            Directory.CreateDirectory(setOutDir);

            var set = AssetDatabase.LoadAssetAtPath<CharacterAnimationSet>($"{OutputDir}/{folderName}.asset");
            bool setCreated = set == null;
            if (setCreated)
            {
                set = ScriptableObject.CreateInstance<CharacterAnimationSet>();
                AssetDatabase.CreateAsset(set, $"{OutputDir}/{folderName}.asset");
            }
            Undo.RecordObject(set, "Generate Character Animation Set");
            set.characterName = folderName;

            int clipCount = 0;
            foreach (var pngPath in Directory.GetFiles(dir, "*.png"))
            {
                var assetPath = pngPath.Replace('\\', '/');
                var stateKey = ExtractStateKey(Path.GetFileNameWithoutExtension(assetPath));
                if (stateKey == CharacterAnimationState.Idle && !Path.GetFileNameWithoutExtension(assetPath).EndsWith("idle"))
                    continue;  // 상태 인식 실패한 시트는 스킵 (기본값 Idle로 잘못 할당 방지)

                var clip = GenerateClip(assetPath, stateKey, setOutDir);
                if (clip == null) continue;

                AssignToSet(set, stateKey, clip);
                clipCount++;
            }

            EditorUtility.SetDirty(set);
            totalSets++;
            totalClips += clipCount;
            Debug.Log($"[CharAnimGenerator] {(setCreated ? "생성" : "갱신")}: {folderName} — clips {clipCount}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[CharAnimGenerator] 완료 — Sets {totalSets}, Clips {totalClips}");
    }

    static CharacterAnimationClip GenerateClip(string pngAssetPath, CharacterAnimationState state, string outDir)
    {
        // PNG 임포트 설정 검증 (Sprite/Multiple, 기본적으로 이미 슬라이싱된 상태 가정)
        AssetDatabase.ImportAsset(pngAssetPath, ImportAssetOptions.ForceSynchronousImport);

        var all = AssetDatabase.LoadAllAssetsAtPath(pngAssetPath);
        var list = new List<Sprite>();
        foreach (var a in all)
        {
            if (!(a is Sprite s)) continue;
            if (s.rect.width < MinFrameSize || s.rect.height < MinFrameSize) continue;
            list.Add(s);
        }
        // 수직 필름스트립이므로 y 좌표 역순 정렬 (위→아래가 프레임 순서인 시트가 일반적)
        // 실제론 이름에 _0, _1 suffix가 있으면 그것을 우선 사용.
        list.Sort((x, y) => CompareByTrailingNumberOrY(x, y));

        if (list.Count == 0)
        {
            Debug.LogWarning($"[CharAnimGenerator] {pngAssetPath}: 유효 프레임 0 (Min={MinFrameSize})");
            return null;
        }

        var clipPath = $"{outDir}/{Path.GetFileNameWithoutExtension(pngAssetPath)}.asset";
        var clip = AssetDatabase.LoadAssetAtPath<CharacterAnimationClip>(clipPath);
        bool created = clip == null;
        if (created)
        {
            clip = ScriptableObject.CreateInstance<CharacterAnimationClip>();
            AssetDatabase.CreateAsset(clip, clipPath);
        }
        Undo.RecordObject(clip, "Generate Character Animation Clip");
        clip.clipName = Path.GetFileNameWithoutExtension(pngAssetPath);
        clip.frames = list.ToArray();
        if (clip.fps <= 0.1f) clip.fps = DefaultFps;
        // TakeDamage·Death는 oneshot, 나머지는 loop
        clip.loop = (state != CharacterAnimationState.TakeDamage && state != CharacterAnimationState.Death);
        EditorUtility.SetDirty(clip);

        Debug.Log($"[CharAnimGenerator] {(created ? "생성" : "갱신")} clip: {clipPath} (state={state}, frames={list.Count}, loop={clip.loop})");
        return clip;
    }

    static CharacterAnimationState ExtractStateKey(string fileName)
    {
        // 파일명 끝에서 상태 키워드를 역으로 찾기 (긴 것 우선)
        var lower = fileName.ToLowerInvariant();
        if (lower.EndsWith("take_damage") || lower.EndsWith("takedamage")) return CharacterAnimationState.TakeDamage;
        if (lower.EndsWith("idle"))   return CharacterAnimationState.Idle;
        if (lower.EndsWith("run"))    return CharacterAnimationState.Run;
        if (lower.EndsWith("charge")) return CharacterAnimationState.Charge;
        if (lower.EndsWith("attack")) return CharacterAnimationState.Attack;
        if (lower.EndsWith("death"))  return CharacterAnimationState.Death;
        return CharacterAnimationState.Idle;  // 기본값 — 호출자가 필터링함
    }

    static void AssignToSet(CharacterAnimationSet set, CharacterAnimationState state, CharacterAnimationClip clip)
    {
        switch (state)
        {
            case CharacterAnimationState.Idle:       set.idle       = clip; break;
            case CharacterAnimationState.Run:        set.run        = clip; break;
            case CharacterAnimationState.Charge:     set.charge     = clip; break;
            case CharacterAnimationState.Attack:     set.attack     = clip; break;
            case CharacterAnimationState.TakeDamage: set.takeDamage = clip; break;
            case CharacterAnimationState.Death:      set.death      = clip; break;
        }
    }

    static int CompareByTrailingNumberOrY(Sprite a, Sprite b)
    {
        int na = ExtractTrailingNumber(a.name);
        int nb = ExtractTrailingNumber(b.name);
        if (na != int.MaxValue && nb != int.MaxValue && na != nb) return na.CompareTo(nb);
        // Fallback: 수직 시트는 y가 클수록 "위" 프레임 — 아래서 위로 읽는 관례라면 y 오름차순.
        // 실제 필름스트립 순서는 대개 위→아래(y 내림차순)이므로 y 역순.
        return b.rect.y.CompareTo(a.rect.y);
    }

    static int ExtractTrailingNumber(string name)
    {
        var m = TrailingNumber.Match(name);
        return m.Success && int.TryParse(m.Groups[1].Value, out var n) ? n : int.MaxValue;
    }
}
#endif
