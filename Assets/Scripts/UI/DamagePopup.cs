using UnityEngine;

/// <summary>
/// 적에게 데미지가 들어갈 때 숫자를 팝업으로 표시한다.
/// 위로 떠오르며 페이드아웃된다. 속성별 색상 적용.
/// </summary>
public class DamagePopup : MonoBehaviour
{
    private TextMesh _text;
    private Color _color;
    private float _lifetime = GameConstants.Popup.Lifetime;
    private float _elapsed;
    private Vector3 _velocity;

    public static void Spawn(Vector3 position, float damage, Element element)
    {
        var go = new GameObject("DmgPopup");
        go.transform.position = position + new Vector3(
            Random.Range(-GameConstants.Popup.SpawnXJitter, GameConstants.Popup.SpawnXJitter),
            GameConstants.Popup.SpawnYOffset, 0);
        go.transform.localScale = Vector3.one * GameConstants.Popup.InitialScale;

        var popup = go.AddComponent<DamagePopup>();
        popup._color = SpellEffectManager.GetElementColor(element);
        popup._velocity = new Vector3(
            Random.Range(-GameConstants.Popup.SpawnXJitter * 1.5f, GameConstants.Popup.SpawnXJitter * 1.5f),
            GameConstants.Popup.VelocityY, 0);

        popup._text = go.AddComponent<TextMesh>();
        popup._text.text = Mathf.RoundToInt(damage).ToString();
        popup._text.fontSize = GameConstants.CelestialLabel.FontSize;
        popup._text.anchor = TextAnchor.MiddleCenter;
        popup._text.alignment = TextAlignment.Center;
        popup._text.color = popup._color;
        popup._text.characterSize = GameConstants.CelestialLabel.CharacterSize;

        go.GetComponent<MeshRenderer>().sortingOrder = GameConstants.SortingOrder.DamagePopup;
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
        _velocity.y -= GameConstants.Popup.Gravity * Time.deltaTime;

        float alpha = 1f - t;
        float scale = GameConstants.Popup.InitialScale * (1f + t * GameConstants.Popup.ScaleGrowth);
        transform.localScale = Vector3.one * scale;
        _text.color = new Color(_color.r, _color.g, _color.b, alpha);
    }
}
