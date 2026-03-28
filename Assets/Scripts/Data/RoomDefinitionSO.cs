using UnityEngine;

[CreateAssetMenu(fileName = "NewRoom", menuName = "Map/Room")]
public class RoomDefinitionSO : ScriptableObject
{
    public string roomName;
    public RoomType roomType;
    public WaveDefinitionSO[] waveDefs;
    public ScriptableObject[] rewardPool;
}

public enum RoomType
{
    Combat,
    Elite,
    Boss,
    Shop,
    Event,
    Rest
}
