/// <summary>
/// 들어오는 피해를 수정하는 보조 계약. IStatusEffect와 함께 구현해서
/// 특정 상태(예: Weakness — 받는 피해 증폭, Shield — 감소)를 표현한다.
///
/// Enemy/AllyUnit.TakeDamage가 활성 status를 순회하며 이 인터페이스를 가진
/// 것에 ModifyIncoming을 호출해 최종 dmg를 결정.
///
/// ISP 분리: 모든 status가 데미지 수정을 하지 않으므로 IStatusEffect 본체에
/// 합치지 않고 옵셔널하게 추가.
/// </summary>
public interface IDamageModifier
{
    float ModifyIncoming(float dmg, Element element);
}
