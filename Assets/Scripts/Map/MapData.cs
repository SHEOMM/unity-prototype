using System.Collections.Generic;

/// <summary>
/// 맵 전체 데이터. 그리드 + 활성 노드 + 보스 노드.
/// </summary>
public class MapData
{
    public int Floors { get; }
    public int Columns { get; }
    public MapNode[,] Grid { get; }
    public List<MapNode> ActiveNodes { get; } = new List<MapNode>();
    public MapNode BossNode { get; set; }

    public MapData(int floors, int columns)
    {
        Floors = floors;
        Columns = columns;
        Grid = new MapNode[floors, columns];

        for (int f = 0; f < floors; f++)
            for (int c = 0; c < columns; c++)
                Grid[f, c] = new MapNode(f, c);
    }

    public MapNode GetNode(int floor, int column)
    {
        if (floor < 0 || floor >= Floors || column < 0 || column >= Columns)
            return null;
        return Grid[floor, column];
    }
}
