using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 행성 스프라이트 티어별 카탈로그. 슬라이싱된 서브 에셋 Sprite들을 모아둔다.
/// PlanetSpriteLibraryPopulator(Editor)가 자동으로 채움. 추후 희귀도 뽑기 등에서 재사용.
///
/// 이 라이브러리는 "어떤 스프라이트가 존재하는가"만 알고, 각 행성에 어떤 스프라이트를
/// 쓸지는 PlanetIconBindingTable이 별도로 관리 (관심사 분리).
/// </summary>
[CreateAssetMenu(fileName = "PlanetSpriteLibrary", menuName = "Data/Planet Sprite Library")]
public class PlanetSpriteLibrary : ScriptableObject
{
    [Header("티어별 스프라이트 풀")]
    public Sprite[] big;
    public Sprite[] medium;
    public Sprite[] small;
    public Sprite[] verySmall;

    public IReadOnlyList<Sprite> GetTier(PlanetTier tier)
    {
        switch (tier)
        {
            case PlanetTier.Big:       return big ?? System.Array.Empty<Sprite>();
            case PlanetTier.Medium:    return medium ?? System.Array.Empty<Sprite>();
            case PlanetTier.Small:     return small ?? System.Array.Empty<Sprite>();
            case PlanetTier.VerySmall: return verySmall ?? System.Array.Empty<Sprite>();
            default: return System.Array.Empty<Sprite>();
        }
    }

    public Sprite FindByName(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return null;
        foreach (var arr in new[] { big, medium, small, verySmall })
        {
            if (arr == null) continue;
            foreach (var s in arr)
                if (s != null && s.name == spriteName) return s;
        }
        return null;
    }
}

public enum PlanetTier { Big, Medium, Small, VerySmall }
