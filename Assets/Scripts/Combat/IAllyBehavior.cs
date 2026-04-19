/// <summary>
/// 아군 유닛 전술 전략. 매 프레임 Tick으로 이동/공격 결정.
/// Tick이 true 반환 시 AllyUnit의 기본 이동 로직이 실행됨.
/// </summary>
public interface IAllyBehavior
{
    /// <summary>프레임 로직. 기본 이동/공격을 허용하면 true.</summary>
    bool Tick(AllyUnit ally, float deltaTime);

    /// <summary>들어오는 피해 보정. 기본 dmg 그대로 반환.</summary>
    float ModifyIncomingDamage(AllyUnit ally, float dmg, Element element);

    /// <summary>사망 시. false 반환 시 일반 사망 처리 스킵 (부활/분할 등).</summary>
    bool OnDeath(AllyUnit ally);
}
