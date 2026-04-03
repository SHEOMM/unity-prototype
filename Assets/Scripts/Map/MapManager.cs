using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 분기형 맵 관리. 맵 생성, 노드 선택, 도달 가능 노드 계산.
/// MapScene 전용 — Combat/Shop/Rest를 모른다.
/// </summary>
public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public MapData CurrentMap { get; private set; }
    public MapNode CurrentNode { get; private set; }

    public System.Action<MapData> OnMapGenerated;
    public System.Action<MapNode> OnNodeSelected;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void GenerateMap(int floors, int columns, int pathCount, int seed)
    {
        CurrentMap = MapGenerator.Generate(floors, columns, pathCount, seed);
        CurrentNode = null;
        Debug.Log($"[Map] 맵 생성 완료: {floors}층 × {columns}열, 활성 노드 {CurrentMap.ActiveNodes.Count}개");
        OnMapGenerated?.Invoke(CurrentMap);
    }

    public List<MapNode> GetReachableNodes()
    {
        var reachable = new List<MapNode>();

        if (CurrentNode == null)
        {
            // 아직 시작 안 함 → 1층 활성 노드 전부
            for (int c = 0; c < CurrentMap.Columns; c++)
            {
                var node = CurrentMap.Grid[0, c];
                if (node.active) reachable.Add(node);
            }
        }
        else
        {
            // 현재 노드에서 연결된 다음 노드
            foreach (var conn in CurrentNode.connections)
                reachable.Add(conn);
        }

        return reachable;
    }

    public void SelectNode(MapNode node)
    {
        node.visited = true;
        CurrentNode = node;
        Debug.Log($"[Map] 노드 선택: {node.roomType} (층 {node.floor}, 열 {node.column})");
        OnNodeSelected?.Invoke(node);
    }

    public bool HasNextNodes()
    {
        if (CurrentNode == null) return CurrentMap?.ActiveNodes.Count > 0;
        return CurrentNode.connections.Count > 0;
    }
}
