using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// 분기형 맵 시각화. 노드를 원으로, 경로를 선으로 표시한다.
/// 도달 가능한 노드는 밝게 하이라이트하고 클릭으로 선택 가능.
/// MapManager(모델)의 이벤트를 구독하여 표시만 담당.
/// </summary>
public class MapView : MonoBehaviour
{
    private MapManager _map;
    private System.IDisposable _viewScope;
    private readonly List<GameObject> _nodeObjects = new List<GameObject>();
    private readonly List<GameObject> _lineObjects = new List<GameObject>();
    private readonly Dictionary<MapNode, SpriteRenderer> _nodeRenderers = new Dictionary<MapNode, SpriteRenderer>();
    private readonly Dictionary<MapNode, TextMesh> _nodeLabels = new Dictionary<MapNode, TextMesh>();

    private bool _active;

    public void Initialize(MapManager map)
    {
        _map = map;

        _map.OnMapGenerated += OnMapGenerated;
        _map.OnNodeSelected += OnNodeSelected;

        // 이미 맵이 생성되어 있으면 바로 표시
        if (_map.CurrentMap != null)
            OnMapGenerated(_map.CurrentMap);
    }

    void OnDestroy()
    {
        if (_map != null)
        {
            _map.OnMapGenerated -= OnMapGenerated;
            _map.OnNodeSelected -= OnNodeSelected;
        }
    }

    void Update()
    {
        if (!_active || _map == null) return;

        var mouse = Mouse.current;
        if (mouse == null || CameraService.Instance == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPos = mouse.position.ReadValue();

            // 화면 밖 좌표 무시
            if (screenPos.x < 1f || screenPos.y < 1f) return;
            if (screenPos.x > Screen.width - 1 || screenPos.y > Screen.height - 1) return;

            Vector2 mp = CameraService.Instance.ScreenToWorld2D(screenPos);
            Debug.Log($"[MapView] 클릭 screen={screenPos}, world={mp}");
            TrySelectNode(mp);
        }
    }

    void OnMapGenerated(MapData map)
    {
        ClearView();
        DrawConnections(map);
        DrawNodes(map);
        UpdateReachableHighlights();
        _active = true;
    }

    void OnNodeSelected(MapNode node)
    {
        UpdateReachableHighlights();
    }

    void TrySelectNode(Vector2 clickPos)
    {
        var reachable = _map.GetReachableNodes();
        Debug.Log($"[MapView] 도달 가능 노드: {reachable.Count}개");

        MapNode closest = null;
        float closestDist = 1.5f; // 클릭 허용 반경 (맵 스케일에 맞춤)

        foreach (var node in reachable)
        {
            float dist = Vector2.Distance(clickPos, node.mapPosition);
            Debug.Log($"[MapView]   노드 ({node.floor},{node.column}) {node.roomType} dist={dist:F2}");
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = node;
            }
        }

        if (closest != null)
        {
            Debug.Log($"[MapView] 선택: ({closest.floor},{closest.column}) {closest.roomType}");
            _map.SelectNode(closest);
        }
        else
        {
            Debug.Log($"[MapView] 근처에 도달 가능한 노드 없음");
        }
    }

    void DrawNodes(MapData map)
    {
        foreach (var node in map.ActiveNodes)
            CreateNodeVisual(node);

        if (map.BossNode != null)
            CreateNodeVisual(map.BossNode);
    }

    void CreateNodeVisual(MapNode node)
    {
        var go = new GameObject($"Node_{node.floor}_{node.column}");
        go.transform.position = (Vector3)node.mapPosition;
        go.transform.localScale = Vector3.one * GetNodeScale(node.roomType);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = UIFactory.MakePixel();
        sr.color = GetNodeColor(node.roomType);
        sr.sortingOrder = GameConstants.SortingOrder.Label;

        _nodeObjects.Add(go);
        _nodeRenderers[node] = sr;

        // 라벨
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform);
        labelGo.transform.localPosition = new Vector3(0, -0.4f, 0);
        float invScale = go.transform.localScale.x > 0.01f ? 1f / go.transform.localScale.x : 1f;
        labelGo.transform.localScale = Vector3.one * invScale * 0.3f;

        var tm = labelGo.AddComponent<TextMesh>();
        tm.text = GetNodeSymbol(node.roomType);
        tm.fontSize = 48;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        tm.characterSize = 0.1f;
        labelGo.GetComponent<MeshRenderer>().sortingOrder = GameConstants.SortingOrder.Label + 1;

        _nodeLabels[node] = tm;
    }

    void DrawConnections(MapData map)
    {
        foreach (var node in map.ActiveNodes)
        {
            foreach (var conn in node.connections)
            {
                var go = new GameObject("Connection");
                var lr = go.AddComponent<LineRenderer>();
                lr.useWorldSpace = true;
                lr.positionCount = 2;
                lr.SetPosition(0, (Vector3)node.mapPosition);
                lr.SetPosition(1, (Vector3)conn.mapPosition);
                lr.startWidth = 0.03f;
                lr.endWidth = 0.03f;
                lr.material = GameConstants.SpriteMaterial;
                lr.startColor = new Color(1f, 1f, 1f, 0.15f);
                lr.endColor = new Color(1f, 1f, 1f, 0.15f);
                lr.sortingOrder = GameConstants.SortingOrder.Background;

                _lineObjects.Add(go);
            }
        }
    }

    void UpdateReachableHighlights()
    {
        var reachable = new HashSet<MapNode>(_map.GetReachableNodes());

        foreach (var kvp in _nodeRenderers)
        {
            var node = kvp.Key;
            var sr = kvp.Value;

            if (node.visited)
                sr.color = new Color(0.3f, 0.3f, 0.3f, 0.5f); // 방문한 노드 어둡게
            else if (reachable.Contains(node))
                sr.color = GetNodeColor(node.roomType); // 도달 가능: 원래 색상 밝게
            else
                sr.color = GetNodeColor(node.roomType) * 0.4f; // 도달 불가: 어둡게
        }
    }

    void ClearView()
    {
        foreach (var go in _nodeObjects) Destroy(go);
        foreach (var go in _lineObjects) Destroy(go);
        _nodeObjects.Clear();
        _lineObjects.Clear();
        _nodeRenderers.Clear();
        _nodeLabels.Clear();
        _active = false;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _active = true;
        UpdateReachableHighlights();

        if (CameraService.Instance != null && _map?.CurrentMap != null)
        {
            float centerY = _map.CurrentMap.Floors * 1.2f * 0.5f;
            float size = Mathf.Max(7f, _map.CurrentMap.Floors * 1.2f * 0.55f);
            float camZ = CameraService.Instance.Camera.transform.position.z;
            _viewScope = CameraService.Instance.PushTemporaryView(
                new Vector3(0, centerY, camZ), size);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _active = false;
        _viewScope?.Dispose();
        _viewScope = null;
    }

    static Color GetNodeColor(RoomType type)
    {
        switch (type)
        {
            case RoomType.Combat: return new Color(0.8f, 0.3f, 0.3f, 1f);
            case RoomType.Elite: return new Color(1f, 0.7f, 0.2f, 1f);
            case RoomType.Boss: return new Color(1f, 0.1f, 0.1f, 1f);
            case RoomType.Rest: return new Color(0.3f, 0.8f, 0.4f, 1f);
            case RoomType.Shop: return new Color(1f, 0.9f, 0.3f, 1f);
            case RoomType.Event: return new Color(0.7f, 0.7f, 0.9f, 1f);
            default: return Color.white;
        }
    }

    static string GetNodeSymbol(RoomType type)
    {
        switch (type)
        {
            case RoomType.Combat: return "전투";
            case RoomType.Elite: return "엘리트";
            case RoomType.Boss: return "보스";
            case RoomType.Rest: return "휴식";
            case RoomType.Shop: return "상점";
            case RoomType.Event: return "?";
            default: return "?";
        }
    }

    static float GetNodeScale(RoomType type)
    {
        return type == RoomType.Boss ? 0.5f : 0.3f;
    }
}
