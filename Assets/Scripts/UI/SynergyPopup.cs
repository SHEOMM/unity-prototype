using UnityEngine;

/// <summary>
/// 시저지 발동 시 화면 상단에 시저지 이름을 표시한다.
/// 1.5초 후 페이드아웃.
/// </summary>
public class SynergyPopup : MonoBehaviour
{
    private TextMesh _text;
    private float _lifetime = 1.5f;
    private float _elapsed;

    public static void Show(string synergyName)
    {
        var go = new GameObject("SynergyPopup");
        // 화면 상단 중앙 (카메라 기준)
        var cam = Camera.main;
        float y = cam != null ? cam.transform.position.y + cam.orthographicSize * 0.7f : 5f;
        go.transform.position = new Vector3(0, y, 0);
        go.transform.localScale = Vector3.one * 0.2f;

        var popup = go.AddComponent<SynergyPopup>();
        popup._text = go.AddComponent<TextMesh>();
        popup._text.text = synergyName + " !";
        popup._text.fontSize = 48;
        popup._text.anchor = TextAnchor.MiddleCenter;
        popup._text.alignment = TextAlignment.Center;
        popup._text.color = new Color(1f, 0.9f, 0.3f, 1f);
        popup._text.characterSize = 0.1f;

        var mr = go.GetComponent<MeshRenderer>();
        mr.sortingOrder = 25;
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
        // 등장: 커지면서, 퇴장: 페이드아웃
        float scale = t < 0.2f ? Mathf.Lerp(0.1f, 0.2f, t / 0.2f) : 0.2f;
        transform.localScale = Vector3.one * scale;
        transform.position += Vector3.up * 0.3f * Time.deltaTime;

        float alpha = t < 0.7f ? 1f : 1f - ((t - 0.7f) / 0.3f);
        _text.color = new Color(1f, 0.9f, 0.3f, alpha);
    }
}
