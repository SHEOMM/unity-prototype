using UnityEngine;

/// <summary>
/// 혜성 런타임. 하늘을 가로지르며 슬래시로 포착 가능.
/// 포착 시 보상 이벤트를 발행한다.
/// </summary>
public class CometBody : MonoBehaviour, ISlashTarget
{
    public CometSO Data { get; private set; }

    public System.Action<CometBody> OnCaptured;

    private SpriteRenderer _sr;
    private Vector2 _startPos;
    private Vector2 _endPos;
    private float _t;
    private float _duration;
    private bool _captured;
    private float _baseScale;

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
        _sr.sortingOrder = 8;
        transform.localScale = Vector3.one * _baseScale;
        transform.position = (Vector3)start;

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;
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

        // 반짝이는 효과
        float pulse = 1f + Mathf.Sin(Time.time * 8f) * 0.15f;
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
    {
        Vector2 pos = transform.position;
        float totalR = (_baseScale * 0.5f) + width * 0.5f;
        Vector2 ab = b - a;
        float dot = Vector2.Dot(ab, ab);
        if (dot < 0.0001f) return Vector2.Distance(pos, a) <= totalR;
        float t = Mathf.Clamp01(Vector2.Dot(pos - a, ab) / dot);
        return Vector2.Distance(pos, a + t * ab) <= totalR;
    }

    public float ProjectionT(Vector2 a, Vector2 b)
    {
        Vector2 pos = transform.position;
        Vector2 ab = b - a;
        return Mathf.Clamp01(Vector2.Dot(pos - a, ab) / Vector2.Dot(ab, ab));
    }
}
