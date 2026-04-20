using UnityEngine;

/// <summary>
/// 생성된 모든 PlanetAnimationClip 카탈로그. Inspector 미리보기·에디터 도구 iterate용.
/// PlanetAnimationClipGenerator가 자동 갱신한다.
///
/// 매핑(Planet↔Clip)은 PlanetAnimationBindingTable이 담당 — 관심사 분리.
/// </summary>
[CreateAssetMenu(fileName = "PlanetAnimationLibrary", menuName = "Data/Planet Animation Library")]
public class PlanetAnimationLibrary : ScriptableObject
{
    [Tooltip("등록된 모든 클립. 이름순.")]
    public PlanetAnimationClip[] clips;

    public PlanetAnimationClip FindByName(string clipName)
    {
        if (string.IsNullOrEmpty(clipName) || clips == null) return null;
        foreach (var c in clips)
            if (c != null && c.clipName == clipName) return c;
        return null;
    }
}
