using UnityEngine;

/// <summary>
/// 스코프 결과 UI 피드백. 시저지 발동 팝업 등을 표시한다.
/// ScopeController.OnScopeComplete 이벤트를 구독.
/// </summary>
public class ScopeFeedbackView : MonoBehaviour
{
    void OnEnable()
    {
        if (ScopeController.Instance != null)
            ScopeController.Instance.OnScopeComplete += HandleScopeComplete;
    }

    void OnDisable()
    {
        if (ScopeController.Instance != null)
            ScopeController.Instance.OnScopeComplete -= HandleScopeComplete;
    }

    void HandleScopeComplete(SlashResult result)
    {
        foreach (var syn in result.activatedSynergies)
            SynergyPopup.Show(syn.synergyName);
    }
}
