using System.Collections;

/// <summary>
/// 시너지 비주얼 전략 인터페이스. ISpellVisual과 대칭되는 시너지 전용 레이어.
/// 새 시너지 비주얼은 이 인터페이스를 구현하고 [SynergyVisualId]를 붙이면
/// SynergyVisualRegistry가 리플렉션으로 자동 수집한다.
/// </summary>
public interface ISynergyVisual
{
    IEnumerator Play(SynergyVisualContext ctx);
}
