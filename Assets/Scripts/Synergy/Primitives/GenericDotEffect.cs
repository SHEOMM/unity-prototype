/// <summary>
/// 범용 DoT IStatusEffect 구현체. DotApplicator가 런타임에 파라미터를 주입해 사용한다.
/// (Registry를 거치지 않고 직접 생성.) PoisonEffect는 독 속성 전용이고,
/// 이 클래스는 임의 데미지/인터벌/속성 DoT를 표현한다.
/// </summary>
public class GenericDotEffect : IStatusEffect
{
    private readonly float _damagePerTick;
    private readonly float _tickInterval;
    private readonly Element _element;
    private float _elapsed;

    public GenericDotEffect(float damagePerTick, float tickInterval, Element element)
    {
        _damagePerTick = damagePerTick;
        _tickInterval = tickInterval;
        _element = element;
    }

    public void OnApplied(IStatusHost target) { _elapsed = 0f; }

    public void Tick(IStatusHost target, float dt)
    {
        _elapsed += dt;
        if (_elapsed >= _tickInterval)
        {
            _elapsed -= _tickInterval;
            if (target != null) target.TakeDamage(_damagePerTick, _element);
        }
    }

    public void OnExpired(IStatusHost target) { }
}
