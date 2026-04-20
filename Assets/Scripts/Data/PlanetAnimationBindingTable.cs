using System;
using UnityEngine;

/// <summary>
/// 행성 ↔ 애니메이션 클립 큐레이션 매핑 SO. Inspector 편집 가능.
///
/// 편집 방법:
///   1) Inspector에서 entries 배열에 행성·클립 엔트리 추가/교체
///   2) 메뉴 Tools/Art/Apply Planet Animation Bindings 실행
///      → 각 PlanetSO.icon에 clip.Icon(첫 프레임) 주입 (정적 경로 호환)
///
/// 런타임:
///   GameManager.Awake가 PlanetSpriteResolver.BindingTable로 주입 → PlanetBody가 ResolveClip 조회.
/// </summary>
[CreateAssetMenu(fileName = "PlanetAnimationBindingTable", menuName = "Data/Planet Animation Binding Table")]
public class PlanetAnimationBindingTable : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public PlanetSO planet;
        public PlanetAnimationClip clip;
    }

    [Tooltip("행성 → 애니메이션 매핑. 동일 행성 중복 시 첫 항목 우선.")]
    public Entry[] entries;

    public PlanetAnimationClip Find(PlanetSO planet)
    {
        if (planet == null || entries == null) return null;
        foreach (var e in entries)
            if (e.planet == planet && e.clip != null) return e.clip;
        return null;
    }
}
