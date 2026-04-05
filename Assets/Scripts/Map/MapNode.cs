using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 맵의 한 노드. 층(floor)과 열(column)로 위치가 정해지고,
/// connections로 다음 층의 노드들과 연결된다.
/// </summary>
public class MapNode
{
    public int floor;
    public int column;
    public RoomType roomType;
    public List<MapNode> connections = new List<MapNode>();
    public bool visited;
    public bool active;         // 경로가 지나가는 노드인가
    public Vector2 mapPosition; // UI 표시용 좌표

    public MapNode(int floor, int column)
    {
        this.floor = floor;
        this.column = column;
    }
}
