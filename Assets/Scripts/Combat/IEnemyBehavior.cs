/// <summary>
/// 적 행동 전략 인터페이스. IStarEffect에 대응.
/// 새 적 유형은 이 인터페이스를 구현하고 [EnemyBehaviorId]를 붙이면 된다.
/// </summary>
public interface IEnemyBehavior
{
    /// <summary>매 프레임 호출. false 반환 시 기본 이동을 스킵한다.</summary>
    bool Tick(Enemy enemy, float deltaTime);

    /// <summary>데미지를 가공한다 (방어막 흡수, 감소 등). 가공된 데미지를 반환.</summary>
    float ModifyIncomingDamage(Enemy enemy, float rawDamage, Element element);

    /// <summary>HP가 0 이하일 때 호출. false 반환 시 기본 사망 처리를 스킵한다.</summary>
    bool OnDeath(Enemy enemy);
}
