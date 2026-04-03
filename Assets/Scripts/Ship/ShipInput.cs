using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 우주선 발사 입력. 클릭+드래그로 방향과 파워를 설정하고, 놓으면 발사한다.
/// </summary>
public class ShipInput : MonoBehaviour
{
    public float celestialYMin = 0f;

    public System.Action<Vector2> OnAimStart;
    public System.Action<Vector2, Vector2> OnAimUpdate;
    public System.Action<Vector2, Vector2, float> OnLaunch;

    private bool _aiming;
    private Vector2 _aimStart;
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

        if (_mouse.leftButton.wasPressedThisFrame && mp.y >= celestialYMin)
        {
            _aiming = true;
            _aimStart = mp;
            OnAimStart?.Invoke(mp);
        }

        if (_aiming)
            OnAimUpdate?.Invoke(_aimStart, mp);

        if (_mouse.leftButton.wasReleasedThisFrame && _aiming)
        {
            _aiming = false;
            Vector2 delta = mp - _aimStart;
            float power = delta.magnitude * GameConstants.ShipPhysics.LaunchPowerMultiplier;
            OnLaunch?.Invoke(_aimStart, delta.normalized, power);
        }
    }
}
