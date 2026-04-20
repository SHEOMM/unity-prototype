/// <summary>
/// 배경 세트 해결자. BackgroundBindingTable을 런타임 조회해 key에 해당하는 Set 반환.
/// BindingTable은 GameManager.Awake가 정적 속성으로 주입 (PersistentScene Inspector에서 바인딩).
///
/// 호출자:
///  - CombatSceneBoot: ResolveSpace + ResolveGround (천상 + 지상)
///  - MapSceneBoot: ResolveSpace 만
/// </summary>
public static class BackgroundResolver
{
    /// <summary>GameManager.Awake가 주입. 전투/씬 전환 동안 영속.</summary>
    public static BackgroundBindingTable BindingTable { get; set; }

    public static SpaceBackgroundSet ResolveSpace(BackgroundKey key)
    {
        return BindingTable != null ? BindingTable.ResolveSpace(key) : null;
    }

    public static GroundBackgroundSet ResolveGround(BackgroundKey key)
    {
        return BindingTable != null ? BindingTable.ResolveGround(key) : null;
    }
}
