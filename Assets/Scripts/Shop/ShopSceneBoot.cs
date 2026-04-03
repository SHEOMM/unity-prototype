using UnityEngine;

/// <summary>
/// ShopScene 진입 시 자동 초기화. 프로토타입: 즉시 맵으로 복귀.
/// </summary>
public class ShopSceneBoot : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[상점] 상점 오픈 — TODO: 상품 UI");
        Invoke(nameof(ReturnToMap), 1f);
    }

    void ReturnToMap()
    {
        GameManager.Instance?.EnterPhase(GamePhase.Map);
    }
}
