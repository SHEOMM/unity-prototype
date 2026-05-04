using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 보상 카드 1개 렌더 + 클릭 감지. Prefab 기반 — 구조는 Prefab이 제공하고
/// 본 컴포넌트는 데이터를 자식 SpriteRenderer/TextMesh에 바인딩한다.
///
/// <para>Prefab 자식 구조 (RewardCard.prefab):
/// <list type="bullet">
/// <item>Bg — SpriteRenderer (카드 배경)</item>
/// <item>Badge — SpriteRenderer (상단 타입 띠)</item>
/// <item>BadgeLabel — TextMesh (타입 텍스트, 예: "행성")</item>
/// <item>Icon — SpriteRenderer (행성/유물 아이콘, 또는 비활성)</item>
/// <item>OrbitPreview/Ring + Dot — LineRenderer + 작은 점 (궤도 전용, 또는 비활성)</item>
/// <item>Name — TextMesh (보상 이름)</item>
/// </list>
/// 데이터 바인딩은 <see cref="Bind"/>가 단일 진입점.</para>
///
/// <para>클릭 감지: New Input System 기반. <c>Mouse.current.leftButton.wasPressedThisFrame</c>
/// → CameraService 월드좌표 변환 → BoxCollider2D.OverlapPoint.</para>
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class RewardCardView : MonoBehaviour
{
    public System.Action<RewardCardView> OnClicked;
    public RewardChoice Choice { get; private set; }

    // ── Prefab 자식 참조 (Inspector 할당) ──
    [Header("Prefab 자식 참조")]
    [SerializeField] SpriteRenderer bg;
    [SerializeField] SpriteRenderer badge;
    [SerializeField] TextMesh badgeLabel;
    [SerializeField] SpriteRenderer iconSr;
    [SerializeField] GameObject orbitPreviewGo;
    [SerializeField] LineRenderer orbitRing;
    [SerializeField] Transform orbitDot;
    [SerializeField] TextMesh nameTm;
    [SerializeField] BoxCollider2D col;

    bool _clickable = true;

    void Awake()
    {
        if (col == null) col = GetComponent<BoxCollider2D>();
        EnsureSprites();
    }

    /// <summary>
    /// Prefab의 SpriteRenderer.sprite가 null인 경우 UIFactory.MakePixel()로 채움.
    /// Bg/Badge/OrbitDot은 색상만 사용하는 사각형이라 1x1 흰 픽셀이면 충분.
    /// (Prefab에 sprite asset을 직접 박지 않은 이유: 아직 Pixel.png 자산이 없음. 추후 폴리싱)
    /// </summary>
    void EnsureSprites()
    {
        var pixel = UIFactory.MakePixel();
        if (bg != null && bg.sprite == null) bg.sprite = pixel;
        if (badge != null && badge.sprite == null) badge.sprite = pixel;
        if (orbitDot != null)
        {
            var dotSr = orbitDot.GetComponent<SpriteRenderer>();
            if (dotSr != null && dotSr.sprite == null) dotSr.sprite = pixel;
        }
    }

    /// <summary>
    /// 카드 데이터 주입. Prefab 자식들의 색·텍스트·아이콘을 갱신.
    /// 궤도 보상은 OrbitPreview 활성화 + 반경 정규화, 그 외는 Icon 활성화.
    /// </summary>
    public void Bind(RewardChoice choice)
    {
        Choice = choice;
        if (badge != null) badge.color = choice.TypeColor;
        if (badgeLabel != null) badgeLabel.text = choice.TypeLabel;
        if (nameTm != null) nameTm.text = choice.DisplayName;

        bool isOrbit = choice.Icon == null && choice.Payload is OrbitSO;
        if (iconSr != null) iconSr.gameObject.SetActive(!isOrbit && choice.Icon != null);
        if (orbitPreviewGo != null) orbitPreviewGo.SetActive(isOrbit);

        if (choice.Icon != null && iconSr != null)
            iconSr.sprite = choice.Icon;

        if (isOrbit && choice.Payload is OrbitSO orbit)
            BindOrbitPreview(orbit);
    }

    void BindOrbitPreview(OrbitSO orbit)
    {
        if (orbitRing == null) return;
        orbitRing.startColor = orbit.orbitLineColor;
        orbitRing.endColor = orbit.orbitLineColor;

        // 카드 안에 들어가도록 반경 정규화 (최대 0.6)
        float normR = Mathf.Min(orbit.radius, 1.8f) * 0.4f;
        const int segs = 40;
        orbitRing.positionCount = segs;
        for (int i = 0; i < segs; i++)
        {
            float t = (float)i / segs * Mathf.PI * 2f;
            orbitRing.SetPosition(i, new Vector3(Mathf.Cos(t) * normR, Mathf.Sin(t) * normR, 0));
        }

        if (orbitDot != null) orbitDot.localPosition = new Vector3(normR, 0, 0);
    }

    public void SetClickable(bool clickable) { _clickable = clickable; }

    void Update()
    {
        if (!_clickable || col == null) return;
        var mouse = Mouse.current;
        if (mouse == null || CameraService.Instance == null) return;
        if (!mouse.leftButton.wasPressedThisFrame) return;

        Vector2 screenPos = mouse.position.ReadValue();
        if (screenPos.x < 1f || screenPos.y < 1f) return;
        if (screenPos.x > Screen.width - 1 || screenPos.y > Screen.height - 1) return;

        Vector2 mp = CameraService.Instance.ScreenToWorld2D(screenPos);
        if (col.OverlapPoint(mp))
            OnClicked?.Invoke(this);
    }

    void OnValidate()
    {
        if (bg == null || badge == null || badgeLabel == null || iconSr == null || nameTm == null)
            Debug.LogWarning($"[RewardCardView] Prefab 자식 참조 누락 — Inspector에서 할당 필요: {name}", this);
    }
}
