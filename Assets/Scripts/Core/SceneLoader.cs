using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Additive 씬 전환 관리. PersistentScene은 항상 유지하고, 다른 씬만 교체한다.
/// 각 씬의 Boot.Start()는 진입 시 <see cref="SceneManager.SetActiveScene"/>을 호출해야
/// 이후 new GameObject()가 해당 씬에 parent된다 (SceneBootExt.SetThisSceneActive).
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private string _currentSceneName;
    public string CurrentSceneName => _currentSceneName;
    public bool IsLoading { get; private set; }

    /// <summary>곧 언로드될 씬 이름. 구독자가 정리 작업 가능.</summary>
    public event System.Action<string> OnSceneWillUnload;
    /// <summary>로드 + 활성화 완료 직후 발행. 구독자가 후처리 훅으로 활용.</summary>
    public event System.Action<string> OnSceneLoaded;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    /// <summary>현재 활성 씬을 언로드하고 새 씬을 Additive로 로드한다.</summary>
    public void LoadScene(string sceneName, System.Action onComplete = null)
    {
        if (IsLoading) return;
        StartCoroutine(LoadSceneRoutine(sceneName, onComplete));
    }

    IEnumerator LoadSceneRoutine(string sceneName, System.Action onComplete)
    {
        IsLoading = true;

        // 현재 씬 언로드
        if (!string.IsNullOrEmpty(_currentSceneName))
        {
            OnSceneWillUnload?.Invoke(_currentSceneName);
            var unload = SceneManager.UnloadSceneAsync(_currentSceneName);
            if (unload != null)
                yield return unload;
        }

        // 새 씬 Additive 로드
        var load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (load != null)
            yield return load;

        _currentSceneName = sceneName;

        // 로드 완료 후 active 씬으로 지정 — 주의: 이미 씬의 Start()는 실행된 뒤다.
        // Boot 스크립트가 자체적으로 SetThisSceneActive()를 호출해야 new GameObject가
        // 자기 씬에 parent된다 (SceneBootExt 참고).
        var loadedScene = SceneManager.GetSceneByName(sceneName);
        if (loadedScene.IsValid())
            SceneManager.SetActiveScene(loadedScene);

        IsLoading = false;

        Debug.Log($"[SceneLoader] {sceneName} 로드 완료 (active)");
        OnSceneLoaded?.Invoke(sceneName);
        onComplete?.Invoke();
    }
}
