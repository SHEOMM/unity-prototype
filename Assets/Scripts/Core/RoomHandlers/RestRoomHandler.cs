/// <summary>휴식 방 처리. RestScene을 로드한다.</summary>
public class RestRoomHandler : IRoomHandler
{
    public RoomType HandledType => RoomType.Rest;

    public void Enter(RoomType roomType)
    {
        SceneLoader.Instance.LoadScene("RestScene");
    }

    public void Exit() { }
}
