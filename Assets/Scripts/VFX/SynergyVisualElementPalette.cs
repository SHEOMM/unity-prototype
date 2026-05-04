using UnityEngine;

/// <summary>
/// Element → Color 매핑. 모든 시너지 비주얼이 공유해 색 중복을 제거.
/// rule.element가 None일 때 fallback gold 반환 (시너지 디폴트 톤).
/// </summary>
public static class SynergyVisualElementPalette
{
    public static Color Resolve(Element e)
    {
        switch (e)
        {
            case Element.Fire:     return GameConstants.Colors.ElementFire;
            case Element.Water:    return GameConstants.Colors.ElementWater;
            case Element.Wind:     return GameConstants.Colors.ElementWind;
            case Element.Earth:    return GameConstants.Colors.ElementEarth;
            case Element.Darkness: return GameConstants.Colors.ElementDarkness;
            default:               return GameConstants.Colors.ElementDefault;
        }
    }
}
