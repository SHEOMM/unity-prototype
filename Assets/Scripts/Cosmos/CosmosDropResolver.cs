/// <summary>
/// 드래그 드롭 결과 분류. 실제 상태 변경은 호출자(CosmosPanelView)가 enum에 맞춰 CosmosService 호출.
/// 분기 논리를 한 곳에 모아 PanelView를 얇게 유지하고 테스트를 쉽게 한다.
/// </summary>
public enum DropResult
{
    SnapBack,    // 원위치 (같은 슬롯·void 드롭·인벤토리→인벤토리)
    MoveOrSwap,  // 궤도→궤도 (Swap이 이동/교환 동시 처리)
    Assign,      // 인벤토리→궤도
    Unassign,    // 궤도→인벤토리
}

public static class CosmosDropResolver
{
    public static DropResult Resolve(OrbitSO originOrbit, CosmosOrbitSlot targetSlot, bool inventoryHit)
    {
        if (targetSlot != null)
        {
            if (originOrbit == targetSlot.Orbit) return DropResult.SnapBack;
            return originOrbit != null ? DropResult.MoveOrSwap : DropResult.Assign;
        }
        if (inventoryHit)
            return originOrbit != null ? DropResult.Unassign : DropResult.SnapBack;
        return DropResult.SnapBack;
    }
}
