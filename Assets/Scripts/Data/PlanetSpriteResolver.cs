using UnityEngine;

/// <summary>
/// 행성 렌더용 Sprite·AnimationClip 해결자. PlanetAnimationBindingTable을 런타임에 조회해
/// 애니메이션 클립을 반환하거나, 클립 없음 시 정적 대표 아이콘(clip.Icon) → PlanetSO.icon → 절차 폴백 순으로 해결.
///
/// BindingTable은 GameManager.Awake가 정적 속성으로 주입한다 (PersistentScene Inspector에서 바인딩).
///
/// 호출자:
///  - PlanetBody.Initialize → ResolveClip (애니메이션)
///  - DeckManager.CreatePlanet, CosmosPlanetToken, RewardManager.MakePlanetChoice → Resolve (정적)
/// </summary>
public static class PlanetSpriteResolver
{
    /// <summary>GameManager.Awake가 주입. 전투/씬 전환 동안 영속.</summary>
    public static PlanetAnimationBindingTable BindingTable { get; set; }

    /// <summary>정적 대표 스프라이트. 카드·토큰·리워드 등 단일 이미지 사용처용.</summary>
    public static Sprite Resolve(PlanetSO so)
    {
        if (so == null) return null;
        var clip = ResolveClip(so);
        if (clip != null && clip.Icon != null) return clip.Icon;
        if (so.icon != null) return so.icon;
        return CelestialSpriteGenerator.GeneratePlanetSprite(so.element, so.bodyColor);
    }

    /// <summary>애니메이션 클립. 없으면 null (정적 fallback 경로로 진행).</summary>
    public static PlanetAnimationClip ResolveClip(PlanetSO so)
    {
        if (so == null || BindingTable == null) return null;
        return BindingTable.Find(so);
    }
}
