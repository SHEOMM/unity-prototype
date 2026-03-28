/// <summary>
/// 별 효과 인터페이스. 모든 행성 고유 효과는 이것을 구현한다.
/// 새 별을 추가할 때 이 인터페이스만 구현하면 된다.
/// </summary>
public interface IStarEffect
{
    /// <summary>효과 실행. EffectContext에 결과를 누적한다.</summary>
    void Execute(EffectContext ctx);
}
