using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Map 화면 상단 "Cosmos" 토글 버튼. 클릭 시 CosmosPanelView.Toggle().
/// 카메라 상단 고정 위치.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class CosmosMapButton : MonoBehaviour
{
    CosmosPanelView _panel;
    BoxCollider2D _col;
    SpriteRenderer _bg;
    TextMesh _tm;
    Camera _cam;

    const float YOffset = 3.8f;      // 카메라 중심에서 위로
    const float Width = 2.4f;
    const float Height = 0.7f;

    public void Bind(CosmosPanelView panel)
    {
        _panel = panel;
    }

    void Start()
    {
        _cam = CameraService.Instance?.Camera;

        _bg = gameObject.AddComponent<SpriteRenderer>();
        _bg.sprite = UIFactory.MakePixel();
        _bg.color = new Color(0.2f, 0.5f, 0.8f, 0.95f);
        _bg.sortingOrder = 20;
        transform.localScale = new Vector3(Width, Height, 1f);

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(transform, false);
        labelGo.transform.localScale = new Vector3(1f / Width, 1f / Height, 1f);
        _tm = labelGo.AddComponent<TextMesh>();
        _tm.text = "Cosmos";
        _tm.fontSize = 40;
        _tm.characterSize = 0.1f;
        _tm.anchor = TextAnchor.MiddleCenter;
        _tm.alignment = TextAlignment.Center;
        _tm.color = Color.white;
        var mr = labelGo.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = 21;

        _col = GetComponent<BoxCollider2D>();
        _col.size = Vector2.one;
    }

    void LateUpdate()
    {
        if (_cam == null) return;
        transform.position = _cam.transform.position + new Vector3(-5.2f, YOffset, 0);
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null || CameraService.Instance == null || _col == null) return;
        if (!mouse.leftButton.wasPressedThisFrame) return;

        Vector2 world = CameraService.Instance.ScreenToWorld2D(mouse.position.ReadValue());
        if (_col.OverlapPoint(world))
        {
            _panel?.Toggle();
        }
    }
}
