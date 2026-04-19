#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 슬라이싱된 Assets/art/planet/*.png 의 서브 에셋 Sprite들을 모아
/// PlanetSpriteLibrary 에셋의 티어별 배열에 채운다.
///
/// 라이브러리 에셋이 없으면 생성. 이름순 정렬로 인덱스 안정성 유지.
/// </summary>
public static class PlanetSpriteLibraryPopulator
{
    const string LibraryPath = "Assets/Data/PlanetSpriteLibrary.asset";

    static readonly (string path, PlanetTier tier)[] Sources =
    {
        ("Assets/art/planet/big_planet.png",        PlanetTier.Big),
        ("Assets/art/planet/medium_planet.png",     PlanetTier.Medium),
        ("Assets/art/planet/small_planet.png",      PlanetTier.Small),
        ("Assets/art/planet/very_small_planet.png", PlanetTier.VerySmall),
    };

    [MenuItem("Tools/Art/Populate Planet Sprite Library")]
    public static void Populate()
    {
        var lib = AssetDatabase.LoadAssetAtPath<PlanetSpriteLibrary>(LibraryPath);
        if (lib == null)
        {
            lib = ScriptableObject.CreateInstance<PlanetSpriteLibrary>();
            System.IO.Directory.CreateDirectory("Assets/Data");
            AssetDatabase.CreateAsset(lib, LibraryPath);
        }

        foreach (var (path, tier) in Sources)
        {
            var sprites = LoadSpritesAtPath(path);
            switch (tier)
            {
                case PlanetTier.Big:       lib.big = sprites; break;
                case PlanetTier.Medium:    lib.medium = sprites; break;
                case PlanetTier.Small:     lib.small = sprites; break;
                case PlanetTier.VerySmall: lib.verySmall = sprites; break;
            }
            Debug.Log($"[LibraryPopulator] {tier}: {sprites.Length}개 from {path}");
        }

        EditorUtility.SetDirty(lib);
        AssetDatabase.SaveAssets();
        Debug.Log($"[LibraryPopulator] 완료 — {LibraryPath}");
    }

    static Sprite[] LoadSpritesAtPath(string assetPath)
    {
        var all = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        var list = new List<Sprite>();
        foreach (var a in all)
            if (a is Sprite sp) list.Add(sp);
        list.Sort((x, y) => string.Compare(x.name, y.name, System.StringComparison.Ordinal));
        return list.ToArray();
    }
}
#endif
