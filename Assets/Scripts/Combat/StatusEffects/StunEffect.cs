/// <summary>
/// 기절: moveSpeed를 0으로 만들어 이동/공격 불가 상태.
/// Enemy.Update가 매 프레임 moveSpeed를 _baseSpeed로 리셋하므로, Tick에서 0으로 덮어씀.
/// AllyUnit.DefaultTick은 moveSpeed ≤ 0 시 공격도 스킵.
/// 만료 시 자동으로 moveSpeed 리셋 흐름에 의해 복원됨.
/// </summary>
[StatusEffectId("stun")]
public class StunEffect : IStatusEffect
{
    public string IconId => "stun";

    public void Tick(IStatusHost target, float dt)
    {
        if (target is Enemy e) e.moveSpeed = 0f;
        else if (target is AllyUnit a) a.moveSpeed = 0f;
    }
}
