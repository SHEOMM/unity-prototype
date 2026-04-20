using UnityEngine;

/// <summary>
/// 지상 배경 1 씬. 다층 parallax 레이어 또는 단일 합성본 지원.
///
/// layers[]는 뒤→앞 순서 (layer[0]=가장 먼 레이어, layer[last]=가장 앞). 각자 투명 배경 픽셀아트.
/// layers가 비어 있거나 길이 1이면 compositeFallback 1장으로 대체 (nature_8처럼 레이어 미분리 씬).
///
/// 파이프라인: BackgroundSetGenerator가 Assets/art/ground_background/nature_N/ 폴더 순회해 자동 생성.
/// </summary>
[CreateAssetMenu(fileName = "NewGroundBackground", menuName = "Data/Ground Background Set")]
public class GroundBackgroundSet : ScriptableObject
{
    [Tooltip("식별 이름 (예: \"nature_1\"). 폴더명과 일치.")]
    public string setName;

    [Tooltip("뒤→앞 순서의 parallax 레이어. layer[0]이 가장 먼 배경.")]
    public Sprite[] layers;

    [Tooltip("레이어별 시차 계수 (0=정적, 1=카메라 완전 따라감). 길이 = layers.Length. 현재는 미사용.")]
    public float[] parallaxFactors;

    [Tooltip("레이어 분리가 없는 씬(nature_8)의 단일 합성 이미지. layers가 비어있을 때 fallback.")]
    public Sprite compositeFallback;

    /// <summary>재생 가능한 레이어 배열을 반환. layers 없으면 compositeFallback 1장 폴백.</summary>
    public Sprite[] ResolveLayers()
    {
        if (layers != null && layers.Length > 0) return layers;
        if (compositeFallback != null) return new[] { compositeFallback };
        return System.Array.Empty<Sprite>();
    }
}
