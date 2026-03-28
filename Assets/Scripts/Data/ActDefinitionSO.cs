using UnityEngine;

[CreateAssetMenu(fileName = "NewAct", menuName = "Map/Act")]
public class ActDefinitionSO : ScriptableObject
{
    public string actName;
    public int floorCount = 7;
    public RoomDefinitionSO bossRoom;
    public RoomDefinitionSO[] possibleCombatRooms;
    public RoomDefinitionSO[] possibleEliteRooms;
    public RoomDefinitionSO[] possibleEventRooms;
    [Range(0f, 1f)] public float eliteChance = 0.15f;
    [Range(0f, 1f)] public float eventChance = 0.2f;
    [Range(0f, 1f)] public float restChance = 0.12f;
    [Range(0f, 1f)] public float shopChance = 0.1f;
}
