/// <summary>
/// 시각 효과 상수 — 마법(SpellVisual) / 노이즈 애니메이션 / 시너지 비주얼.
/// </summary>
public static partial class GameConstants
{
    /// <summary>
    /// 마법 비주얼(ISpellVisual) 공통 파라미터.
    /// 화살·번개 등 행성별 visual이 공유하는 기본값.
    /// </summary>
    public static class VFX
    {
        /// <summary>마법 시작점 Y 오프셋 — 시전 위치 위쪽에서 시작.</summary>
        public const float DefaultSpellOriginHeight = 4f;

        /// <summary>마법 기본 지속(초) — 적에 도달해 데미지 발생까지 시간.</summary>
        public const float DefaultSpellDuration = 0.2f;

        // ── 화살 (ArrowVisual) ──

        public const float ArrowFlightDuration = 0.2f;
        public const float ArrowSpawnHeight = 5f;
        public const float ArrowArcHeight = 2f;       // 포물선 정점 높이

        // ── 번개 (LightningVisual) ──

        public const float LightningHeight = 8f;       // 번개 시작점 높이
        public const float LightningDuration = 0.25f;
        public const int LightningSegments = 8;        // 지그재그 마디 수
        public const float LightningZigzag = 0.4f;     // 좌우 흔들림 진폭
    }

    /// <summary>
    /// Perlin·Sin 기반 절차적 애니메이션 노이즈.
    /// "왜 15Hz인가?" 같은 매직 넘버를 의미명으로 표시.
    /// </summary>
    public static class VFXAnimation
    {
        // ── 슬링샷 최대 충전 시 미세 떨림 (ShipVisual.ShowSlingshotBand) ──

        /// <summary>Perlin 입력 시간 배율(Hz). 떨림 속도 결정.</summary>
        public const float SlingshotJitterFrequency = 15f;

        /// <summary>떨림 위치 오프셋 진폭 (월드 단위).</summary>
        public const float SlingshotJitterAmplitude = 0.05f;

        // ── 혜성 박동 (CometBody.Update) ──

        /// <summary>Sin 입력 시간 배율(Hz). 박동 속도.</summary>
        public const float CometPulseFrequency = 8f;

        /// <summary>박동 진폭 — 스케일이 1 ± 0.15 범위로 진동.</summary>
        public const float CometPulseAmplitude = 0.15f;

        /// <summary>혜성 클릭 충돌 영역 (CircleCollider2D 반지름).</summary>
        public const float CometColliderRadius = 0.5f;
    }

    /// <summary>
    /// 시너지 비주얼 6종(area_pulse / default / spawn_burst 등) 공통 파라미터.
    /// SynergyVisualHost가 visualId로 dispatch한 ISynergyVisual 구현체가 참조.
    /// </summary>
    public static class SynergyVisuals
    {
        /// <summary>모든 펄스 visual의 시작 반경 (0 → maxR로 확장).</summary>
        public const float PulseInitialRadius = 0.1f;

        // ── AreaPulseVisual (AoE 시너지 공용) ──
        // Rule.radius로 스케일링 — 큰 시너지일수록 더 넓고 더 길게.

        /// <summary>Rule.radius가 null일 때 폴백 반경.</summary>
        public const float AreaPulseRadiusFallback = 2f;

        /// <summary>너무 작은 반경 클램프 — 0.5보다 작으면 시각적 충격이 없음.</summary>
        public const float AreaPulseRadiusMin = 0.5f;

        /// <summary>지속 = 반경 × 이 값. 큰 반경에서 펄스가 느리게 펴지도록.</summary>
        public const float AreaPulseDurationPerRadius = 0.12f;

        public const float AreaPulseDurationMin = 0.25f;
        public const float AreaPulseDurationMax = 0.6f;

        public const float AreaPulseStartWidth = 0.22f;  // LineRenderer 시작 두께
        public const float AreaPulseEndWidth = 0.04f;     // 끝 두께 (확장하며 가늘어짐)
        public const int AreaPulseSegments = 40;          // 원 분할 수

        // ── DefaultSynergyVisual (visualId 미지정 / "default" 폴백) ──

        public const float DefaultDuration = 0.3f;
        public const float DefaultMaxRadius = 1.2f;
        public const float DefaultStartWidth = 0.1f;
        public const float DefaultEndWidth = 0.02f;
        public const int DefaultSegments = 24;

        // ── SpawnBurstVisual (Ally/Structure 소환 시너지) ──
        // spawnCount만큼 작은 펄스를 시간차로 연속 발생.

        /// <summary>각 개별 펄스의 지속.</summary>
        public const float SpawnBurstDuration = 0.35f;

        /// <summary>펄스 사이 지연 — 한 시너지에서 여러 개체 소환 시 연쇄 효과.</summary>
        public const float SpawnBurstDelayBetween = 0.08f;

        public const float SpawnBurstMaxRadius = 0.9f;
        public const float SpawnBurstStartWidth = 0.18f;
        public const float SpawnBurstEndWidth = 0.03f;
        public const int SpawnBurstSegments = 20;

        /// <summary>한 시너지에서 발생할 펄스 개수 상한 (rule.spawnCount를 클램프).</summary>
        public const int SpawnBurstCountMax = 5;

        /// <summary>가로 분산 최소 폭 — 너무 작은 spawnArea여도 이만큼은 퍼뜨림.</summary>
        public const float SpawnBurstSpanMin = 1.5f;

        /// <summary>rule.spawnArea.width × 이 비율 = 실제 분산 폭(span).</summary>
        public const float SpawnBurstSpawnAreaScale = 0.5f;

        /// <summary>rule.spawnArea가 null일 때 폴백 폭.</summary>
        public const float SpawnBurstSpawnAreaFallback = 2f;
    }
}
