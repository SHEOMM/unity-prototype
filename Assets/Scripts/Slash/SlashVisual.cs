using UnityEngine;

/// <summary>
/// 슬래시 라인 비주얼. 드래그 중 선을 그려준다.
/// 히트된 별 수에 따라 선 색상이 밝아지고 굵어진다.
/// </summary>
public class SlashVisual : MonoBehaviour
{
    public float lineWidth = 0.3f;
    public Color lineColor = new Color(1f, 1f, 0.5f, 0.7f);

    private LineRenderer _lr;
    private int _hitCount;

    void Awake()
    {
        var go = new GameObject("SlashLineVisual");
        go.transform.SetParent(transform);
        _lr = go.AddComponent<LineRenderer>();
        _lr.material = GameConstants.SpriteMaterial;
        _lr.positionCount = 0;
        _lr.sortingOrder = 10;
        ApplyStyle(0);
    }

    public void ShowLine(Vector2 start, Vector2 end, int hitCount = 0)
    {
        _lr.positionCount = 2;
        _lr.SetPosition(0, (Vector3)start);
        _lr.SetPosition(1, (Vector3)end);

        if (hitCount != _hitCount)
        {
            _hitCount = hitCount;
            ApplyStyle(hitCount);
        }
    }

    public void HideLine()
    {
        _lr.positionCount = 0;
        _hitCount = 0;
        ApplyStyle(0);
    }

    void ApplyStyle(int hitCount)
    {
        float widthMult = 1f + hitCount * 0.15f;
        _lr.startWidth = lineWidth * widthMult;
        _lr.endWidth = lineWidth * widthMult * 0.6f;

        // 히트 수가 많을수록 밝고 뜨거운 색상
        float intensity = Mathf.Clamp01(hitCount * 0.2f);
        Color c = Color.Lerp(lineColor, new Color(1f, 0.6f, 0.2f, 0.9f), intensity);
        _lr.startColor = c;
        _lr.endColor = new Color(c.r, c.g, c.b, c.a * 0.5f);
    }
}
