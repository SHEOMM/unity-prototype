using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// StS 스타일 분기형 맵 생성. 순수 로직 — MonoBehaviour 아님.
/// 경로 생성 → 노드 활성화 → 타입 배정 → 보스 연결.
/// </summary>
public static class MapGenerator
{
    public static MapData Generate(int floors, int columns, int pathCount, int seed)
    {
        var rng = new System.Random(seed);
        var map = new MapData(floors, columns);

        GeneratePaths(map, pathCount, rng);
        CollectActiveNodes(map);
        AssignRoomTypes(map, rng);
        CreateBossNode(map);
        AssignMapPositions(map);

        return map;
    }

    static void GeneratePaths(MapData map, int pathCount, System.Random rng)
    {
        for (int p = 0; p < pathCount; p++)
        {
            int col = rng.Next(0, map.Columns);
            map.Grid[0, col].active = true;

            for (int f = 0; f < map.Floors - 1; f++)
            {
                var from = map.Grid[f, col];
                int nextCol = PickNextColumn(col, map.Columns, from, map.Grid, f + 1, rng);
                var to = map.Grid[f + 1, nextCol];
                to.active = true;

                if (!from.connections.Contains(to))
                    from.connections.Add(to);

                col = nextCol;
            }
        }
    }

    static int PickNextColumn(int currentCol, int maxCols, MapNode from, MapNode[,] grid, int nextFloor, System.Random rng)
    {
        var candidates = new List<int>();
        for (int dc = -1; dc <= 1; dc++)
        {
            int nc = currentCol + dc;
            if (nc < 0 || nc >= maxCols) continue;

            // 경로 교차 방지: 같은 층의 다른 노드에서 이미 이 열로 가는 연결이 있는지 확인
            if (!WouldCross(from, grid, nextFloor, nc))
                candidates.Add(nc);
        }

        if (candidates.Count == 0)
            candidates.Add(currentCol); // fallback

        return candidates[rng.Next(candidates.Count)];
    }

    static bool WouldCross(MapNode from, MapNode[,] grid, int nextFloor, int nextCol)
    {
        int currentFloor = from.floor;
        int cols = grid.GetLength(1);

        for (int c = 0; c < cols; c++)
        {
            if (c == from.column) continue;
            var other = grid[currentFloor, c];
            foreach (var conn in other.connections)
            {
                if (conn.floor != nextFloor) continue;
                // 교차: from.col < other.col이면 nextCol > conn.col이어야 교차
                // 또는 from.col > other.col이면 nextCol < conn.col이어야 교차
                if (from.column < c && nextCol > conn.column) return true;
                if (from.column > c && nextCol < conn.column) return true;
            }
        }
        return false;
    }

    static void CollectActiveNodes(MapData map)
    {
        map.ActiveNodes.Clear();
        for (int f = 0; f < map.Floors; f++)
            for (int c = 0; c < map.Columns; c++)
                if (map.Grid[f, c].active)
                    map.ActiveNodes.Add(map.Grid[f, c]);
    }

    static void AssignRoomTypes(MapData map, System.Random rng)
    {
        foreach (var node in map.ActiveNodes)
        {
            // 고정층
            if (node.floor == 0)
            {
                node.roomType = RoomType.Combat;
                continue;
            }
            if (node.floor == map.Floors - 1)
            {
                node.roomType = RoomType.Rest; // 보스 전 휴식
                continue;
            }

            // 확률 배정
            node.roomType = RollRoomType(node, map, rng);
        }
    }

    static RoomType RollRoomType(MapNode node, MapData map, System.Random rng)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            float roll = (float)rng.NextDouble();
            RoomType type;

            if (roll < 0.55f) type = RoomType.Combat;
            else if (roll < 0.65f) type = RoomType.Elite;
            else if (roll < 0.77f) type = RoomType.Rest;
            else if (roll < 0.85f) type = RoomType.Shop;
            else type = RoomType.Event;

            // 연속 규칙: 엘리트/상점/휴식은 이전 층에서 같은 타입이면 불가
            if (IsSpecialType(type) && HasConsecutiveSpecial(node, type, map))
                continue;

            return type;
        }
        return RoomType.Combat; // fallback
    }

    static bool IsSpecialType(RoomType type)
        => type == RoomType.Elite || type == RoomType.Shop || type == RoomType.Rest;

    static bool HasConsecutiveSpecial(MapNode node, RoomType type, MapData map)
    {
        if (node.floor == 0) return false;

        // 이전 층에서 이 노드로 연결된 노드 중 같은 타입이 있는지
        for (int c = 0; c < map.Columns; c++)
        {
            var prev = map.Grid[node.floor - 1, c];
            if (!prev.active) continue;
            foreach (var conn in prev.connections)
            {
                if (conn == node && prev.roomType == type)
                    return true;
            }
        }
        return false;
    }

    static void CreateBossNode(MapData map)
    {
        var boss = new MapNode(map.Floors, map.Columns / 2)
        {
            active = true,
            roomType = RoomType.Boss,
            mapPosition = new Vector2(0, map.Floors + 0.5f)
        };
        map.BossNode = boss;

        // 마지막 층 모든 활성 노드 → 보스 연결
        for (int c = 0; c < map.Columns; c++)
        {
            var node = map.Grid[map.Floors - 1, c];
            if (node.active)
                node.connections.Add(boss);
        }
    }

    static void AssignMapPositions(MapData map)
    {
        float colSpacing = 2f;
        float floorSpacing = 1.2f;
        float offsetX = -(map.Columns - 1) * colSpacing * 0.5f;

        foreach (var node in map.ActiveNodes)
        {
            node.mapPosition = new Vector2(
                offsetX + node.column * colSpacing,
                node.floor * floorSpacing
            );
        }

        if (map.BossNode != null)
            map.BossNode.mapPosition = new Vector2(0, map.Floors * floorSpacing);
    }
}
