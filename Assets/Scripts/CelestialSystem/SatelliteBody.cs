using UnityEngine;

/// <summary>
/// 위성 런타임. 행성에 부착되어 공전하며 패시브 버프를 제공한다.
/// </summary>
public class SatelliteBody : MonoBehaviour
{
    public SatelliteSO Data { get; private set; }
    private float _angle;
    private SpriteRenderer _sr;

    public void Initialize(SatelliteSO data, Sprite sprite)
    {
        Data = data;
        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null) _sr = gameObject.AddComponent<SpriteRenderer>();
        if (sprite != null) _sr.sprite = sprite;
        _sr.color = data.bodyColor;
        _sr.sortingOrder = GameConstants.SortingOrder.SatelliteBody;
        transform.localScale = Vector3.one * data.visualScale;
    }

    void Update()
    {
        if (Data == null) return;
        _angle += Data.orbitSpeed * Time.deltaTime;
        float rad = _angle * Mathf.Deg2Rad;
        transform.localPosition = new Vector3(
            Mathf.Cos(rad) * Data.orbitRadius,
            Mathf.Sin(rad) * Data.orbitRadius,
            0
        );
    }
}
