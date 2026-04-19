/// <summary>
/// 호스트(IStatusHost)에 적용된 활성 상태이상 인스턴스.
/// IStatusEffect 전략과 지속시간을 묶어서 관리한다.
/// </summary>
public class StatusEffect
{
    public IStatusEffect effect;
    public float duration;
    private float _elapsed;
    private bool _applied;

    public bool IsExpired => _elapsed >= duration;

    public StatusEffect(IStatusEffect effect, float duration)
    {
        this.effect = effect;
        this.duration = duration;
    }

    public void Tick(IStatusHost target, float dt)
    {
        if (!_applied)
        {
            _applied = true;
            effect.OnApplied(target);
        }
        _elapsed += dt;
        effect.Tick(target, dt);
        if (IsExpired) effect.OnExpired(target);
    }
}
