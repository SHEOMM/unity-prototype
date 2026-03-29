using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 원형 스코프 입력 감지. 마우스 위치를 추적하고 클릭 시 스냅샷 이벤트를 발행한다.
/// </summary>
public class ScopeInput : MonoBehaviour
{
    public float celestialYMin = 0f;

    /// <summary>마우스 위치 (매 프레임)</summary>
    public System.Action<Vector2> OnCursorMove;
    /// <summary>클릭 시 스냅샷 요청</summary>
    public System.Action<Vector2> OnSnapshot;

    private Camera _cam;
    private Mouse _mouse;

    void Start()
    {
        _cam = Camera.main;
        _mouse = Mouse.current;
    }

    void Update()
    {
        if (_mouse == null || _cam == null) return;
        Vector2 mp = _cam.ScreenToWorldPoint(_mouse.position.ReadValue());

        OnCursorMove?.Invoke(mp);

        if (_mouse.leftButton.wasPressedThisFrame && mp.y >= celestialYMin)
            OnSnapshot?.Invoke(mp);
    }
}
