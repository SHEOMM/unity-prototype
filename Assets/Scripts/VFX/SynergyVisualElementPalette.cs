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
            case Element.Fire:     return new Color(1f, 0.45f, 0.18f, 1f);
            case Element.Water:    return new Color(0.3f, 0.65f, 1f, 1f);
            case Element.Wind:     return new Color(0.55f, 0.95f, 0.95f, 1f);
            case Element.Earth:    return new Color(0.65f, 0.45f, 0.22f, 1f);
            case Element.Darkness: return new Color(0.6f, 0.3f, 0.85f, 1f);
            default:               return new Color(1f, 0.85f, 0.3f, 1f); // None=gold
        }
    }
}
