using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 부모 GameObject의 IStatusHost를 찾아 활성 상태이상 아이콘을 머리 위에 렌더.
/// 각 상태는 IStatusEffect.IconId → StatusIconRegistry 조회 → 색상 + 라벨로 표시.
/// 별도 스프라이트 에셋 없이 UIFactory.MakePixel() + 색상으로 간이 표현.
///
/// Enemy/AllyUnit/Structure 어디에 붙어도 동일 동작 (IStatusHost 의존만).
/// </summary>
public class StatusIconView : MonoBehaviour
{
    private IStatusHost _host;

    private const float IconSize = 0.2f;
    private const float IconGap = 0.05f;
    private const float YOffset = 1.05f;

    private readonly List<IconSlot> _slots = new List<IconSlot>();

    struct IconSlot
    {
        public string id;
        public GameObject go;
        public SpriteRenderer sr;
        public TextMesh label;
    }

    void Start()
    {
        _host = GetComponent<IStatusHost>();
    }

    void Update()
    {
        if (_host == null || _host.ActiveStatuses == null) return;

        // 유니크 id 목록 수집 (동일 id 여러 인스턴스는 1개만 표시)
        var active = new List<string>();
        foreach (var s in _host.ActiveStatuses)
        {
            if (s?.effect == null) continue;
            var id = s.effect.IconId;
            if (string.IsNullOrEmpty(id)) continue;
            if (!active.Contains(id)) active.Add(id);
        }

        // 제거: slot 중 active에 없는 것
        for (int i = _slots.Count - 1; i >= 0; i--)
        {
            if (!active.Contains(_slots[i].id))
            {
                if (_slots[i].go != null) Destroy(_slots[i].go);
                _slots.RemoveAt(i);
            }
        }

        // 추가: active 중 slot에 없는 것
        foreach (var id in active)
        {
            bool exists = false;
            foreach (var slot in _slots) if (slot.id == id) { exists = true; break; }
            if (!exists)
            {
                var meta = StatusIconRegistry.Get(id);
                if (meta == null) continue;
                _slots.Add(CreateSlot(id, meta));
            }
        }

        // 배치 (오른쪽으로 나열)
        float parentScale = transform.lossyScale.x;
        float invScale = parentScale > 0.01f ? 1f / parentScale : 1f;
        for (int i = 0; i < _slots.Count; i++)
        {
            var slot = _slots[i];
            if (slot.go == null) continue;
            float x = (IconSize + IconGap) * i * invScale;
            slot.go.transform.localPosition = new Vector3(x, YOffset * invScale, 0);
        }
    }

    IconSlot CreateSlot(string id, IStatusIconMeta meta)
    {
        var go = new GameObject("StatusIcon_" + id);
        go.transform.SetParent(transform);
        go.transform.localScale = Vector3.one * IconSize;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = UIFactory.MakePixel();
        sr.color = meta.Color;
        sr.sortingOrder = GameConstants.SortingOrder.HPBarText + 1;

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform);
        labelGo.transform.localPosition = Vector3.zero;
        labelGo.transform.localScale = Vector3.one * 0.6f;
        var tm = labelGo.AddComponent<TextMesh>();
        tm.text = meta.Label;
        tm.fontSize = 32;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.characterSize = 0.08f;
        tm.color = Color.black;
        var mr = labelGo.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = GameConstants.SortingOrder.HPBarText + 2;

        return new IconSlot { id = id, go = go, sr = sr, label = tm };
    }

    void OnDestroy()
    {
        foreach (var slot in _slots)
            if (slot.go != null) Destroy(slot.go);
        _slots.Clear();
    }
}
