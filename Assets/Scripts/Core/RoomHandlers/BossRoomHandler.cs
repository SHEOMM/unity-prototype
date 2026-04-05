/// <summary>보스 전투 방 처리. CombatScene을 보스 모드로 로드한다.</summary>
public class BossRoomHandler : IRoomHandler
{
    public RoomType HandledType => RoomType.Boss;

    public void Enter(RoomType roomType)
    {
        SceneLoader.Instance.LoadScene("CombatScene");
    }

    public void Exit() { }
}
