using UnityEngine;

/// <summary>
/// 상점 로직. 행성/유물을 골드로 구매한다.
/// Core/Data만 참조 — Combat/Map을 모른다.
/// 프로토타입: 간단한 자동 구매 → TODO: UI로 교체
/// </summary>
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public System.Action OnShopClosed;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void OpenShop()
    {
        Debug.Log("[상점] 상점 오픈 — TODO: 상품 목록 UI");

        // 프로토타입: 즉시 닫힘
        CloseShop();
    }

    public void CloseShop()
    {
        Debug.Log("[상점] 상점 닫힘");
        OnShopClosed?.Invoke();
    }
}
