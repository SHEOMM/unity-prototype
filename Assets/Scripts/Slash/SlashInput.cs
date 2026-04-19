using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Input System 기반 드래그 입력 감지.
/// 드래그 시작/진행/종료 이벤트를 발행한다.
/// </summary>
public class SlashInput : MonoBehaviour
{
    public float celestialYMin = 0f;
    public System.Action<Vector2> OnDragStart;
    public System.Action<Vector2> OnDragUpdate;
    public System.Action<Vector2, Vector2> OnDragEnd;

    private bool _dragging;
    private Vector2 _start;
    private Mouse _mouse;

    void Start()
    {
        _mouse = Mouse.current;
    }

    void Update()
    {
        if (_mouse == null || CameraService.Instance == null) return;
        Vector2 mp = CameraService.Instance.ScreenToWorld2D(_mouse.position.ReadValue());

        if (_mouse.leftButton.wasPressedThisFrame && mp.y >= celestialYMin)
        {
            _dragging = true;
            _start = mp;
            OnDragStart?.Invoke(mp);
        }

        if (_dragging)
            OnDragUpdate?.Invoke(mp);

        if (_mouse.leftButton.wasReleasedThisFrame && _dragging)
        {
            _dragging = false;
            OnDragEnd?.Invoke(_start, mp);
        }
    }
}
