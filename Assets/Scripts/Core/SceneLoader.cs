using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Additive 씬 전환 관리. PersistentScene은 항상 유지하고, 다른 씬만 교체한다.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private string _currentSceneName;
    public string CurrentSceneName => _currentSceneName;
    public bool IsLoading { get; private set; }

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
            var unload = SceneManager.UnloadSceneAsync(_currentSceneName);
            if (unload != null)
                yield return unload;
        }

        // 새 씬 Additive 로드
        var load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (load != null)
            yield return load;

        _currentSceneName = sceneName;
        IsLoading = false;

        Debug.Log($"[SceneLoader] {sceneName} 로드 완료");
        onComplete?.Invoke();
    }
}
