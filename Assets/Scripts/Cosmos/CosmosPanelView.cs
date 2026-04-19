using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Cosmos 배치 패널. MapScene 오버레이 — 궤도 슬롯 + 인벤토리 영역 + 드래그 컨트롤러.
/// Show/Hide 토글, Refresh로 RunState 기준 재구성.
///
/// 구조:
///   Panel (this)
///   ├─ Bg        (반투명 전면 — 외부 클릭 차단)
///   ├─ Title
///   ├─ CloseX    (클릭 시 Hide)
///   ├─ OrbitSlots[]
///   ├─ InventoryArea
///   └─ DragController
/// </summary>
public class CosmosPanelView : MonoBehaviour
{
    public bool IsOpen { get; private set; }

    CosmosDragController _drag;
    CosmosInventoryArea _inventory;
    readonly List<CosmosOrbitSlot> _slots = new List<CosmosOrbitSlot>();
    readonly List<CosmosPlanetToken> _tokens = new List<CosmosPlanetToken>();

    GameObject _bg;
    GameObject _title;
    GameObject _closeBtn;
    BoxCollider2D _closeCol;

    const float PanelWidth = 14f;
    const float PanelHeight = 8f;
    const float SlotAreaY = 1.4f;
    const float SlotSpacing = 2.2f;
    const float InventoryY = -1.8f;

    public IReadOnlyList<CosmosPlanetToken> AllTokens => _tokens;

    public void Initialize()
    {
        _drag = gameObject.AddComponent<CosmosDragController>();
        _drag.Initialize(this);
        _drag.enabled = false;
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen) Hide();
        else Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        IsOpen = true;
        BuildStaticChildren();
        Refresh();
        _drag.enabled = true;
    }

    public void Hide()
    {
        IsOpen = false;
        _drag.enabled = false;
        // 자식 전부 파괴 (Refresh가 다음 Show 때 재구성)
        foreach (Transform c in transform) Destroy(c.gameObject);
        _slots.Clear();
        _tokens.Clear();
        _inventory = null;
        _bg = _title = _closeBtn = null;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!IsOpen || _closeCol == null) return;
        var mouse = Mouse.current;
        if (mouse == null || CameraService.Instance == null) return;
        if (!mouse.leftButton.wasPressedThisFrame) return;
        // 드래그 중이면 닫기 버튼 클릭 무시 (드래그 종료가 우선)
        if (_drag != null && _drag.IsDragging) return;

        Vector2 screen = mouse.position.ReadValue();
        Vector2 world = CameraService.Instance.ScreenToWorld2D(screen);
        if (_closeCol.OverlapPoint(world)) Hide();
    }

    void BuildStaticChildren()
    {
        var cam = CameraService.Instance?.Camera;
        Vector3 center = cam != null ? cam.transform.position : Vector3.zero;
        center.z = 0;
        transform.position = center;

        // 배경
        _bg = new GameObject("Bg");
        _bg.transform.SetParent(transform, false);
        _bg.transform.localScale = new Vector3(PanelWidth, PanelHeight, 1f);
        var sr = _bg.AddComponent<SpriteRenderer>();
        sr.sprite = UIFactory.MakePixel();
        sr.color = new Color(0.05f, 0.06f, 0.1f, 0.92f);
        sr.sortingOrder = 28;

        // 타이틀
        _title = new GameObject("Title");
        _title.transform.SetParent(transform, false);
        _title.transform.localPosition = new Vector3(0, PanelHeight * 0.5f - 0.4f, 0);
        var tm = _title.AddComponent<TextMesh>();
        tm.text = "Cosmos — 궤도 배치";
        tm.fontSize = 48;
        tm.characterSize = 0.08f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1f, 0.95f, 0.6f, 1f);
        var mr = _title.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = 44;

        // 닫기 버튼 (우상단)
        _closeBtn = new GameObject("CloseX");
        _closeBtn.transform.SetParent(transform, false);
        _closeBtn.transform.localPosition = new Vector3(PanelWidth * 0.5f - 0.5f, PanelHeight * 0.5f - 0.4f, 0);
        _closeBtn.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        var bgX = _closeBtn.AddComponent<SpriteRenderer>();
        bgX.sprite = UIFactory.MakePixel();
        bgX.color = new Color(0.6f, 0.2f, 0.2f, 0.9f);
        bgX.sortingOrder = 45;
        var xGo = new GameObject("X");
        xGo.transform.SetParent(_closeBtn.transform, false);
        xGo.transform.localScale = new Vector3(1f / 0.6f, 1f / 0.6f, 1f);
        var xTm = xGo.AddComponent<TextMesh>();
        xTm.text = "X";
        xTm.fontSize = 44;
        xTm.characterSize = 0.08f;
        xTm.anchor = TextAnchor.MiddleCenter;
        xTm.alignment = TextAlignment.Center;
        xTm.color = Color.white;
        var xMr = xGo.GetComponent<MeshRenderer>();
        if (xMr != null) xMr.sortingOrder = 46;
        _closeCol = _closeBtn.AddComponent<BoxCollider2D>();
        _closeCol.size = Vector2.one;
    }

    /// <summary>RunState 스냅샷 기준으로 슬롯·토큰 재생성.</summary>
    public void Refresh()
    {
        var run = RunState.Instance;
        if (run == null) return;

        // 슬롯 파괴 → 재생성
        foreach (var s in _slots) if (s != null) Destroy(s.gameObject);
        _slots.Clear();
        foreach (var t in _tokens) if (t != null) Destroy(t.gameObject);
        _tokens.Clear();
        if (_inventory != null) { Destroy(_inventory.gameObject); _inventory = null; }

        // 궤도 슬롯들 (상단)
        var orbits = run.unlockedOrbits;
        int n = orbits.Count;
        float startX = -(n - 1) * 0.5f * SlotSpacing;
        for (int i = 0; i < n; i++)
        {
            var orbit = orbits[i];
            if (orbit == null) continue;
            var go = new GameObject("Slot");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(startX + i * SlotSpacing, SlotAreaY, 0);
            var slot = go.AddComponent<CosmosOrbitSlot>();
            slot.Initialize(orbit);
            _slots.Add(slot);
        }

        // 인벤토리 영역 (하단)
        var invGo = new GameObject("InventoryArea");
        invGo.transform.SetParent(transform, false);
        invGo.transform.localPosition = new Vector3(0, InventoryY, 0);
        _inventory = invGo.AddComponent<CosmosInventoryArea>();
        _inventory.Initialize(new Vector2(PanelWidth - 1f, 2.2f));

        // 각 행성 토큰 생성 후 슬롯/인벤토리에 배치
        var assignedTokens = new List<CosmosPlanetToken>();
        var unassignedTokens = new List<CosmosPlanetToken>();
        foreach (var p in run.planetDeck)
        {
            if (p == null) continue;
            var tgo = new GameObject("Token_" + p.bodyName);
            var token = tgo.AddComponent<CosmosPlanetToken>();
            token.Initialize(p);
            _tokens.Add(token);

            var orbit = run.FindOrbitForPlanet(p.bodyName);
            if (orbit != null)
            {
                var slot = FindSlotForOrbit(orbit);
                if (slot != null)
                {
                    token.transform.SetParent(slot.TokenAnchor, false);
                    token.transform.localPosition = Vector3.zero;
                    token.transform.localScale = Vector3.one * 0.7f;
                    assignedTokens.Add(token);
                    continue;
                }
            }
            unassignedTokens.Add(token);
        }
        _inventory.LayoutTokens(unassignedTokens);
    }

    CosmosOrbitSlot FindSlotForOrbit(OrbitSO orbit)
    {
        foreach (var s in _slots)
            if (s != null && s.Orbit == orbit) return s;
        return null;
    }

    public CosmosOrbitSlot FindSlotUnderPoint(Vector2 world)
    {
        foreach (var s in _slots)
            if (s != null && s.ContainsWorldPoint(world)) return s;
        return null;
    }

    public bool InventoryContainsPoint(Vector2 world)
    {
        return _inventory != null && _inventory.ContainsWorldPoint(world);
    }

    /// <summary>
    /// 드래그 결과 처리. originOrbit은 RunState 조회로 얻고, 대상은 slot/inventory/none.
    /// 실패 시 토큰을 원위치로 스냅백. 성공 시 Refresh로 UI 재구성.
    /// </summary>
    public void ResolveDrop(CosmosPlanetToken token, CosmosOrbitSlot targetSlot, bool inventoryHit)
    {
        if (token == null) return;
        var run = RunState.Instance;
        if (run == null) { token.SnapBack(); return; }

        var planet = token.Planet;
        var originOrbit = run.FindOrbitForPlanet(planet.bodyName);

        if (targetSlot != null)
        {
            var targetOrbit = targetSlot.Orbit;
            if (originOrbit == targetOrbit)
            {
                // 같은 슬롯 — no-op
                token.SnapBack();
                return;
            }
            // 교환 or 이동 (AssignPlanetToOrbit이 기존 궤도의 다른 행성을 제거하므로,
            // 교환이 필요한 경우 다음 Refresh에서 해제된 행성은 인벤토리로 내려감.
            // 명시적 swap 의미론을 원하면 아래 로직으로 처리.)
            string displacedName = run.FindPlanetForOrbit(targetOrbit.orbitName);
            run.AssignPlanetToOrbit(targetOrbit, planet);
            // 기존 궤도에 전위된 행성 재배치 (swap 의미 보장):
            //   - 출발이 궤도였고(originOrbit != null) + 대상에 다른 행성(displaced)이 있었을 때
            if (originOrbit != null && !string.IsNullOrEmpty(displacedName) && displacedName != planet.bodyName)
            {
                var displacedPlanet = run.FindPlanetByName(displacedName);
                if (displacedPlanet != null)
                    run.AssignPlanetToOrbit(originOrbit, displacedPlanet);
            }
            Refresh();
            return;
        }

        if (inventoryHit)
        {
            if (originOrbit != null)
            {
                run.UnassignPlanet(planet);
                Refresh();
            }
            else
            {
                // 인벤토리 → 인벤토리: no-op
                token.SnapBack();
            }
            return;
        }

        // void 드롭 — 원위치
        token.SnapBack();
    }
}
