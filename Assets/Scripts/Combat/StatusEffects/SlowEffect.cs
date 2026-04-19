/// <summary>
/// 둔화: 이동 속도 감소. IMoveable 구현 타겟에만 적용 (Structure 같은 non-IMoveable은 무시).
/// Enemy.Update의 moveSpeed 리셋 흐름과 맞물려 작동 (매 프레임 리셋 → SlowEffect.Tick에서 감속).
/// </summary>
[StatusEffectId("slow")]
public class SlowEffect : IStatusEffect
{
    public float slowFactor = 0.4f;

    public void Tick(IStatusHost target, float dt)
    {
        if (target is Enemy e)
        {
            // 현재 Enemy.moveSpeed는 매 프레임 _baseSpeed로 리셋되므로 여기서 배율 적용.
            e.moveSpeed *= (1f - slowFactor);
        }
        // TODO: AllyUnit도 비슷한 패턴. Phase 5에서 필요 시 확장.
    }
}
