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

    // 카메라 뷰 기준 비율 (halfWidth/halfHeight 상대값, -1~+1)
    const float XAnchor = -0.78f;
    const float YAnchor = 0.88f;
    const float Width = 2.4f;
    const float Height = 0.7f;

    public void Bind(CosmosPanelView panel)
    {
        _panel = panel;
    }

    void Start()
    {
        _cam = CameraService.Instance?.Camera;
        if (_cam == null) Debug.LogWarning("[CosmosMapButton] CameraService.Camera 없음 — 위치 고정 불가");

        _bg = gameObject.AddComponent<SpriteRenderer>();
        _bg.sprite = UIFactory.MakePixel();
        _bg.color = new Color(0.2f, 0.5f, 0.8f, 0.95f);
        _bg.sortingOrder = 50;  // MapView 노드보다 위
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
        if (mr != null) mr.sortingOrder = 51;

        _col = GetComponent<BoxCollider2D>();
        _col.size = Vector2.one;

        ApplyCameraAnchoredPosition();
    }

    void LateUpdate()
    {
        ApplyCameraAnchoredPosition();
    }

    void ApplyCameraAnchoredPosition()
    {
        if (_cam == null) _cam = CameraService.Instance?.Camera;
        if (_cam == null) return;

        float halfH = _cam.orthographicSize;
        float halfW = halfH * _cam.aspect;
        var camPos = _cam.transform.position;
        transform.position = new Vector3(
            camPos.x + halfW * XAnchor + Width * 0.5f,
            camPos.y + halfH * YAnchor - Height * 0.5f,
            0);
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
