/// <summary>
/// 적의 프레임 간 지속 상태 인터페이스. IPlanetState에 대응.
/// MonoBehaviour로 구현하여 Enemy의 GameObject에 부착된다.
/// </summary>
public interface IEnemyState
{
    void Tick(float deltaTime);
}
