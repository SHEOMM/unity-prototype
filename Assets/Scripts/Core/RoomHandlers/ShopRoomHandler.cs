/// <summary>상점 방 처리. ShopScene을 로드한다.</summary>
public class ShopRoomHandler : IRoomHandler
{
    public RoomType HandledType => RoomType.Shop;

    public void Enter(RoomType roomType)
    {
        SceneLoader.Instance.LoadScene("ShopScene");
    }

    public void Exit() { }
}
