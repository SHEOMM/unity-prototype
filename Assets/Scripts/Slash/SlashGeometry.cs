using UnityEngine;

/// <summary>
/// 슬래시 충돌 판정 유틸리티. PlanetBody와 CometBody에서 공유.
/// </summary>
public static class SlashGeometry
{
    public static bool IntersectsLine(Vector2 pos, float objectRadius, Vector2 a, Vector2 b, float lineWidth)
    {
        float totalR = objectRadius + lineWidth * 0.5f;
        Vector2 ab = b - a;
        float dot = Vector2.Dot(ab, ab);
        if (dot < 0.0001f) return Vector2.Distance(pos, a) <= totalR;
        float t = Mathf.Clamp01(Vector2.Dot(pos - a, ab) / dot);
        return Vector2.Distance(pos, a + t * ab) <= totalR;
    }

    public static float ProjectionT(Vector2 pos, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        return Mathf.Clamp01(Vector2.Dot(pos - a, ab) / Vector2.Dot(ab, ab));
    }
}
