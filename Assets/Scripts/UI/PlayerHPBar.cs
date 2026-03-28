using UnityEngine;

/// <summary>
/// 화면 좌상단에 플레이어 HP 바를 표시한다.
/// </summary>
public class PlayerHPBar : MonoBehaviour
{
    private Texture2D _bgTex;
    private Texture2D _fillTex;
    private Texture2D _damageTex;

    void Awake()
    {
        _bgTex = MakeTexture(new Color(0.15f, 0.15f, 0.2f, 0.8f));
        _fillTex = MakeTexture(new Color(0.2f, 0.8f, 0.3f, 0.9f));
        _damageTex = MakeTexture(new Color(0.8f, 0.2f, 0.2f, 0.9f));
    }

    void OnGUI()
    {
        if (PlayerState.Instance == null) return;

        float ratio = PlayerState.Instance.currentHP / PlayerState.Instance.maxHP;
        float barW = 220f;
        float barH = 22f;
        float x = 12f;
        float y = 12f;

        // 배경
        GUI.DrawTexture(new Rect(x - 2, y - 2, barW + 4, barH + 4), _bgTex);

        // 손실 HP (빨간색)
        GUI.DrawTexture(new Rect(x, y, barW, barH), _damageTex);

        // 현재 HP (초록색)
        GUI.DrawTexture(new Rect(x, y, barW * Mathf.Clamp01(ratio), barH), _fillTex);

        // 텍스트
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 13,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(x, y, barW, barH),
            $"HP  {Mathf.CeilToInt(PlayerState.Instance.currentHP)} / {Mathf.CeilToInt(PlayerState.Instance.maxHP)}",
            style);
    }

    static Texture2D MakeTexture(Color color)
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return tex;
    }
}
