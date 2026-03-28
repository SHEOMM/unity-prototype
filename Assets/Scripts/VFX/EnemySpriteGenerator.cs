using UnityEngine;

/// <summary>
/// EnemyShape별 고유 스프라이트를 프로시저럴로 생성하는 유틸리티.
/// </summary>
public static class EnemySpriteGenerator
{
    public static Sprite GenerateEnemySprite(EnemySO data, int size = 64)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float half = size * 0.5f;
        float radius = half - 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - half;
                float dy = y - half;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float norm = dist / radius;

                float mask = GetShapeMask(data.shape, dx, dy, radius);
                if (mask <= 0f)
                {
                    tex.SetPixel(x, y, Color.clear);
                    continue;
                }

                float glow = Mathf.Clamp01(1f - norm * 0.8f);
                float edgeSoft = Mathf.Clamp01(mask * 5f);

                Color c = Color.Lerp(data.bodyColor * 0.4f, data.bodyColor, glow);
                c = Color.Lerp(c, Color.white, Mathf.Pow(Mathf.Max(0, 1f - norm * 1.5f), 3f) * 0.3f);

                // 속성 패턴 오버레이
                if (data.element != Element.None)
                {
                    float pattern = GetElementOverlay(data.element, dx, dy, norm);
                    c = Color.Lerp(c, Color.white * 0.8f, pattern * 0.2f);
                }

                c.a = edgeSoft;
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    static float GetShapeMask(EnemyShape shape, float dx, float dy, float r)
    {
        float norm = Mathf.Sqrt(dx * dx + dy * dy) / r;

        switch (shape)
        {
            case EnemyShape.Square:
            {
                float d = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) / r;
                return 1f - d;
            }
            case EnemyShape.Hexagon:
            {
                float ax = Mathf.Abs(dx) / r;
                float ay = Mathf.Abs(dy) / r;
                float d = Mathf.Max(ax, (ax * 0.5f + ay * 0.866f));
                return 1f - d;
            }
            case EnemyShape.Triangle:
            {
                float ty = (dy / r + 1f) * 0.5f; // 0 at bottom, 1 at top
                float halfW = (1f - ty) * 0.8f;
                float tx = Mathf.Abs(dx) / r;
                if (ty < 0f || ty > 1f || tx > halfW) return 0f;
                return Mathf.Min(ty * 2f, (halfW - tx) * 3f, (1f - ty) * 2f);
            }
            case EnemyShape.Circle:
                return 1f - norm;
            case EnemyShape.Diamond:
            {
                float d = (Mathf.Abs(dx) + Mathf.Abs(dy)) / r;
                return 1f - d;
            }
            case EnemyShape.Blob:
            {
                float angle = Mathf.Atan2(dy, dx);
                float wobble = 1f + Mathf.Sin(angle * 5f) * 0.15f + Mathf.Sin(angle * 3f) * 0.1f;
                return 1f - norm / wobble;
            }
            case EnemyShape.Crystal:
            {
                float angle = Mathf.Atan2(dy, dx);
                float sides = Mathf.Cos(angle * 4f) * 0.15f + 0.85f;
                return 1f - norm / sides;
            }
            case EnemyShape.Crown:
            {
                float angle = Mathf.Atan2(dy, dx);
                float spikes = Mathf.Max(0, Mathf.Sin(angle * 5f)) * 0.3f;
                float baseShape = 1f - norm;
                // 윗부분에만 스파이크
                if (dy > 0) baseShape += spikes * (1f - norm);
                return baseShape;
            }
            default:
                return 1f - norm;
        }
    }

    static float GetElementOverlay(Element element, float dx, float dy, float norm)
    {
        switch (element)
        {
            case Element.Fire:
                float angle = Mathf.Atan2(dy, dx);
                return Mathf.Max(0, Mathf.Sin(angle * 6f)) * (1f - norm) * 0.6f;
            case Element.Water:
                return Mathf.Max(0, Mathf.Sin(norm * Mathf.PI * 5f)) * (1f - norm) * 0.5f;
            case Element.Wind:
                float a = Mathf.Atan2(dy, dx);
                return Mathf.Max(0, Mathf.Sin(a * 2f + norm * 8f)) * (1f - norm) * 0.5f;
            case Element.Earth:
                return Mathf.Abs(Mathf.Sin(dx * 0.4f) * Mathf.Cos(dy * 0.4f)) * (1f - norm) * 0.4f;
            case Element.Darkness:
                return norm * norm * 0.5f;
            default:
                return 0f;
        }
    }
}
