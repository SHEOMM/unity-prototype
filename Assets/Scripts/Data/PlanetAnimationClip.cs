using UnityEngine;

/// <summary>
/// 하나의 행성 애니메이션 클립 (시트 1장 = 클립 1개). 슬라이싱된 서브 스프라이트 배열 + 재생 메타데이터.
///
/// 런타임 사용: PlanetAnimator가 frames/fps 그대로 소비.
/// 정적 아이콘 경로(Cosmos 토큰, Reward 카드): Icon 프로퍼티 = frames[0].
///
/// 파이프라인:
///   planet2/*.png (슬라이싱됨) → PlanetAnimationClipGenerator (에디터) → 이 SO 자동 생성
/// </summary>
[CreateAssetMenu(fileName = "NewPlanetAnimation", menuName = "Data/Planet Animation Clip")]
public class PlanetAnimationClip : ScriptableObject
{
    [Tooltip("애니메이션 식별 이름 (예: \"earth\"). 시트 PNG 파일명과 일치.")]
    public string clipName;

    [Tooltip("재생할 프레임 목록. 순서대로 루프. 배열 공유(clip SO 소유)라 인스턴스별 중복 없음.")]
    public Sprite[] frames;

    [Range(1f, 30f)]
    [Tooltip("초당 프레임 수. 기본 10 — 행성 자전 애니에 충분하며 저부하.")]
    public float fps = 10f;

    /// <summary>정적 아이콘 경로용 대표 프레임 (frames[0]). Cosmos/Reward 정적 표시에 재사용.</summary>
    public Sprite Icon => frames != null && frames.Length > 0 ? frames[0] : null;
}
