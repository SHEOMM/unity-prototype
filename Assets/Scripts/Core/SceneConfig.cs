using UnityEngine;

/// <summary>
/// Boot 스크립트가 Inspector에서 받는 도메인별 설정 SO의 추상 베이스.
///
/// <para>이 프로젝트에서 GameConstants는 컴파일 타임 상수의 진실 원본이지만,
/// 디자이너가 Play 진입 없이 튜닝하고 싶은 값(보상 카드 간격, 전투 난이도 배수 등)은
/// SO로 노출하는 것이 효율적이다. SceneConfig 파생 SO를 Boot 스크립트의
/// <c>[SerializeField]</c>로 받아 Inspector에서 즉시 변경 가능하게 한다.</para>
///
/// <para>도메인별 파생 예시:
/// <list type="bullet">
/// <item><c>RewardSceneConfig</c> — CardSpacing, CardsY, TitleY 등</item>
/// <item><c>CombatSceneConfig</c> — 난이도 배수, 천체 배치 파라미터 등</item>
/// <item><c>MapSceneConfig</c> — 노드 크기, 폰트 등</item>
/// </list></para>
///
/// <para>각 파생 SO는 자체 <c>[CreateAssetMenu]</c> 어트리뷰트로 메뉴 등록 후
/// <c>Assets/Data/SceneConfigs/</c> 하위에 .asset 인스턴스로 보관.</para>
///
/// <para><b>주의</b>: SceneConfig 값이 GameConstants와 분기될 수 있다. 디자이너 튜닝이
/// 의도라면 SceneConfig가 우선 (Inspector 편집을 의도하므로). 코드 상수는 Phase 0
/// 마이그레이션 시점의 시작값으로만 사용.</para>
/// </summary>
public abstract class SceneConfig : ScriptableObject
{
}
