using UnityEngine;

/// <summary>
/// 캐릭터 애니메이션 한 상태의 프레임 + 재생 메타. PlanetAnimationClip과 유사하나
/// 캐릭터 상태 머신 용도라 loop 플래그를 명시한다 (oneshot 상태 지원).
///
/// frames[]는 Set SO 공유 참조 → 런타임 인스턴스당 메모리 중복 없음.
/// </summary>
[CreateAssetMenu(fileName = "NewCharacterClip", menuName = "Data/Character Animation Clip")]
public class CharacterAnimationClip : ScriptableObject
{
    [Tooltip("애니메이션 식별 이름 (예: \"B_witch_idle\").")]
    public string clipName;

    [Tooltip("재생할 프레임 목록 (자연수 정렬됨).")]
    public Sprite[] frames;

    [Range(1f, 30f)]
    [Tooltip("초당 프레임 수. 기본 10.")]
    public float fps = 10f;

    [Tooltip("무한 루프 여부. false면 마지막 프레임에서 멈춤.")]
    public bool loop = true;

    /// <summary>정적 대표 프레임 (frames[0]).</summary>
    public Sprite Icon => frames != null && frames.Length > 0 ? frames[0] : null;

    /// <summary>전체 재생 시간 (초). fps·frame 수 기반.</summary>
    public float Duration => frames != null && frames.Length > 0 ? frames.Length / Mathf.Max(0.1f, fps) : 0f;
}
