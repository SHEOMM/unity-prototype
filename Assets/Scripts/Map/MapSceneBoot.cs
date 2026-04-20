using UnityEngine;

/// <summary>
/// MapScene 진입 시 자동 초기화. MapView + Cosmos 패널 + 버튼 생성.
/// 활성 씬 설정 + 환경 적용은 SceneBootBase가 처리.
/// </summary>
public class MapSceneBoot : SceneBootBase
{
    protected override void OnBoot()
    {
        EnsureHud();
        SpawnBackgroundView();

        var map = MapManager.Instance;
        if (map == null) { Debug.LogError("[MapScene] MapManager not found"); return; }

        var viewGo = new GameObject("MapView");
        var view = viewGo.AddComponent<MapView>();
        view.Initialize(map);
        view.Show();

        // Cosmos 패널 (초기 숨김) + 상단 토글 버튼
        var panelGo = new GameObject("CosmosPanel");
        var panel = panelGo.AddComponent<CosmosPanelView>();
        panel.Initialize();

        var btnGo = new GameObject("CosmosButton");
        var btn = btnGo.AddComponent<CosmosMapButton>();
        btn.Bind(panel);
    }

    static void SpawnBackgroundView()
    {
        var cam = CameraService.Instance?.Camera;
        var spaceSet = BackgroundResolver.ResolveSpace(BackgroundKey.Map);
        if (cam == null || spaceSet == null) return;

        float camTop = cam.transform.position.y + cam.orthographicSize;
        float camBottom = cam.transform.position.y - cam.orthographicSize;

        var bgGo = new GameObject("BackgroundView");
        bgGo.AddComponent<BackgroundView>()
            .ApplySpace(spaceSet, cam, camBottom, camTop, GameConstants.SortingOrder.BackgroundSpace);
    }
}
