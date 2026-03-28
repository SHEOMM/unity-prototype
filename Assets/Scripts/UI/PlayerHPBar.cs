using UnityEngine;

/// <summary>
/// 화면 좌상단에 플레이어 HP 바를 표시한다.
/// PlayerState.OnHPChanged 이벤트를 구독하여 캐싱된 값으로 그린다.
/// </summary>
public class PlayerHPBar : MonoBehaviour
{
    private Texture2D _bgTex;
    private Texture2D _fillTex;
    private Texture2D _damageTex;
    private float _ratio = 1f;
    private float _current;
    private float _max = 100f;

    void Awake()
    {
        _bgTex = MakeTexture(new Color(0.15f, 0.15f, 0.2f, 0.8f));
        _fillTex = MakeTexture(new Color(0.2f, 0.8f, 0.3f, 0.9f));
        _damageTex = MakeTexture(new Color(0.8f, 0.2f, 0.2f, 0.9f));
    }

    void OnEnable()
    {
        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.OnHPChanged += HandleHPChanged;
            _current = PlayerState.Instance.currentHP;
            _max = PlayerState.Instance.maxHP;
            _ratio = _max > 0 ? _current / _max : 0f;
        }
    }

    void OnDisable()
    {
        if (PlayerState.Instance != null)
            PlayerState.Instance.OnHPChanged -= HandleHPChanged;
    }

    void HandleHPChanged(float current, float max)
    {
        _current = current;
        _max = max;
        _ratio = max > 0 ? current / max : 0f;
    }

    void OnGUI()
    {
        float barW = 220f;
        float barH = 22f;
        float x = 12f;
        float y = 12f;

        GUI.DrawTexture(new Rect(x - 2, y - 2, barW + 4, barH + 4), _bgTex);
        GUI.DrawTexture(new Rect(x, y, barW, barH), _damageTex);
        GUI.DrawTexture(new Rect(x, y, barW * Mathf.Clamp01(_ratio), barH), _fillTex);

        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 13,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(x, y, barW, barH),
            $"HP  {Mathf.CeilToInt(_current)} / {Mathf.CeilToInt(_max)}", style);
    }

    static Texture2D MakeTexture(Color color)
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return tex;
    }
}
