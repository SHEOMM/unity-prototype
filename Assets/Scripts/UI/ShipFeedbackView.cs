using UnityEngine;

/// <summary>
/// 우주선 비행 결과 UI 피드백. 시저지 발동 팝업 등을 표시한다.
/// ShipController.OnShipComplete 이벤트를 구독.
/// </summary>
public class ShipFeedbackView : MonoBehaviour
{
    void OnEnable()
    {
        if (ShipController.Instance != null)
            ShipController.Instance.OnShipComplete += HandleShipComplete;
    }

    void OnDisable()
    {
        if (ShipController.Instance != null)
            ShipController.Instance.OnShipComplete -= HandleShipComplete;
    }

    void HandleShipComplete(SlashResult result)
    {
        foreach (var syn in result.activatedSynergies)
            SynergyPopup.Show(syn.synergyName);
    }
}
