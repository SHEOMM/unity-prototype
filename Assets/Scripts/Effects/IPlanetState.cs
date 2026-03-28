/// <summary>
/// 행성의 프레임 간 지속 상태 인터페이스.
/// MonoBehaviour로 구현하여 PlanetBody의 GameObject에 부착된다.
/// 효과(IStarEffect)는 무상태 전략으로 유지하고, 상태는 이 인터페이스로 분리한다.
/// </summary>
public interface IPlanetState
{
    void Tick(float deltaTime);
}
