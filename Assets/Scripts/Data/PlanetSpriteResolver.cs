using UnityEngine;

/// <summary>
/// 행성 렌더용 Sprite 해결자. PlanetSO.icon이 있으면 픽셀 아트 사용,
/// 없으면 CelestialSpriteGenerator로 절차 스프라이트 생성(폴백).
///
/// 현재 유일 호출자: DeckManager.CreatePlanet.
/// 픽셀 아트 미바인딩 행성이 생겨도 기존 절차 렌더로 무결성 유지.
/// </summary>
public static class PlanetSpriteResolver
{
    public static Sprite Resolve(PlanetSO so)
    {
        if (so == null) return null;
        if (so.icon != null) return so.icon;
        return CelestialSpriteGenerator.GeneratePlanetSprite(so.element, so.bodyColor);
    }
}
