using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Cosmos 패널 내 드래그 상태 관리자.
/// 입력 루프 (ShipInput 패턴): Down → Drag → Up.
/// 토큰을 파괴 또는 중복 로직 분산 없이 단일 컨트롤러가 처리.
/// Panel.Resolve(token, targetSlot, targetInventory)로 드롭 결과 위임.
/// </summary>
public class CosmosDragController : MonoBehaviour
{
    CosmosPanelView _panel;
    CosmosPlanetToken _draggingToken;

    public bool IsDragging => _draggingToken != null;

    public void Initialize(CosmosPanelView panel)
    {
        _panel = panel;
    }

    void Update()
    {
        if (_panel == null || !_panel.IsOpen) return;

        var mouse = Mouse.current;
        if (mouse == null || CameraService.Instance == null) return;

        Vector2 screen = mouse.position.ReadValue();
        Vector2 world = CameraService.Instance.ScreenToWorld2D(screen);

        if (_draggingToken == null)
        {
            if (mouse.leftButton.wasPressedThisFrame)
                TryStartDrag(world);
        }
        else
        {
            // 드래그 중
            _draggingToken.SetDragWorldPos(world);
            if (mouse.leftButton.wasReleasedThisFrame)
                EndDrag(world);
        }
    }

    void TryStartDrag(Vector2 world)
    {
        var tokens = _panel.AllTokens;
        // 뒤에서부터 찾기 — 렌더 순서 상 위에 있는 것 우선
        for (int i = tokens.Count - 1; i >= 0; i--)
        {
            var t = tokens[i];
            if (t == null) continue;
            if (t.ContainsWorldPoint(world))
            {
                _draggingToken = t;
                t.RememberOrigin();
                // parent 해제 — 월드 좌표로 이동
                t.transform.SetParent(null, true);
                return;
            }
        }
    }

    void EndDrag(Vector2 world)
    {
        var slot = _panel.FindSlotUnderPoint(world);
        var invHit = _panel.InventoryContainsPoint(world);
        _panel.ResolveDrop(_draggingToken, slot, invHit);
        _draggingToken = null;
    }
}
