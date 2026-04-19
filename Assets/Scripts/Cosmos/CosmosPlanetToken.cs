using UnityEngine;

/// <summary>
/// Cosmos 패널에서 드래그 가능한 행성 토큰.
/// 렌더(아이콘 + 이름) + 원위치 기억 + 드래그 중 위치 조작.
/// 실제 드래그 입력 감지는 CosmosDragController가 담당.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class CosmosPlanetToken : MonoBehaviour
{
    public PlanetSO Planet { get; private set; }
    /// <summary>드래그 시작 시 Parent Transform. 실패 시 여기로 복귀.</summary>
    public Transform OriginParent { get; private set; }
    public Vector3 OriginLocalPos { get; private set; }

    const float TokenSize = 0.7f;

    BoxCollider2D _col;
    SpriteRenderer _bg;
    SpriteRenderer _icon;
    TextMesh _nameTm;

    public void Initialize(PlanetSO planet)
    {
        Planet = planet;

        _bg = GetOrAdd<SpriteRenderer>(gameObject);
        _bg.sprite = UIFactory.MakePixel();
        _bg.color = new Color(0.2f, 0.22f, 0.28f, 0.9f);
        _bg.sortingOrder = 42;
        transform.localScale = Vector3.one * TokenSize;

        var iconGo = new GameObject("Icon");
        iconGo.transform.SetParent(transform, false);
        iconGo.transform.localScale = Vector3.one * 0.85f;
        _icon = iconGo.AddComponent<SpriteRenderer>();
        _icon.sprite = PlanetSpriteResolver.Resolve(planet);
        _icon.color = Color.white;
        _icon.sortingOrder = 43;

        var nameGo = new GameObject("Name");
        nameGo.transform.SetParent(transform, false);
        nameGo.transform.localPosition = new Vector3(0, -0.7f, 0);
        nameGo.transform.localScale = Vector3.one;
        _nameTm = nameGo.AddComponent<TextMesh>();
        _nameTm.text = planet.bodyName;
        _nameTm.fontSize = 40;
        _nameTm.characterSize = 0.08f;
        _nameTm.anchor = TextAnchor.MiddleCenter;
        _nameTm.alignment = TextAlignment.Center;
        _nameTm.color = Color.white;
        var mr = nameGo.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = 44;

        _col = GetComponent<BoxCollider2D>();
        _col.size = Vector2.one;  // 부모 스케일 적용 후 월드 크기 = TokenSize
        _col.offset = Vector2.zero;
    }

    /// <summary>드래그 시작 전 parent/localPos 스냅샷. 실패 시 SnapBack으로 복구.</summary>
    public void RememberOrigin()
    {
        OriginParent = transform.parent;
        OriginLocalPos = transform.localPosition;
    }

    /// <summary>드래그 중: 월드 좌표로 위치 이동 (parent 해제된 상태).</summary>
    public void SetDragWorldPos(Vector3 worldPos)
    {
        transform.position = new Vector3(worldPos.x, worldPos.y, 0);
    }

    /// <summary>드래그 실패 → 원위치로 복귀.</summary>
    public void SnapBack()
    {
        if (OriginParent != null) transform.SetParent(OriginParent, false);
        transform.localPosition = OriginLocalPos;
    }

    /// <summary>새 parent로 재부착 (슬롯/인벤토리에 배치 완료).</summary>
    public void Reparent(Transform newParent, Vector3 newLocalPos)
    {
        transform.SetParent(newParent, false);
        transform.localPosition = newLocalPos;
    }

    /// <summary>월드 포인트가 토큰 위에 있는지. Collider 기반.</summary>
    public bool ContainsWorldPoint(Vector2 worldPoint)
    {
        return _col != null && _col.OverlapPoint(worldPoint);
    }

    static T GetOrAdd<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        return c != null ? c : go.AddComponent<T>();
    }
}
