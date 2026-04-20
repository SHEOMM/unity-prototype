/// <summary>
/// 배경을 조회할 때 쓰는 키. 씬·룸타입별로 다른 분위기를 뽑을 수 있도록 세분화.
/// BackgroundBindingTable이 이 키로 SpaceBackgroundSet / GroundBackgroundSet 조회.
/// </summary>
public enum BackgroundKey
{
    Map,          // MapScene 전용 (우주 배경만 사용)
    Combat,       // CombatScene 일반 전투
    CombatElite,  // 엘리트 룸
    CombatBoss,   // 보스 룸
    Rest,         // 휴식 씬
    Shop,         // 상점 씬
    Reward,       // 보상 씬
}
