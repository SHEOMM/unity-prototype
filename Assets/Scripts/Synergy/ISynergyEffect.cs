/// <summary>
/// 시너지 효과의 실행 전략. 두 가지 라이프사이클 훅을 제공.
///
/// OnHit: 발사체가 천체를 터치할 때마다 즉시 호출.
///   용도: Mercury "에너지 충전"처럼 충돌 순간 즉시 효과.
///
/// OnFlightEnd: 발사체 비행 종료 시 (에너지 소진/화면이탈/수동 종료) 호출.
///   용도: 속성 누적 시너지 / 시퀀스 기반 시너지 대부분 여기에 속함.
///
/// 기본 구현은 no-op이며, 상속자가 필요한 훅만 override한다 (SynergyEffectBase 사용 권장).
/// </summary>
public interface ISynergyEffect
{
    void OnHit(SynergyContext ctx);
    void OnFlightEnd(SynergyContext ctx);
}

/// <summary>
/// 편의 추상 베이스. 두 훅 모두 기본 no-op — 상속자가 필요한 것만 override.
/// </summary>
public abstract class SynergyEffectBase : ISynergyEffect
{
    public virtual void OnHit(SynergyContext ctx) { }
    public virtual void OnFlightEnd(SynergyContext ctx) { }
}
