using UnityEngine;

/// <summary>
/// 혜성 런타임. 하늘을 가로지르며 우주선으로 포착 가능.
/// 포착 시 보상 이벤트를 발행한다.
/// </summary>
public class CometBody : MonoBehaviour
{
    public static readonly System.Collections.Generic.List<CometBody> ActiveComets = new System.Collections.Generic.List<CometBody>();

    public CometSO Data { get; private set; }

    public System.Action<CometBody> OnCaptured;

    private SpriteRenderer _sr;
    private Vector2 _startPos;
    private Vector2 _endPos;
    private float _t;
    private float _duration;
    private bool _captured;
    private float _baseScale;

    void OnEnable() { ActiveComets.Add(this); }
    void OnDisable() { ActiveComets.Remove(this); }

    public void Initialize(CometSO data, Sprite sprite, Vector2 start, Vector2 end)
    {
        Data = data;
        _startPos = start;
        _endPos = end;
        _baseScale = data.visualScale;

        float dist = Vector2.Distance(start, end);
        _duration = dist / Mathf.Max(0.1f, data.flySpeed);

        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null) _sr = gameObject.AddComponent<SpriteRenderer>();
        if (sprite != null) _sr.sprite = sprite;
        _sr.color = data.bodyColor;
        _sr.sortingOrder = GameConstants.SortingOrder.CometBody;
        transform.localScale = Vector3.one * _baseScale;
        transform.position = (Vector3)start;

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = GameConstants.VFXAnimation.CometColliderRadius;
    }

    void Update()
    {
        if (_captured) return;

        _t += Time.deltaTime / _duration;
        if (_t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector2.Lerp(_startPos, _endPos, _t);

        // 반짝이는 효과 (Sin 기반 스케일 펄스)
        float pulse = 1f + Mathf.Sin(Time.time * GameConstants.VFXAnimation.CometPulseFrequency)
                          * GameConstants.VFXAnimation.CometPulseAmplitude;
        transform.localScale = Vector3.one * _baseScale * pulse;
    }

    public void Capture()
    {
        if (_captured) return;
        _captured = true;
        OnCaptured?.Invoke(this);
        Destroy(gameObject, 0.3f);
    }

    public bool IntersectsLine(Vector2 a, Vector2 b, float width)
        => CollisionGeometry.IntersectsLine(transform.position, _baseScale * 0.5f, a, b, width);

    public float ProjectionT(Vector2 a, Vector2 b)
        => CollisionGeometry.ProjectionT(transform.position, a, b);
}
