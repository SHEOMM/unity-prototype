/// <summary>독: 일정 간격으로 지속 데미지.</summary>
[StatusEffectId("poison")]
public class PoisonEffect : IStatusEffect
{
    private float _tickTimer;

    public float tickInterval = 0.5f;
    public float damagePerTick = 5f;

    public void Tick(IStatusHost target, float dt)
    {
        if (target == null) return;
        _tickTimer += dt;
        if (_tickTimer >= tickInterval)
        {
            _tickTimer = 0f;
            target.TakeDamage(damagePerTick);
        }
    }
}
