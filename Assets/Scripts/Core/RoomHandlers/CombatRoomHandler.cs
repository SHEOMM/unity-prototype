/// <summary>일반 전투 방 처리. CombatScene을 로드한다.</summary>
public class CombatRoomHandler : IRoomHandler
{
    public RoomType HandledType => RoomType.Combat;

    public void Enter(RoomType roomType)
    {
        SceneLoader.Instance.LoadScene("CombatScene");
    }

    public void Exit()
    {
        // CombatScene은 씬 언로드 시 자동 정리
    }
}
