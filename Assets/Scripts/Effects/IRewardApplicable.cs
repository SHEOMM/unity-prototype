/// <summary>
/// 보상으로 적용 가능한 SO가 구현하는 인터페이스.
/// RewardManager에서 타입 분기 없이 다형적으로 보상을 적용한다.
/// </summary>
public interface IRewardApplicable
{
    void ApplyAsReward(PlayerState player, RunState run);
}
