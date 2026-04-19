using UnityEngine;

/// <summary>
/// UI 요소 생성 유틸리티. 라벨, HP바 등 공통 UI를 코드로 생성한다.
/// Enemy, PlanetBody, OrbitBody 등에서 중복되던 코드를 통합.
/// </summary>
public static class UIFactory
{
    public static TextMesh CreateLabel(Transform parent, string text, float yOffset, float scaleMultiplier, Color color, int sortingOrder = GameConstants.SortingOrder.Label)
    {
        float parentScale = parent.lossyScale.x;
        float invScale = parentScale > GameConstants.CelestialLabel.MinScaleThreshold ? 1f / parentScale : 1f;

        var go = new GameObject("Label");
        go.transform.SetParent(parent);
        go.transform.localPosition = new Vector3(0, yOffset, 0);
        go.transform.localScale = Vector3.one * invScale * scaleMultiplier;

        var tm = go.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = GameConstants.CelestialLabel.FontSize;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = color;
        tm.characterSize = GameConstants.CelestialLabel.CharacterSize;

        go.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return tm;
    }

    public struct HPBarHandle
    {
        public SpriteRenderer background;
        public SpriteRenderer fill;
        public TextMesh text;
    }

    public static HPBarHandle CreateHPBar(Transform parent, float yOffset, float barWidth = GameConstants.EnemyUI.HPBarWidth)
    {
        float parentScale = parent.lossyScale.x;
        float invScale = parentScale > GameConstants.CelestialLabel.MinScaleThreshold ? 1f / parentScale : 1f;

        // 배경
        var bgGo = new GameObject("HPBar_BG");
        bgGo.transform.SetParent(parent);
        bgGo.transform.localPosition = new Vector3(0, yOffset, 0);
        bgGo.transform.localScale = new Vector3(
            barWidth * invScale,
            GameConstants.EnemyUI.HPBarHeight * invScale, 1f);
        var bg = bgGo.AddComponent<SpriteRenderer>();
        bg.sprite = MakePixel();
        bg.color = GameConstants.Colors.HPBarBg;
        bg.sortingOrder = GameConstants.SortingOrder.HPBarBackground;

        // 채움
        var fillGo = new GameObject("HPBar_Fill");
        fillGo.transform.SetParent(bgGo.transform);
        fillGo.transform.localPosition = Vector3.zero;
        fillGo.transform.localScale = Vector3.one;
        var fill = fillGo.AddComponent<SpriteRenderer>();
        fill.sprite = MakePixel();
        fill.color = GameConstants.Colors.HPFull;
        fill.sortingOrder = GameConstants.SortingOrder.HPBarFill;

        // 수치
        var textGo = new GameObject("HPText");
        textGo.transform.SetParent(parent);
        textGo.transform.localPosition = new Vector3(0, yOffset, 0);
        textGo.transform.localScale = Vector3.one * invScale * GameConstants.EnemyUI.HPTextScale;
        var tm = textGo.AddComponent<TextMesh>();
        tm.fontSize = GameConstants.EnemyUI.LabelFontSize;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        tm.characterSize = GameConstants.CelestialLabel.CharacterSize;
        textGo.GetComponent<MeshRenderer>().sortingOrder = GameConstants.SortingOrder.HPBarText;

        return new HPBarHandle { background = bg, fill = fill, text = tm };
    }

    public static void UpdateHPBar(HPBarHandle handle, float currentHP, float maxHP)
    {
        if (handle.fill == null) return;
        float ratio = Mathf.Clamp01(currentHP / maxHP);
        handle.fill.transform.localScale = new Vector3(ratio, 1f, 1f);
        handle.fill.transform.localPosition = new Vector3((ratio - 1f) * 0.5f, 0, 0);
        handle.fill.color = Color.Lerp(GameConstants.Colors.HPEmpty, GameConstants.Colors.HPFull, ratio);
        if (handle.text != null)
            handle.text.text = $"{Mathf.CeilToInt(currentHP)}/{Mathf.CeilToInt(maxHP)}";
    }

    public static Sprite MakePixel()
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
