using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 우주선 발사 입력. 천상 하단 고정점에서 발사된다.
/// 클릭+드래그로 발사 방향과 파워를 결정하고, 놓으면 발사한다.
/// </summary>
public class ShipInput : MonoBehaviour
{
    public float celestialYMin = 0f;

    public Vector2 LaunchOrigin => new Vector2(0f, celestialYMin + 0.2f);

    public System.Action<Vector2> OnAimStart;
    public System.Action<Vector2, Vector2> OnAimUpdate;
    public System.Action<Vector2, Vector2, float> OnLaunch;
    public System.Action OnAimCancel;

    private bool _aiming;
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

        if (_mouse.leftButton.wasPressedThisFrame && !_aiming)
        {
            _aiming = true;
            OnAimStart?.Invoke(LaunchOrigin);
        }

        if (_aiming && _mouse.leftButton.isPressed)
        {
            OnAimUpdate?.Invoke(LaunchOrigin, mp);
        }

        if (_mouse.leftButton.wasReleasedThisFrame && _aiming)
        {
            _aiming = false;
            Vector2 delta = mp - LaunchOrigin;
            float dist = delta.magnitude;

            if (dist < 0.3f)
            {
                Debug.Log($"[ShipInput] 조준 취소: 드래그 거리 {dist:F2} < 0.3");
                OnAimCancel?.Invoke();
                return;
            }

            float power = dist * GameConstants.ShipPhysics.LaunchPowerMultiplier;
            Debug.Log($"[ShipInput] 발사 요청: dist={dist:F2}, power={power:F1}");
            OnLaunch?.Invoke(LaunchOrigin, delta.normalized, power);
        }
    }

    /// <summary>외부에서 조준을 강제로 취소할 때 호출</summary>
    public void ForceCancel()
    {
        if (_aiming)
        {
            _aiming = false;
            OnAimCancel?.Invoke();
        }
    }
}
