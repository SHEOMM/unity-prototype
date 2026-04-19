#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Assets/art/planet/*.png 4개 스프라이트 시트를 Grid 방식으로 자동 슬라이싱한다.
/// TextureImporter.spritesheet을 채우고 SaveAndReimport로 서브 에셋 Sprite들을 생성.
///
/// 셀 크기가 어긋나면 Cells 상수를 조정해 재실행. 재실행 시 기존 sprites는 덮어씌워진다.
/// </summary>
public static class PlanetSpriteSheetSlicer
{
    struct Cells { public int w; public int h; }

    static readonly Dictionary<string, Cells> Grids = new Dictionary<string, Cells>
    {
        { "Assets/art/planet/big_planet.png",        new Cells { w = 48, h = 48 } },
        { "Assets/art/planet/medium_planet.png",     new Cells { w = 40, h = 40 } },
        { "Assets/art/planet/small_planet.png",      new Cells { w = 24, h = 24 } },
        { "Assets/art/planet/very_small_planet.png", new Cells { w = 34, h = 34 } },
    };

    [MenuItem("Tools/Art/Slice Planet Sheets")]
    public static void SliceAll()
    {
        int total = 0;
        foreach (var kv in Grids)
            total += Slice(kv.Key, kv.Value.w, kv.Value.h);
        Debug.Log($"[PlanetSlicer] 슬라이싱 완료: {total}개 스프라이트 ({Grids.Count}개 시트)");
    }

    static int Slice(string assetPath, int cellW, int cellH)
    {
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null) { Debug.LogError($"[PlanetSlicer] TextureImporter 못 찾음: {assetPath}"); return 0; }

        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (tex == null) { Debug.LogError($"[PlanetSlicer] Texture2D 로드 실패: {assetPath}"); return 0; }

        int cols = tex.width / cellW;
        int rows = tex.height / cellH;
        if (cols <= 0 || rows <= 0)
        {
            Debug.LogError($"[PlanetSlicer] 셀 크기 비정상: {assetPath} ({tex.width}x{tex.height} / {cellW}x{cellH})");
            return 0;
        }

        string baseName = Path.GetFileNameWithoutExtension(assetPath);
        var metas = new List<SpriteMetaData>(cols * rows);

        // Unity 스프라이트 rect는 좌하단 원점. 시트를 좌상→우하로 읽도록 y를 뒤집어 이름 매긴다.
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int indexTopLeft = r * cols + c;              // 화면 기준 좌상→우하 인덱스
                int yPixel = tex.height - (r + 1) * cellH;    // Unity 좌표 (bottom-left)
                metas.Add(new SpriteMetaData
                {
                    name = $"{baseName}_{indexTopLeft:D3}",
                    rect = new Rect(c * cellW, yPixel, cellW, cellH),
                    pivot = new Vector2(0.5f, 0.5f),
                    alignment = (int)SpriteAlignment.Center,
                    border = Vector4.zero,
                });
            }
        }

        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritesheet = metas.ToArray();
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        Debug.Log($"[PlanetSlicer] {baseName}: {metas.Count}개 ({cols}x{rows})");
        return metas.Count;
    }
}
#endif
