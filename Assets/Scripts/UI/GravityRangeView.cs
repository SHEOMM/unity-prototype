using UnityEngine;

/// <summary>
/// 중력 범위를 반투명 원으로 시각화한다.
/// IGravitySource(모델)에서 범위와 색상을 읽어 표시만 담당.
/// 부모 스케일을 상쇄하여 월드 기준 정확한 반지름으로 표시.
/// </summary>
public class GravityRangeView : MonoBehaviour
{
    private const int Segments = 48;

    void Start()
    {
        var source = GetComponent<IGravitySource>();
        if (source == null || source.GravityStrength <= 0f) return;

        DrawRange(source);
    }

    void DrawRange(IGravitySource source)
    {
        var go = new GameObject("GravityRange");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        // 부모 스케일 상쇄 — 월드 기준 정확한 반지름
        float parentScale = transform.lossyScale.x;
        float invScale = parentScale > 0.01f ? 1f / parentScale : 1f;
        go.transform.localScale = Vector3.one * invScale;

        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.startWidth = 0.03f;
        lr.endWidth = 0.03f;
        lr.material = GameConstants.SpriteMaterial;
        lr.sortingOrder = GameConstants.SortingOrder.OrbitPath;

        Color rangeColor = source.CachedGravityType.RangeColor;
        lr.startColor = rangeColor;
        lr.endColor = rangeColor;

        float radius = source.GravityRange * parentScale;
        lr.positionCount = Segments;
        for (int i = 0; i < Segments; i++)
        {
            float angle = (float)i / Segments * Mathf.PI * 2f;
            lr.SetPosition(i, new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius, 0));
        }
    }
}
