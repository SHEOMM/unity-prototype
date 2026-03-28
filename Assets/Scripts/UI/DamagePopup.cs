using UnityEngine;

/// <summary>
/// 적에게 데미지가 들어갈 때 숫자를 팝업으로 표시한다.
/// 위로 떠오르며 페이드아웃된다. 속성별 색상 적용.
/// </summary>
public class DamagePopup : MonoBehaviour
{
    private TextMesh _text;
    private Color _color;
    private float _lifetime = 0.8f;
    private float _elapsed;
    private Vector3 _velocity;

    public static void Spawn(Vector3 position, float damage, Element element)
    {
        var go = new GameObject("DmgPopup");
        go.transform.position = position + new Vector3(Random.Range(-0.3f, 0.3f), 0.5f, 0);
        go.transform.localScale = Vector3.one * 0.12f;

        var popup = go.AddComponent<DamagePopup>();
        popup._color = SpellEffectManager.GetElementColor(element);
        popup._velocity = new Vector3(Random.Range(-0.5f, 0.5f), 2f, 0);

        popup._text = go.AddComponent<TextMesh>();
        popup._text.text = Mathf.RoundToInt(damage).ToString();
        popup._text.fontSize = 64;
        popup._text.anchor = TextAnchor.MiddleCenter;
        popup._text.alignment = TextAlignment.Center;
        popup._text.color = popup._color;
        popup._text.characterSize = 0.1f;

        var mr = go.GetComponent<MeshRenderer>();
        mr.sortingOrder = 20;
    }

    void Update()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed >= _lifetime)
        {
            Destroy(gameObject);
            return;
        }

        float t = _elapsed / _lifetime;
        transform.position += _velocity * Time.deltaTime;
        _velocity.y -= 3f * Time.deltaTime;

        // 페이드아웃 + 크기 축소
        float alpha = 1f - t;
        float scale = 0.12f * (1f + t * 0.3f);
        transform.localScale = Vector3.one * scale;
        _text.color = new Color(_color.r, _color.g, _color.b, alpha);
    }
}
