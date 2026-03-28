using UnityEngine;

/// <summary>
/// 천체별 고유 스프라이트를 프로시저럴로 생성하는 유틸리티.
/// Element에 따라 다른 내부 패턴을 가진 원형 글로우 스프라이트를 만든다.
/// </summary>
public static class CelestialSpriteGenerator
{
    public static Sprite GeneratePlanetSprite(Element element, Color baseColor, int size = 64)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float half = size * 0.5f;
        float radius = half - 1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - half;
                float dy = y - half;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float norm = dist / radius;

                if (norm > 1f)
                {
                    tex.SetPixel(x, y, Color.clear);
                    continue;
                }

                // 기본 원형 글로우
                float glow = 1f - norm * norm;
                float pattern = GetElementPattern(element, dx, dy, norm, half);
                float brightness = Mathf.Clamp01(glow * (0.7f + pattern * 0.5f));
                float edgeSoft = Mathf.Clamp01((1f - norm) * 5f);

                Color c = Color.Lerp(Color.black, baseColor, brightness);
                c.a = edgeSoft * Mathf.Clamp01(brightness + 0.15f);
                // 중심부를 밝게
                c = Color.Lerp(c, Color.white * 0.8f, Mathf.Pow(glow, 3f) * 0.3f);
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    public static Sprite GenerateStarSprite(Color baseColor, int size = 96)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float half = size * 0.5f;
        float radius = half - 1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - half;
                float dy = y - half;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float norm = dist / radius;

                // 원형 글로우
                float glow = Mathf.Max(0, 1f - norm);
                float core = Mathf.Pow(Mathf.Max(0, 1f - norm * 2f), 2f);

                // 십자형 빛줄기
                float ax = Mathf.Abs(dx);
                float ay = Mathf.Abs(dy);
                float crossH = Mathf.Exp(-ay * 0.3f) * Mathf.Max(0, 1f - norm * 0.5f) * 0.5f;
                float crossV = Mathf.Exp(-ax * 0.3f) * Mathf.Max(0, 1f - norm * 0.5f) * 0.5f;
                // 대각선 줄기
                float diag1 = Mathf.Exp(-Mathf.Abs(dx - dy) * 0.4f) * Mathf.Max(0, 1f - norm * 0.7f) * 0.25f;
                float diag2 = Mathf.Exp(-Mathf.Abs(dx + dy) * 0.4f) * Mathf.Max(0, 1f - norm * 0.7f) * 0.25f;

                float total = Mathf.Clamp01(glow * 0.6f + core + crossH + crossV + diag1 + diag2);
                float edgeSoft = Mathf.Clamp01((1f - norm) * 3f + crossH + crossV + diag1 + diag2);

                Color c = Color.Lerp(baseColor, Color.white, core * 0.8f);
                c = Color.Lerp(Color.black, c, total);
                c.a = Mathf.Clamp01(edgeSoft);
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    public static Sprite GenerateSatelliteSprite(Color baseColor, int size = 32)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float half = size * 0.5f;
        float radius = half - 1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - half;
                float dy = y - half;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float norm = dist / radius;

                if (norm > 1f) { tex.SetPixel(x, y, Color.clear); continue; }

                float glow = Mathf.Pow(1f - norm, 1.5f);
                float edgeSoft = Mathf.Clamp01((1f - norm) * 4f);

                Color c = Color.Lerp(baseColor * 0.5f, baseColor, glow);
                c = Color.Lerp(c, Color.white, Mathf.Pow(glow, 4f) * 0.4f);
                c.a = edgeSoft * Mathf.Clamp01(glow + 0.1f);
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    static float GetElementPattern(Element element, float dx, float dy, float norm, float half)
    {
        switch (element)
        {
            case Element.Fire:
                // 방사형 불꽃 줄무늬 (8방향)
                float angle = Mathf.Atan2(dy, dx);
                float flicker = Mathf.Sin(angle * 8f) * 0.5f + 0.5f;
                float radial = Mathf.Max(0, 1f - norm * 1.2f);
                return flicker * radial * 0.8f;

            case Element.Water:
                // 동심원 물결 링
                float rings = Mathf.Sin(norm * Mathf.PI * 7f) * 0.5f + 0.5f;
                return rings * (1f - norm) * 0.6f;

            case Element.Wind:
                // 나선형 소용돌이
                float a = Mathf.Atan2(dy, dx);
                float spiral = Mathf.Sin(a * 3f + norm * 12f) * 0.5f + 0.5f;
                return spiral * (1f - norm) * 0.7f;

            case Element.Earth:
                // 각진 결정 패턴
                float crystal = Mathf.Abs(Mathf.Sin(dx * 0.3f) * Mathf.Cos(dy * 0.3f));
                float facets = Mathf.Abs(Mathf.Sin((dx + dy) * 0.2f)) * Mathf.Abs(Mathf.Sin((dx - dy) * 0.2f));
                return (crystal * 0.5f + facets * 0.5f) * (1f - norm) * 0.7f;

            case Element.Darkness:
                // 역글로우: 가장자리가 밝고 중심이 어둡다
                float invGlow = norm * norm;
                float darkPulse = Mathf.Sin(norm * Mathf.PI * 4f) * 0.3f;
                return invGlow * 0.6f + darkPulse * (1f - norm);

            default: // None
                return 0f;
        }
    }
}
