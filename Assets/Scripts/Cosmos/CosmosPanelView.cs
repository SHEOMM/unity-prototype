using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cosmos 배치 패널. MapScene 오버레이 — 궤도 슬롯 + 인벤토리 영역 + 드래그.
/// Show/Hide 토글, Refresh로 RunState 기준 재구성.
///
/// PR4 리팩터링: Chrome(Bg/Title/Close) / DropResolver(enum 분류) 분리.
/// PanelView는 슬롯·토큰 Refresh 조정 + 드롭 enum 처리에 집중.
/// </summary>
public class CosmosPanelView : MonoBehaviour
{
    public bool IsOpen { get; private set; }

    CosmosDragController _drag;
    CosmosPanelChrome _chrome;
    CosmosInventoryArea _inventory;
    readonly List<CosmosOrbitSlot> _slots = new List<CosmosOrbitSlot>();
    readonly List<CosmosPlanetToken> _tokens = new List<CosmosPlanetToken>();

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

        _chrome = gameObject.AddComponent<CosmosPanelChrome>();
        _chrome.OnCloseClicked = Hide;
        _chrome.ShouldSuppressClick = () => _drag != null && _drag.IsDragging;

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

        var cam = CameraService.Instance?.Camera;
        Vector3 center = cam != null ? cam.transform.position : Vector3.zero;
        center.z = 0;
        transform.position = center;

        _chrome.Build(transform, PanelWidth, PanelHeight);
        Refresh();
        _drag.enabled = true;
    }

    public void Hide()
    {
        IsOpen = false;
        _drag.enabled = false;
        foreach (Transform c in transform) Destroy(c.gameObject);
        _slots.Clear();
        _tokens.Clear();
        _inventory = null;
        _chrome?.Clear();
        gameObject.SetActive(false);
    }

    /// <summary>RunState 스냅샷 기준으로 슬롯·토큰 재생성.</summary>
    public void Refresh()
    {
        var run = RunState.Instance;
        if (run == null) return;

        foreach (var s in _slots) if (s != null) Destroy(s.gameObject);
        _slots.Clear();
        foreach (var t in _tokens) if (t != null) Destroy(t.gameObject);
        _tokens.Clear();
        if (_inventory != null) { Destroy(_inventory.gameObject); _inventory = null; }

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

        var invGo = new GameObject("InventoryArea");
        invGo.transform.SetParent(transform, false);
        invGo.transform.localPosition = new Vector3(0, InventoryY, 0);
        _inventory = invGo.AddComponent<CosmosInventoryArea>();
        _inventory.Initialize(new Vector2(PanelWidth - 1f, 2.2f));

        var unassignedTokens = new List<CosmosPlanetToken>();
        foreach (var p in run.planetDeck)
        {
            if (p == null) continue;
            var tgo = new GameObject("Token_" + p.bodyName);
            var token = tgo.AddComponent<CosmosPlanetToken>();
            token.Initialize(p);
            _tokens.Add(token);

            var orbit = OrbitAssignmentQuery.FindOrbitForPlanet(run, p.bodyName);
            var slot = orbit != null ? FindSlotForOrbit(orbit) : null;
            if (slot != null)
            {
                token.transform.SetParent(slot.TokenAnchor, false);
                token.transform.localPosition = Vector3.zero;
                token.transform.localScale = Vector3.one * 0.7f;
            }
            else
            {
                unassignedTokens.Add(token);
            }
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

    /// <summary>드래그 결과 처리 — DropResolver로 분류하고 CosmosService 호출.</summary>
    public void ResolveDrop(CosmosPlanetToken token, CosmosOrbitSlot targetSlot, bool inventoryHit)
    {
        if (token == null) return;
        var run = RunState.Instance;
        if (run == null) { token.SnapBack(); return; }

        var planet = token.Planet;
        var originOrbit = OrbitAssignmentQuery.FindOrbitForPlanet(run, planet.bodyName);
        var result = CosmosDropResolver.Resolve(originOrbit, targetSlot, inventoryHit);

        switch (result)
        {
            case DropResult.MoveOrSwap:
                CosmosService.Swap(run, originOrbit, targetSlot.Orbit);
                Refresh();
                break;
            case DropResult.Assign:
                CosmosService.Assign(run, targetSlot.Orbit, planet);
                Refresh();
                break;
            case DropResult.Unassign:
                CosmosService.Unassign(run, planet);
                Refresh();
                break;
            case DropResult.SnapBack:
            default:
                token.SnapBack();
                break;
        }
    }
}
