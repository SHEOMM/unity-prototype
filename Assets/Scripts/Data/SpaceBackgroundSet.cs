using UnityEngine;

/// <summary>
/// 우주(천상) 배경 1장. 단일 스프라이트 + 메타데이터.
/// 현재 Map 씬 + Combat 천상 영역에 공유 사용. 향후 Map 전용 분리 예정.
///
/// 파이프라인: BackgroundSetGenerator (에디터)가 Assets/art/space_background/*.png 순회해 자동 생성.
/// </summary>
[CreateAssetMenu(fileName = "NewSpaceBackground", menuName = "Data/Space Background Set")]
public class SpaceBackgroundSet : ScriptableObject
{
    [Tooltip("식별 이름 (예: \"black\", \"blue\", \"red\"). 파일명에서 추출.")]
    public string setName;

    [Tooltip("배경 스프라이트 (타일러블 패턴 권장).")]
    public Sprite sprite;

    [Tooltip("렌더 시 적용할 색 틴트. 기본 흰색 = 원본 유지.")]
    public Color tint = Color.white;

    [Range(0f, 1f)]
    [Tooltip("향후 카메라 시차 계수 (0=정적, 1=카메라 완전 따라감). 현재는 미사용.")]
    public float parallaxFactor = 0f;
}
