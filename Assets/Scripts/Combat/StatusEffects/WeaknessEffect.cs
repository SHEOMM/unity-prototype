/// <summary>
/// 약화: 받는 피해를 amplifier 배율로 증폭 (기본 1.3 = 130%).
/// IDamageModifier를 구현하여 Enemy/AllyUnit.TakeDamage가 활성 status 순회 시
/// 이 인스턴스를 찾아 ModifyIncoming을 호출하고 결과를 반영.
/// </summary>
[StatusEffectId("weakness")]
public class WeaknessEffect : IStatusEffect, IDamageModifier
{
    public float amplifier = 1.3f;

    public void Tick(IStatusHost target, float dt) { /* no-op — 수정은 ModifyIncoming에서 */ }

    public float ModifyIncoming(float dmg, Element element)
    {
        return dmg * amplifier;
    }
}
