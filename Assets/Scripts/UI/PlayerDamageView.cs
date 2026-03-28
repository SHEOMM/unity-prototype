using UnityEngine;

/// <summary>
/// 플레이어 피격 시 데미지 팝업을 표시한다.
/// PlayerState(모델)의 OnDamaged 이벤트를 구독.
/// </summary>
public class PlayerDamageView : MonoBehaviour
{
    void OnEnable()
    {
        if (PlayerState.Instance != null)
            PlayerState.Instance.OnDamaged += HandleDamaged;
    }

    void OnDisable()
    {
        if (PlayerState.Instance != null)
            PlayerState.Instance.OnDamaged -= HandleDamaged;
    }

    void HandleDamaged(float damage)
    {
        var cam = Camera.main;
        if (cam == null) return;
        DamagePopup.Spawn(cam.transform.position + Vector3.down * 3f, damage, Element.None);
    }
}
