using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 보상 카드 1개 렌더 + 클릭 감지. RewardSceneBoot이 3개 인스턴스화.
/// New Input System 기반 클릭 판정 (MapView와 동일 패턴):
///   Mouse.current.leftButton.wasPressedThisFrame → CameraService 월드좌표 변환 → BoxCollider2D.OverlapPoint
/// (레거시 OnMouseDown은 New Input System 환경에서 비활성화될 수 있어 미사용.)
///
/// 레이아웃:
///   [TypeBadge] (카드 상단에 얇은 띠 — 타입 색상)
///   [Icon]      (중앙 — 스프라이트 또는 궤도 미니 원)
///   [Name]      (아이콘 아래, TextMesh)
///   [TypeLabel] (뱃지 중앙, TextMesh)
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class RewardCardView : MonoBehaviour
{
    public System.Action<RewardCardView> OnClicked;
    public RewardChoice Choice { get; private set; }

    const float CardWidth = 2.6f;
    const float CardHeight = 3.6f;
    const float IconSize = 1.4f;

    SpriteRenderer _bg;
    SpriteRenderer _badge;
    SpriteRenderer _iconSr;
    TextMesh _nameTm;
    TextMesh _typeTm;
    LineRenderer _orbitRing;
    BoxCollider2D _col;
    bool _clickable = true;

    public void Initialize(RewardChoice choice)
    {
        Choice = choice;
        BuildBackground();
        BuildBadge(choice.TypeColor, choice.TypeLabel);
        BuildIcon(choice);
        BuildName(choice.DisplayName);

        _col = GetComponent<BoxCollider2D>();
        _col.size = new Vector2(CardWidth, CardHeight);
        _col.offset = Vector2.zero;
    }

    public void SetClickable(bool clickable) { _clickable = clickable; }

    void Update()
    {
        if (!_clickable || _col == null) return;
        var mouse = Mouse.current;
        if (mouse == null || CameraService.Instance == null) return;
        if (!mouse.leftButton.wasPressedThisFrame) return;

        Vector2 screenPos = mouse.position.ReadValue();
        if (screenPos.x < 1f || screenPos.y < 1f) return;
        if (screenPos.x > Screen.width - 1 || screenPos.y > Screen.height - 1) return;

        Vector2 mp = CameraService.Instance.ScreenToWorld2D(screenPos);
        if (_col.OverlapPoint(mp))
        {
            OnClicked?.Invoke(this);
        }
    }

    void BuildBackground()
    {
        var go = new GameObject("Bg");
        go.transform.SetParent(transform, false);
        go.transform.localScale = new Vector3(CardWidth, CardHeight, 1f);
        _bg = go.AddComponent<SpriteRenderer>();
        _bg.sprite = UIFactory.MakePixel();
        _bg.color = new Color(0.12f, 0.14f, 0.2f, 0.95f);
        _bg.sortingOrder = GameConstants.SortingOrder.RewardCardBg;
    }

    void BuildBadge(Color color, string label)
    {
        var go = new GameObject("Badge");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0, CardHeight * 0.5f - 0.22f, 0);
        go.transform.localScale = new Vector3(CardWidth * 0.95f, 0.4f, 1f);
        _badge = go.AddComponent<SpriteRenderer>();
        _badge.sprite = UIFactory.MakePixel();
        _badge.color = color;
        _badge.sortingOrder = GameConstants.SortingOrder.RewardCardMid;

        var labelGo = new GameObject("BadgeLabel");
        labelGo.transform.SetParent(transform, false);
        labelGo.transform.localPosition = new Vector3(0, CardHeight * 0.5f - 0.22f, 0);
        labelGo.transform.localScale = Vector3.one;
        _typeTm = labelGo.AddComponent<TextMesh>();
        _typeTm.text = label;
        _typeTm.fontSize = 42;
        _typeTm.characterSize = 0.08f;
        _typeTm.anchor = TextAnchor.MiddleCenter;
        _typeTm.alignment = TextAlignment.Center;
        _typeTm.color = Color.black;
        var mr = labelGo.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = GameConstants.SortingOrder.RewardCardText;
    }

    void BuildIcon(RewardChoice choice)
    {
        if (choice.Icon != null)
        {
            var go = new GameObject("Icon");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0, 0.25f, 0);
            go.transform.localScale = new Vector3(IconSize, IconSize, 1f);
            _iconSr = go.AddComponent<SpriteRenderer>();
            _iconSr.sprite = choice.Icon;
            _iconSr.sortingOrder = GameConstants.SortingOrder.RewardCardMid;
        }
        else if (choice.Payload is OrbitSO orbit)
        {
            BuildOrbitPreview(orbit);
        }
    }

    void BuildOrbitPreview(OrbitSO orbit)
    {
        var go = new GameObject("OrbitPreview");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0, 0.25f, 0);
        go.transform.localScale = Vector3.one;
        _orbitRing = go.AddComponent<LineRenderer>();
        _orbitRing.useWorldSpace = false;
        _orbitRing.loop = true;
        _orbitRing.startWidth = 0.04f;
        _orbitRing.endWidth = 0.04f;
        _orbitRing.material = GameConstants.SpriteMaterial;
        _orbitRing.startColor = orbit.orbitLineColor;
        _orbitRing.endColor = orbit.orbitLineColor;
        _orbitRing.sortingOrder = GameConstants.SortingOrder.RewardCardMid;

        // 카드 안에 들어가도록 반경 정규화 (최대 0.6)
        float normR = Mathf.Min(orbit.radius, 1.8f) * 0.4f;
        const int segs = 40;
        _orbitRing.positionCount = segs;
        for (int i = 0; i < segs; i++)
        {
            float t = (float)i / segs * Mathf.PI * 2f;
            _orbitRing.SetPosition(i, new Vector3(Mathf.Cos(t) * normR, Mathf.Sin(t) * normR, 0));
        }

        // 궤도 중앙에 작은 원 (행성 위치 표시)
        var dotGo = new GameObject("OrbitDot");
        dotGo.transform.SetParent(go.transform, false);
        dotGo.transform.localPosition = new Vector3(normR, 0, 0);
        dotGo.transform.localScale = Vector3.one * 0.14f;
        var sr = dotGo.AddComponent<SpriteRenderer>();
        sr.sprite = UIFactory.MakePixel();
        sr.color = Color.white;
        sr.sortingOrder = GameConstants.SortingOrder.RewardCardText;
    }

    void BuildName(string name)
    {
        var go = new GameObject("Name");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0, -1.1f, 0);
        _nameTm = go.AddComponent<TextMesh>();
        _nameTm.text = name;
        _nameTm.fontSize = 44;
        _nameTm.characterSize = 0.08f;
        _nameTm.anchor = TextAnchor.MiddleCenter;
        _nameTm.alignment = TextAlignment.Center;
        _nameTm.color = Color.white;
        var mr = go.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = GameConstants.SortingOrder.RewardCardText;
    }
}
