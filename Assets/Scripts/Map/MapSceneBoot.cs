using UnityEngine;

/// <summary>
/// MapScene 진입 시 자동 초기화. MapView를 생성하고 MapManager에 연결.
/// </summary>
public class MapSceneBoot : MonoBehaviour
{
    void Start()
    {
        var map = MapManager.Instance;
        if (map == null) { Debug.LogError("[MapScene] MapManager not found"); return; }

        Debug.Log($"[MapScene] Boot — Camera.main = {Camera.main}");

        var viewGo = new GameObject("MapView");
        var view = viewGo.AddComponent<MapView>();
        view.Initialize(map);
        view.Show();
    }
}
