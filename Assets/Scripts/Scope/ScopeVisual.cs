using UnityEngine;

/// <summary>
/// 원형 스코프 비주얼. 마우스를 따라다니는 원형 범위 표시 + 쿨타임 인디케이터.
/// </summary>
public class ScopeVisual : MonoBehaviour
{
    private LineRenderer _scopeRing;
    private LineRenderer _cooldownArc;
    private float _radius;
    private bool _visible;

    private const int RingSegments = 64;

    public void Initialize(float radius)
    {
        _radius = radius;

        // 스코프 원형 링
        var ringGo = new GameObject("ScopeRing");
        ringGo.transform.SetParent(transform);
        _scopeRing = ringGo.AddComponent<LineRenderer>();
        _scopeRing.useWorldSpace = true;
        _scopeRing.loop = true;
        _scopeRing.startWidth = 0.04f;
        _scopeRing.endWidth = 0.04f;
        _scopeRing.material = GameConstants.SpriteMaterial;
        _scopeRing.sortingOrder = GameConstants.SortingOrder.SlashLine;
        _scopeRing.positionCount = RingSegments;

        // 쿨타임 호 (링 위에 다른 색으로 겹침)
        var cdGo = new GameObject("CooldownArc");
        cdGo.transform.SetParent(transform);
        _cooldownArc = cdGo.AddComponent<LineRenderer>();
        _cooldownArc.useWorldSpace = true;
        _cooldownArc.loop = false;
        _cooldownArc.startWidth = 0.06f;
        _cooldownArc.endWidth = 0.06f;
        _cooldownArc.material = GameConstants.SpriteMaterial;
        _cooldownArc.sortingOrder = GameConstants.SortingOrder.SlashLine + 1;
        _cooldownArc.positionCount = 0;
    }

    public void Show(Vector2 center, int hitCount, bool onCooldown, float cooldownRatio)
    {
        _visible = true;

        // 히트 수에 따라 색상 변화
        float intensity = Mathf.Clamp01(hitCount * 0.2f);
        Color ringColor = onCooldown
            ? new Color(0.5f, 0.5f, 0.5f, 0.3f)
            : Color.Lerp(new Color(0.5f, 0.8f, 1f, 0.5f), new Color(1f, 0.8f, 0.3f, 0.8f), intensity);

        _scopeRing.startColor = ringColor;
        _scopeRing.endColor = ringColor;

        for (int i = 0; i < RingSegments; i++)
        {
            float angle = (float)i / RingSegments * Mathf.PI * 2f;
            float x = center.x + Mathf.Cos(angle) * _radius;
            float y = center.y + Mathf.Sin(angle) * _radius;
            _scopeRing.SetPosition(i, new Vector3(x, y, 0));
        }

        // 쿨타임 호 표시
        if (onCooldown && cooldownRatio > 0f)
        {
            int arcSegs = Mathf.Max(2, Mathf.CeilToInt(RingSegments * cooldownRatio));
            _cooldownArc.positionCount = arcSegs;
            _cooldownArc.startColor = new Color(1f, 0.3f, 0.3f, 0.7f);
            _cooldownArc.endColor = new Color(1f, 0.3f, 0.3f, 0.7f);
            for (int i = 0; i < arcSegs; i++)
            {
                float angle = (float)i / RingSegments * Mathf.PI * 2f;
                float x = center.x + Mathf.Cos(angle) * _radius;
                float y = center.y + Mathf.Sin(angle) * _radius;
                _cooldownArc.SetPosition(i, new Vector3(x, y, 0));
            }
        }
        else
        {
            _cooldownArc.positionCount = 0;
        }
    }

    public void Hide()
    {
        _visible = false;
        _scopeRing.positionCount = 0;
        _cooldownArc.positionCount = 0;
    }
}
