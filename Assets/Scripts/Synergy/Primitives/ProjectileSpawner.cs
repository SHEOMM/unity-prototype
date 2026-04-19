using UnityEngine;

/// <summary>
/// 보조 발사체 생성을 요청. Gemini(별자리) 같은 "착지 시 추가 발사" 시너지 primitive.
/// Phase 0에서는 API shape만 정의 (실제 구현은 Phase 4에서 ShipController 확장).
/// </summary>
public static class ProjectileSpawner
{
    /// <summary>
    /// 비동기적으로 보조 발사체를 큐잉. 현재 구현은 placeholder — 경고 로그만.
    /// Phase 4에서 ShipController.RequestSecondaryLaunch(...)로 위임 예정.
    /// </summary>
    public static void SpawnSecondary(Vector2 origin, Vector2 direction, float energy)
    {
        Debug.LogWarning($"[ProjectileSpawner] Phase 4 미구현 — origin={origin}, dir={direction}, energy={energy}");
    }
}
