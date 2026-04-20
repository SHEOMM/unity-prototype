using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Cosmos 패널 장식(chrome) 빌더 + 닫기 클릭 감지.
/// 기존 CosmosPanelView.BuildStaticChildren + Update 내 닫기 로직을 이전.
///
/// 수명: 패널 GameObject에 AddComponent로 한 번 부착. Show 때마다 Build → 자식 3개(Bg/Title/CloseX) 생성.
/// Hide에서 패널의 foreach Destroy가 자식 GameObject를 제거하면 Clear()로 참조 정리.
///
/// 드래그 중 등 외부 조건으로 클릭을 억제하려면 ShouldSuppressClick Func 설정.
/// </summary>
public class CosmosPanelChrome : MonoBehaviour
{
    public System.Action OnCloseClicked;
    public System.Func<bool> ShouldSuppressClick;

    BoxCollider2D _closeCol;

    public void Build(Transform parent, float panelWidth, float panelHeight)
    {
        BuildBg(parent, panelWidth, panelHeight);
        BuildTitle(parent, panelHeight);
        BuildCloseBtn(parent, panelWidth, panelHeight);
    }

    /// <summary>패널 Hide 시 자식 GameObject는 이미 파괴되므로 Collider 참조만 클리어.</summary>
    public void Clear()
    {
        _closeCol = null;
    }

    void BuildBg(Transform parent, float w, float h)
    {
        var go = new GameObject("Bg");
        go.transform.SetParent(parent, false);
        go.transform.localScale = new Vector3(w, h, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = UIFactory.MakePixel();
        sr.color = new Color(0.05f, 0.06f, 0.1f, 0.92f);
        sr.sortingOrder = GameConstants.SortingOrder.CosmosPanelBg;
    }

    void BuildTitle(Transform parent, float h)
    {
        var go = new GameObject("Title");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(0, h * 0.5f - 0.4f, 0);
        var tm = go.AddComponent<TextMesh>();
        tm.text = "Cosmos — 궤도 배치";
        tm.fontSize = 48;
        tm.characterSize = 0.08f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1f, 0.95f, 0.6f, 1f);
        var mr = go.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = GameConstants.SortingOrder.CosmosLabel;
    }

    void BuildCloseBtn(Transform parent, float w, float h)
    {
        const float CloseScale = 0.6f;
        var go = new GameObject("CloseX");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(w * 0.5f - 0.5f, h * 0.5f - 0.4f, 0);
        go.transform.localScale = new Vector3(CloseScale, CloseScale, 1f);
        var bgX = go.AddComponent<SpriteRenderer>();
        bgX.sprite = UIFactory.MakePixel();
        bgX.color = new Color(0.6f, 0.2f, 0.2f, 0.9f);
        bgX.sortingOrder = GameConstants.SortingOrder.CosmosCloseBg;

        var xGo = new GameObject("X");
        xGo.transform.SetParent(go.transform, false);
        xGo.transform.localScale = UIFactory.InverseScale(CloseScale);
        var xTm = xGo.AddComponent<TextMesh>();
        xTm.text = "X";
        xTm.fontSize = 44;
        xTm.characterSize = 0.08f;
        xTm.anchor = TextAnchor.MiddleCenter;
        xTm.alignment = TextAlignment.Center;
        xTm.color = Color.white;
        var xMr = xGo.GetComponent<MeshRenderer>();
        if (xMr != null) xMr.sortingOrder = GameConstants.SortingOrder.CosmosCloseText;

        _closeCol = go.AddComponent<BoxCollider2D>();
        _closeCol.size = Vector2.one;
    }

    void Update()
    {
        if (_closeCol == null) return;
        if (ShouldSuppressClick != null && ShouldSuppressClick()) return;
        var mouse = Mouse.current;
        if (mouse == null || CameraService.Instance == null) return;
        if (!mouse.leftButton.wasPressedThisFrame) return;
        Vector2 world = CameraService.Instance.ScreenToWorld2D(mouse.position.ReadValue());
        if (_closeCol.OverlapPoint(world)) OnCloseClicked?.Invoke();
    }
}
