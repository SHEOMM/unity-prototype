/// <summary>둔화: 이동 속도 감소.</summary>
[StatusEffectId("slow")]
public class SlowEffect : IStatusEffect
{
    public float slowFactor = 0.4f;

    public void Tick(Enemy target, float dt)
    {
        target.moveSpeed *= (1f - slowFactor);
    }
}
