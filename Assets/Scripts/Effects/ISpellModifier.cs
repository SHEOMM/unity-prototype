using System.Collections.Generic;

/// <summary>
/// Spell 히트리스트를 전처리 단계에서 변경할 수 있는 효과용 인터페이스.
/// IStarEffect와 함께 구현한다. SpellResolver가 효과 실행 전에 호출한다.
/// 예: 장군별 — 근처의 비포함 별을 징집하여 히트리스트에 삽입.
/// </summary>
public interface ISpellModifier
{
    List<PlanetBody> ModifyHitList(List<PlanetBody> currentHits, int sourceIndex, PlanetBody source);
}
