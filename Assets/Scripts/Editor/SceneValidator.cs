#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Scene/Camera 아키텍처 불변 조건 에디터 검증.
///
/// 검사 규칙:
///   R1. Main Camera tag를 가진 GameObject는 오직 PersistentScene에만 존재
///   R2. Global Light 2D (Light2D의 lightType=Global)는 오직 PersistentScene에만 존재
///   R3. PersistentScene 이외의 Build 포함 씬은 SceneBootBase 파생 컴포넌트를 정확히 1개 가짐
///
/// 트리거:
///   - 프로젝트 열 때 자동 실행 (InitializeOnLoadMethod)
///   - 씬 저장 시 자동 실행 (EditorSceneManager.sceneSaved)
///   - 메뉴: Tools > Project > Validate Scenes (수동)
/// </summary>
[InitializeOnLoad]
public static class SceneValidator
{
    private const string PersistentSceneName = "PersistentScene";

    static SceneValidator()
    {
        EditorSceneManager.sceneSaved += OnSceneSaved;
        // 프로젝트 열자마자 전체 검증 (compilation 뒤 지연 실행, play mode 진입 시엔 스킵)
        EditorApplication.delayCall += () => { if (!EditorApplication.isPlayingOrWillChangePlaymode) ValidateAll(); };
    }

    [MenuItem("Tools/Project/Validate Scenes")]
    public static void ValidateAll()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("[SceneValidator] Play mode 중에는 검증 불가. Stop 후 다시 시도.");
            return;
        }
        int violations = 0;
        foreach (var scenePath in GetBuildScenePaths())
        {
            violations += ValidateSceneFile(scenePath);
        }
        if (violations == 0)
            Debug.Log("[SceneValidator] 전 씬 규칙 준수 ✓");
        else
            Debug.LogError($"[SceneValidator] {violations}개 규칙 위반 감지 — 콘솔 로그 참고");
    }

    static void OnSceneSaved(Scene scene)
    {
        // Play mode 중 저장은 건너뜀 (EditorSceneManager 호출 불가)
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        ValidateLoadedScene(scene);
    }

    static IEnumerable<string> GetBuildScenePaths()
    {
        foreach (var s in EditorBuildSettings.scenes)
            if (s.enabled) yield return s.path;
    }

    static int ValidateSceneFile(string scenePath)
    {
        // 현재 로드된 씬이면 그대로 검증
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.path == scenePath && s.isLoaded)
                return ValidateLoadedScene(s);
        }
        // 아니면 일시 로드해 검증 후 언로드
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        int v = ValidateLoadedScene(scene);
        EditorSceneManager.CloseScene(scene, true);
        return v;
    }

    static int ValidateLoadedScene(Scene scene)
    {
        int v = 0;
        bool isPersistent = scene.name == PersistentSceneName;
        var roots = scene.GetRootGameObjects();

        // R1: Main Camera tag
        int mainCams = 0;
        foreach (var root in roots)
            mainCams += CountWithTag(root.transform, "MainCamera");
        if (isPersistent)
        {
            if (mainCams != 1)
            { Debug.LogError($"[SceneValidator][R1] {scene.name}: Main Camera는 정확히 1개여야 함 (현재 {mainCams})"); v++; }
        }
        else
        {
            if (mainCams != 0)
            { Debug.LogError($"[SceneValidator][R1] {scene.name}: Main Camera는 PersistentScene에만 허용 (현재 {mainCams}개)"); v++; }
        }

        // R2: Global Light 2D
        int globalLights = 0;
        foreach (var root in roots)
        {
            var lights = root.GetComponentsInChildren<Light2D>(true);
            foreach (var l in lights)
                if (l.lightType == Light2D.LightType.Global) globalLights++;
        }
        if (isPersistent)
        {
            if (globalLights != 1)
            { Debug.LogError($"[SceneValidator][R2] {scene.name}: Global Light 2D는 정확히 1개여야 함 (현재 {globalLights})"); v++; }
        }
        else
        {
            if (globalLights != 0)
            { Debug.LogError($"[SceneValidator][R2] {scene.name}: Global Light 2D는 PersistentScene에만 허용 (현재 {globalLights}개)"); v++; }
        }

        // R3: SceneBootBase 파생 컴포넌트 (PersistentScene은 해당 없음)
        if (!isPersistent)
        {
            int boots = 0;
            foreach (var root in roots)
                boots += root.GetComponentsInChildren<SceneBootBase>(true).Length;
            if (boots != 1)
            { Debug.LogError($"[SceneValidator][R3] {scene.name}: SceneBootBase 파생 컴포넌트는 정확히 1개여야 함 (현재 {boots})"); v++; }
        }

        return v;
    }

    static int CountWithTag(Transform root, string tag)
    {
        int count = 0;
        if (root.CompareTag(tag)) count++;
        foreach (Transform child in root)
            count += CountWithTag(child, tag);
        return count;
    }
}
#endif
