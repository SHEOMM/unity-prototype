/// <summary>
/// 고정 타임스텝 서브스테핑. 프레임 독립적 시뮬레이션을 보장한다.
/// 나선(spiral of death) 방지: 최대 서브스텝 수를 제한.
/// </summary>
public class FixedTimestepSimulator
{
    private float _accumulator;

    public void Accumulate(float deltaTime)
    {
        _accumulator += deltaTime;
        float maxAccumulated = GameConstants.ShipPhysics.FixedDt
                              * GameConstants.ShipPhysics.MaxSubSteps;
        if (_accumulator > maxAccumulated)
            _accumulator = maxAccumulated;
    }

    public bool ConsumeStep()
    {
        if (_accumulator < GameConstants.ShipPhysics.FixedDt)
            return false;
        _accumulator -= GameConstants.ShipPhysics.FixedDt;
        return true;
    }

    public void Reset() => _accumulator = 0f;
}
