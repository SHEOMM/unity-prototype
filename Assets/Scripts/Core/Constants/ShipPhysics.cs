/// <summary>
/// 우주선 발사체 물리 / 슬링샷 / 월드 경계 / 에너지 시스템 상수.
/// 모두 <c>Assets/Scripts/Ship/</c> 하위에서만 참조.
/// </summary>
public static partial class GameConstants
{
    public static class ShipPhysics
    {
        // ── 적분 / Sub-step ─────────────────────────────────────────

        /// <summary>고정 dt (120Hz). 가변 frametime을 흡수해 결정론적 중력 시뮬레이션을 보장.</summary>
        public const float FixedDt = 1f / 120f;

        /// <summary>한 Update에서 처리할 최대 sub-step (≈66ms 프레임 hitch까지 따라잡음).</summary>
        public const int MaxSubSteps = 8;

        // ── 중력 ────────────────────────────────────────────────────

        /// <summary>중력 분모 클램프 — 행성 중심에 너무 가까이 갈 때 무한대 가속도 방지.</summary>
        public const float MinGravityDistance = 0.8f;

        /// <summary>가속도 상한 — 슬링샷 효과로 발산하는 것을 막음.</summary>
        public const float MaxGravityForce = 50f;

        /// <summary>속도 상한 — 충돌 검출 누락(터널링) 방지를 겸함.</summary>
        public const float MaxSpeed = 30f;

        /// <summary>발사체 ↔ 행성 충돌 판정 반경 (행성 표면에 더해짐).</summary>
        public const float ShipCollisionRadius = 0.1f;

        /// <summary>
        /// 행성 충돌 시 차감되는 에너지 비율: <c>cost = planet.gravityStrength × 이 값</c>.
        /// 0.35로 통일 — 무거운 행성일수록 많이 깎임. 행성별 오버라이드 없음 (전역 단일).
        /// </summary>
        public const float GravityEnergyRatio = 0.35f;

        // ── 에너지 ──────────────────────────────────────────────────

        /// <summary>발사 시 시작 에너지. 0이 되면 라운드 자동 종료.</summary>
        public const float DefaultEnergy = 100f;

        /// <summary>초당 자연 소진 — 100/(33.33) ≈ 3초면 에너지 0. 도킹 중에도 동일 차감.</summary>
        public const float DefaultEnergyDrain = 33.33f;

        /// <summary>속도 항력 — 매 프레임 velocity *= (1 - DefaultDrag * dt)에 가깝게 적용.</summary>
        public const float DefaultDrag = 0.05f;

        // ── 발사력 ──────────────────────────────────────────────────

        /// <summary>슬링샷 당김 거리 × 이 값 = 초기 속도 크기.</summary>
        public const float LaunchPowerMultiplier = 5f;

        /// <summary>도킹 후 클릭 재발사 시 고정 속도 — 슬링샷 당김이 없어 일정 속도로 발사.</summary>
        public const float RelaunchPower = 20f;

        // ── 월드 경계 (카메라 가시영역과 무관) ───────────────────────────

        /// <summary>좌우 절대 경계 반폭. ±50을 벗어나면 OOB로 라운드 종료.</summary>
        public const float WorldBoundsX = 50f;

        /// <summary>하단 경계 — 지면 아래로 너무 떨어지면 종료.</summary>
        public const float WorldBoundsYMin = -15f;

        /// <summary>상단 경계 — 우주로 멀리 나가면 종료.</summary>
        public const float WorldBoundsYMax = 50f;

        // ── 슬링샷 (앵그리버드 스타일) ──────────────────────────────────

        /// <summary>당김 거리 상한 — 마우스가 멀리 가도 클램핑.</summary>
        public const float MaxPullDistance = 4f;

        /// <summary>당김 거리 하한 — 너무 짧은 발사 방지(미스 클릭 보호).</summary>
        public const float MinPullDistance = 0.3f;

        /// <summary>발사 원점 근처 클릭 히트 반경 — 이 안에서 클릭해야 슬링샷 시작.</summary>
        public const float PullGateRadius = 1.5f;

        // ── 궤적 미리보기 ───────────────────────────────────────────

        /// <summary>미리보기 시뮬레이션 스텝 수. FixedDt(1/120) × 300 = 2.5초 분량.</summary>
        public const int TrajectoryPreviewSteps = 300;

        /// <summary>미리보기 점 표시 개수. Steps 중 이 만큼 균등 샘플링해 점으로 표시.</summary>
        public const int TrajectoryPreviewDotCount = 40;
    }
}
