using UnityEngine;

/// <summary>
/// 궤도 하나의 드롭 슬롯. 미니 궤도 프리뷰(LineRenderer 원) + 이름 라벨 + 드롭 타겟 Collider.
/// 점유 상태는 자식 CosmosPlanetToken 존재 여부로 판단.
/// CosmosDragController가 이 Collider로 OverlapPoint 조회해 드롭 판정.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class CosmosOrbitSlot : MonoBehaviour
{
    public OrbitSO Orbit { get; private set; }

    /// <summary>토큰이 부착될 앵커. 슬롯 중앙.</summary>
    public Transform TokenAnchor => _anchor;

    const float SlotSize = 1.5f;

    BoxCollider2D _col;
    SpriteRenderer _bg;
    LineRenderer _preview;
    TextMesh _nameTm;
    Transform _anchor;

    public void Initialize(OrbitSO orbit)
    {
        Orbit = orbit;
        gameObject.name = "Slot_" + orbit.orbitName;

        _bg = gameObject.AddComponent<SpriteRenderer>();
        _bg.sprite = UIFactory.MakePixel();
        _bg.color = new Color(0.15f, 0.17f, 0.22f, 0.95f);
        _bg.sortingOrder = GameConstants.SortingOrder.CosmosSlotBg;
        transform.localScale = new Vector3(SlotSize, SlotSize, 1f);

        // 이름 라벨 (슬롯 상단)
        var nameGo = new GameObject("Name");
        nameGo.transform.SetParent(transform, false);
        nameGo.transform.localPosition = new Vector3(0, 0.55f, 0);
        nameGo.transform.localScale = Vector3.one;
        _nameTm = nameGo.AddComponent<TextMesh>();
        _nameTm.text = orbit.orbitName;
        _nameTm.fontSize = 36;
        _nameTm.characterSize = 0.08f;
        _nameTm.anchor = TextAnchor.MiddleCenter;
        _nameTm.alignment = TextAlignment.Center;
        _nameTm.color = new Color(1f, 0.95f, 0.7f, 1f);
        var mr = nameGo.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = GameConstants.SortingOrder.CosmosLabel;

        // 궤도 프리뷰 (슬롯 중앙, LineRenderer 원)
        var previewGo = new GameObject("Preview");
        previewGo.transform.SetParent(transform, false);
        previewGo.transform.localPosition = Vector3.zero;
        previewGo.transform.localScale = Vector3.one;
        _preview = previewGo.AddComponent<LineRenderer>();
        _preview.useWorldSpace = false;
        _preview.loop = true;
        _preview.startWidth = 0.03f;
        _preview.endWidth = 0.03f;
        _preview.material = GameConstants.SpriteMaterial;
        _preview.startColor = orbit.orbitLineColor;
        _preview.endColor = orbit.orbitLineColor;
        _preview.sortingOrder = GameConstants.SortingOrder.CosmosSlotPreview;

        float normR = Mathf.Min(orbit.radius, 3f) * 0.18f;  // 슬롯 안에 들어가도록 정규화
        const int segs = 40;
        _preview.positionCount = segs;
        for (int i = 0; i < segs; i++)
        {
            float t = (float)i / segs * Mathf.PI * 2f;
            _preview.SetPosition(i, new Vector3(Mathf.Cos(t) * normR, Mathf.Sin(t) * normR, 0));
        }

        // 토큰 부착 앵커 (슬롯 중앙)
        var anchorGo = new GameObject("TokenAnchor");
        anchorGo.transform.SetParent(transform, false);
        anchorGo.transform.localPosition = Vector3.zero;
        // 부모(슬롯)의 스케일을 상쇄해 토큰이 원래 크기로 보이게
        anchorGo.transform.localScale = UIFactory.InverseScale(SlotSize);
        _anchor = anchorGo.transform;

        _col = GetComponent<BoxCollider2D>();
        _col.size = Vector2.one;
        _col.offset = Vector2.zero;
    }

    public bool ContainsWorldPoint(Vector2 worldPoint)
    {
        return _col != null && _col.OverlapPoint(worldPoint);
    }
}
