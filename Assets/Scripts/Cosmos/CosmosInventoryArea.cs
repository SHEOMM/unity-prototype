using UnityEngine;

/// <summary>
/// 하단 인벤토리 영역. 미배치 행성 토큰들이 이 Transform 아래에 정렬되어 배치됨.
/// 드롭 타겟 Collider로 궤도→인벤토리 언바인딩 지원.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class CosmosInventoryArea : MonoBehaviour
{
    BoxCollider2D _col;
    SpriteRenderer _bg;
    TextMesh _labelTm;
    Transform _tokenRoot;
    Vector2 _size;

    const float TokenSpacing = 0.9f;

    /// <summary>토큰이 부착될 앵커 (부모 스케일 상쇄된 자식).</summary>
    public Transform TokenRoot => _tokenRoot;

    public void Initialize(Vector2 size)
    {
        _size = size;
        _bg = gameObject.AddComponent<SpriteRenderer>();
        _bg.sprite = UIFactory.MakePixel();
        _bg.color = new Color(0.1f, 0.12f, 0.16f, 0.85f);
        _bg.sortingOrder = 30;
        transform.localScale = new Vector3(size.x, size.y, 1f);

        // 토큰 앵커 — 부모(area) 스케일을 상쇄해 월드 좌표 기준 배치 가능
        var rootGo = new GameObject("TokenRoot");
        rootGo.transform.SetParent(transform, false);
        rootGo.transform.localPosition = Vector3.zero;
        rootGo.transform.localScale = new Vector3(1f / size.x, 1f / size.y, 1f);
        _tokenRoot = rootGo.transform;

        // 라벨 (좌상단)
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(transform, false);
        labelGo.transform.localPosition = new Vector3(-0.43f, 0.4f, 0);
        // 부모 스케일 상쇄
        labelGo.transform.localScale = new Vector3(1f / size.x, 1f / size.y, 1f);
        _labelTm = labelGo.AddComponent<TextMesh>();
        _labelTm.text = "인벤토리";
        _labelTm.fontSize = 32;
        _labelTm.characterSize = 0.1f;
        _labelTm.anchor = TextAnchor.MiddleLeft;
        _labelTm.alignment = TextAlignment.Left;
        _labelTm.color = new Color(0.85f, 0.85f, 0.9f, 1f);
        var mr = labelGo.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = 44;

        _col = GetComponent<BoxCollider2D>();
        _col.size = Vector2.one;
        _col.offset = Vector2.zero;
    }

    public bool ContainsWorldPoint(Vector2 worldPoint)
    {
        return _col != null && _col.OverlapPoint(worldPoint);
    }

    /// <summary>
    /// 인벤토리 토큰들을 가로로 재정렬 (TokenRoot 아래 로컬 배치 — 부모 스케일 상쇄됨).
    /// </summary>
    public void LayoutTokens(System.Collections.Generic.List<CosmosPlanetToken> tokens)
    {
        if (tokens == null || tokens.Count == 0 || _tokenRoot == null) return;
        int n = tokens.Count;
        float startX = -(n - 1) * 0.5f * TokenSpacing;

        for (int i = 0; i < n; i++)
        {
            var t = tokens[i];
            if (t == null) continue;
            t.transform.SetParent(_tokenRoot, false);
            t.transform.localPosition = new Vector3(startX + i * TokenSpacing, 0, 0);
            t.transform.localScale = Vector3.one * 0.7f;
        }
    }
}
