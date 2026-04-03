/// <summary>
/// 방 타입별 진입/종료를 처리하는 인터페이스.
/// 새 방 타입 추가 = IRoomHandler 구현 1개 + 씬 1개. 기존 코드 수정 불필요.
/// </summary>
public interface IRoomHandler
{
    RoomType HandledType { get; }
    void Enter(RoomType roomType);
    void Exit();
}
