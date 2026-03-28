using UnityEngine;

/// <summary>
/// 슬래시 결과 UI 피드백. 시저지 발동 팝업 등을 표시한다.
/// SlashController.OnSlashComplete 이벤트를 구독.
/// </summary>
public class SlashFeedbackView : MonoBehaviour
{
    void OnEnable()
    {
        if (SlashController.Instance != null)
            SlashController.Instance.OnSlashComplete += HandleSlashComplete;
    }

    void OnDisable()
    {
        if (SlashController.Instance != null)
            SlashController.Instance.OnSlashComplete -= HandleSlashComplete;
    }

    void HandleSlashComplete(SlashResult result)
    {
        foreach (var syn in result.activatedSynergies)
            SynergyPopup.Show(syn.synergyName);
    }
}
