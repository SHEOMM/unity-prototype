/// <summary>
/// 전투 개체(Enemy/AllyUnit/Structure)의 데미지 피드백·사망 처리 상수.
/// EnemyView/AllyView/StructureView가 공통 참조.
/// </summary>
public static partial class GameConstants
{
    public static class Combat
    {
        /// <summary>피격 시 SpriteRenderer 색이 빨갛게 깜빡이는 총 지속(초).</summary>
        public const float DamageFlashDuration = 0.2f;

        /// <summary>플래시 깜빡임 주파수(Hz). 빈도가 높을수록 빠르게 점멸.</summary>
        public const float DamageFlashSpeed = 5f;

        /// <summary>일반 사망 — 시체가 이 시간 후 Destroy. 데미지 팝업 재생 여유.</summary>
        public const float DeathTimerNormal = 0.1f;

        /// <summary>경계(좌측 끝) 사망 — 즉시에 가까운 짧은 타이머. 화면 밖이라 시각 여유 불필요.</summary>
        public const float DeathTimerBoundary = 0.01f;
    }

    /// <summary>
    /// EnemySpawner의 기본 스폰 위치/타이밍. WaveDefinitionSO가 오버라이드 가능.
    /// </summary>
    public static class Spawner
    {
        /// <summary>적 스폰 Y(지상). 지면 아래쪽에서 등장.</summary>
        public const float DefaultSpawnY = -3f;

        /// <summary>X 범위 — 화면 우측 영역에서 무작위 스폰.</summary>
        public const float DefaultSpawnXMin = 7f;
        public const float DefaultSpawnXMax = 10f;

        /// <summary>웨이브 사이 대기(초). 다음 웨이브가 시작되기까지의 정적 간격.</summary>
        public const float DefaultDelayBetweenWaves = 5f;
    }
}
