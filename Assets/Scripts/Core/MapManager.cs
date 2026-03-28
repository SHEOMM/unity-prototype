using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Slay the Spire 스타일 지도 관리. 프로토타입은 선형 7방.
/// </summary>
public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public System.Action<RoomDefinitionSO> OnRoomSelected;

    private List<RoomDefinitionSO> _rooms = new List<RoomDefinitionSO>();
    private int _currentIndex = -1;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void GenerateMap(ActDefinitionSO act)
    {
        _rooms.Clear();
        _currentIndex = -1;

        if (act == null) return;

        var rng = new System.Random(RunState.Instance?.runSeed ?? 0);

        for (int i = 0; i < act.floorCount - 1; i++)
        {
            float roll = (float)rng.NextDouble();
            RoomDefinitionSO room;

            if (roll < act.eliteChance && act.possibleEliteRooms?.Length > 0)
                room = act.possibleEliteRooms[rng.Next(act.possibleEliteRooms.Length)];
            else if (roll < act.eliteChance + act.eventChance && act.possibleEventRooms?.Length > 0)
                room = act.possibleEventRooms[rng.Next(act.possibleEventRooms.Length)];
            else if (act.possibleCombatRooms?.Length > 0)
                room = act.possibleCombatRooms[rng.Next(act.possibleCombatRooms.Length)];
            else
                room = null;

            if (room != null) _rooms.Add(room);
        }

        if (act.bossRoom != null) _rooms.Add(act.bossRoom);

        Debug.Log($"[지도] {act.actName} 생성 완료: {_rooms.Count}개 방");
    }

    public void ShowMap()
    {
        // 프로토타입: 자동으로 다음 방 선택
        _currentIndex++;
        if (_currentIndex < _rooms.Count)
        {
            var room = _rooms[_currentIndex];
            Debug.Log($"[지도] {_currentIndex + 1}층: {room.roomName} ({room.roomType})");
            RunState.Instance?.AdvanceFloor();
            OnRoomSelected?.Invoke(room);
        }
    }

    public bool HasNextRoom() => _currentIndex + 1 < _rooms.Count;
    public int CurrentFloor => _currentIndex + 1;
    public int TotalFloors => _rooms.Count;
}
