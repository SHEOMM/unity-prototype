using UnityEngine;

/// <summary>
/// 물병별 수위 게이지. AquariusState의 WaterRatio를 읽어 바 형태로 표시한다.
/// 자체 Update에서 갱신 — PlanetBody가 호출하지 않는다.
/// </summary>
public class AquariusHUD : MonoBehaviour, IPlanetHUD
{
    private SpriteRenderer _fillBar;
    private AquariusState _state;

    void Start()
    {
        _state = GetComponent<AquariusState>();
        CreateBar();
    }

    void Update()
    {
        UpdateHUD();
    }

    void CreateBar()
    {
        float parentScale = transform.lossyScale.x;
        float invScale = parentScale > GameConstants.CelestialLabel.MinScaleThreshold ? 1f / parentScale : 1f;

        var bgGo = new GameObject("WaterBar_BG");
        bgGo.transform.SetParent(transform);
        bgGo.transform.localPosition = new Vector3(0, 1.2f, 0);
        bgGo.transform.localScale = new Vector3(0.8f * invScale, 0.08f * invScale, 1f);

        var bg = bgGo.AddComponent<SpriteRenderer>();
        bg.sprite = UIFactory.MakePixel();
        bg.color = new Color(0.1f, 0.1f, 0.3f, 0.6f);
        bg.sortingOrder = GameConstants.SortingOrder.HPBarFill;

        var fillGo = new GameObject("WaterBar_Fill");
        fillGo.transform.SetParent(bgGo.transform);
        fillGo.transform.localPosition = Vector3.zero;
        fillGo.transform.localScale = Vector3.one;

        _fillBar = fillGo.AddComponent<SpriteRenderer>();
        _fillBar.sprite = UIFactory.MakePixel();
        _fillBar.color = new Color(0.3f, 0.5f, 1f, 0.9f);
        _fillBar.sortingOrder = GameConstants.SortingOrder.Label;
    }

    public void UpdateHUD()
    {
        if (_state == null || _fillBar == null) return;
        float ratio = _state.WaterRatio;
        _fillBar.transform.localScale = new Vector3(ratio, 1f, 1f);
        _fillBar.transform.localPosition = new Vector3((ratio - 1f) * 0.5f, 0, 0);
        _fillBar.color = Color.Lerp(new Color(0.3f, 0.5f, 1f, 0.9f), new Color(0.1f, 0.8f, 1f, 1f), ratio);
    }
}
