using System;
using UnityEngine;

/// <summary>
/// 행성 ↔ 스프라이트 큐레이션 매핑 SO. Inspector에서 드래그로 자유 편집.
///
/// 편집 방법:
///   1) Inspector에서 `entries` 배열에 행성·스프라이트 엔트리 추가/교체
///   2) 메뉴 `Tools/Art/Apply Planet Icon Bindings` 실행 → 각 PlanetSO.icon에 반영
///
/// 초기 기본값은 `Tools/Art/Seed Planet Icon Bindings` 메뉴가 12개 행성에 대해
/// 추천 스프라이트 이름(`big_planet_001` 등)으로 채워준다.
///
/// 이 테이블은 큐레이션만 담당 — 스프라이트 풀(Library)과 해결(Resolver)은 별도.
/// </summary>
[CreateAssetMenu(fileName = "PlanetIconBindingTable", menuName = "Data/Planet Icon Binding Table")]
public class PlanetIconBindingTable : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public PlanetSO planet;
        public Sprite sprite;
    }

    [Tooltip("행성 → 스프라이트 매핑. 엔트리 순서·개수 자유. 동일 행성 중복 시 첫 항목 우선.")]
    public Entry[] entries;
}
