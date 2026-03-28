using UnityEngine;

/// <summary>
/// 물병별 수위 게이지. AquariusState의 WaterRatio를 읽어 바 형태로 표시한다.
/// PlanetBody 위에 작은 게이지 바로 표시된다.
/// </summary>
public class AquariusHUD : MonoBehaviour, IPlanetHUD
{
    private SpriteRenderer _bgBar;
    private SpriteRenderer _fillBar;
    private AquariusState _state;
    private float _barWidth = 0.8f;
    private float _barHeight = 0.08f;

    void Start()
    {
        _state = GetComponent<AquariusState>();
        CreateBar();
    }

    void CreateBar()
    {
        // 배경 바
        var bgGo = new GameObject("WaterBar_BG");
        bgGo.transform.SetParent(transform);
        bgGo.transform.localPosition = new Vector3(0, 1.2f, 0);
        // 부모 스케일 상쇄
        float parentScale = transform.lossyScale.x;
        float invScale = parentScale > 0.01f ? 1f / parentScale : 1f;
        bgGo.transform.localScale = new Vector3(_barWidth * invScale, _barHeight * invScale, 1f);

        _bgBar = bgGo.AddComponent<SpriteRenderer>();
        _bgBar.sprite = CreatePixelSprite();
        _bgBar.color = new Color(0.1f, 0.1f, 0.3f, 0.6f);
        _bgBar.sortingOrder = 9;

        // 채움 바
        var fillGo = new GameObject("WaterBar_Fill");
        fillGo.transform.SetParent(bgGo.transform);
        fillGo.transform.localPosition = Vector3.zero;
        fillGo.transform.localScale = Vector3.one;

        _fillBar = fillGo.AddComponent<SpriteRenderer>();
        _fillBar.sprite = CreatePixelSprite();
        _fillBar.color = new Color(0.3f, 0.5f, 1f, 0.9f);
        _fillBar.sortingOrder = 10;
    }

    public void UpdateHUD()
    {
        if (_state == null || _fillBar == null) return;

        float ratio = _state.WaterRatio;
        var t = _fillBar.transform;
        t.localScale = new Vector3(ratio, 1f, 1f);
        // 왼쪽 정렬: 피벗을 왼쪽으로 밀기
        t.localPosition = new Vector3((ratio - 1f) * 0.5f, 0, 0);

        // 수위에 따라 색상 변화
        _fillBar.color = Color.Lerp(
            new Color(0.3f, 0.5f, 1f, 0.9f),
            new Color(0.1f, 0.8f, 1f, 1f),
            ratio
        );
    }

    static Sprite CreatePixelSprite()
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
