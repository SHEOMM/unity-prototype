using UnityEngine;

/// <summary>
/// 명명된 헥스 팔레트 — 인라인 <c>new Color(r,g,b,a)</c> 대신 의도가 보이는 이름을 참조.
/// 신규 색 추가 시: <c>Hex("#RRGGBB", alpha)</c> 헬퍼로 한 줄 정의.
/// 알파 0~1, 헥스 6자리. 잘못된 헥스는 magenta로 폴백되어 누락이 즉시 눈에 띈다.
/// </summary>
public static partial class GameConstants
{
    public static class Colors
    {
        // ── 헬퍼 ──

        /// <summary>
        /// 헥스 문자열(예: "#FFB340")과 α(0~1)로 Color 생성.
        /// 잘못된 헥스는 magenta(누락 신호) 폴백.
        /// </summary>
        static Color Hex(string hex, float alpha = 1f)
        {
            if (!ColorUtility.TryParseHtmlString(hex, out var c)) c = Color.magenta;
            c.a = alpha;
            return c;
        }

        // ── HP / 라벨 / 궤도 ────────────────────────────────────────

        /// <summary>행성 충돌 시 일시 강조(노란 빛).</summary>
        public static readonly Color PlanetHighlight = Hex("#FFFF80");

        /// <summary>HP 바 풀 — 초록.</summary>
        public static readonly Color HPFull  = Hex("#33D933", 0.9f);

        /// <summary>HP 바 빈 — 빨강. 보간으로 풀↔빈을 잇는다.</summary>
        public static readonly Color HPEmpty = Hex("#D93333", 0.9f);

        /// <summary>HP 바 배경 — 짙은 회색.</summary>
        public static readonly Color HPBarBg = Hex("#333333", 0.7f);

        /// <summary>천상/지상 구분선 — 옅은 흰색.</summary>
        public static readonly Color DividerLine = Hex("#FFFFFF", 0.2f);

        /// <summary>궤도 LineRenderer — 매우 옅은 흰색 (가독성을 해치지 않을 정도).</summary>
        public static readonly Color OrbitPath = Hex("#FFFFFF", 0.15f);

        public static readonly Color SkyLabel    = Hex("#B3CCFF", 0.3f);   // 천상 라벨
        public static readonly Color GroundLabel = Hex("#FFB3B3", 0.3f);   // 지상 라벨
        public static readonly Color StarLabel   = Hex("#FFFFCC", 0.8f);
        public static readonly Color PlanetLabel = Hex("#FFFFFF", 0.9f);
        public static readonly Color EnemyLabel  = Hex("#FFCCCC", 0.85f);

        // ── 슬링샷 밴드 (ShipVisual.ShowSlingshotBand) ─────────────────

        /// <summary>당김 0% — 옅은 크림. 비활성에 가까운 색.</summary>
        public static readonly Color SlingshotBandIdle    = Hex("#FFFFB3", 0.35f);

        /// <summary>당김 100% 직전 — 호박색. Idle과 보간되어 강도 표현.</summary>
        public static readonly Color SlingshotBandCharged = Hex("#FFB340", 0.90f);

        /// <summary>최대 충전 시작색 — 빨강. 떨림과 함께 위험성 강조.</summary>
        public static readonly Color SlingshotBandMaxStart = Hex("#FF4033", 0.95f);

        /// <summary>최대 충전 끝색 — 주황 페이드. 그라디언트로 시작에서 페이드.</summary>
        public static readonly Color SlingshotBandMaxEnd   = Hex("#FF6633", 0.55f);

        /// <summary>슬링샷 비활성 시 LineRenderer 시작색 (CreateBand에서 초기화).</summary>
        public static readonly Color SlingshotBandRest    = Hex("#FFE666", 0.50f);

        /// <summary>슬링샷 비활성 시 끝색 — 더 투명.</summary>
        public static readonly Color SlingshotBandRestEnd = Hex("#FFE666", 0.15f);

        // ── 에너지 게이지 (ShipVisual.UpdateEnergyGauge) ────────────────
        // ratio ∈ [0, 1]: 0~50% Low→Mid 보간, 50~100% Mid→High 보간

        public static readonly Color EnergyGaugeBg   = Hex("#262626", 0.75f);

        /// <summary>50% 초과 — 초록 (충분한 에너지).</summary>
        public static readonly Color EnergyGaugeHigh = Hex("#4DE666", 0.90f);

        /// <summary>~50% — 노랑 (경고).</summary>
        public static readonly Color EnergyGaugeMid  = Hex("#F2D919", 0.90f);

        /// <summary>0~50% — 빨강 (긴급).</summary>
        public static readonly Color EnergyGaugeLow  = Hex("#E63326", 0.90f);

        // ── 발사체 본체·트레일 (ShipVisual.SpawnShip) ──────────────────

        public static readonly Color ShipBody       = Hex("#FFE680");      // 본체 (옅은 금)
        public static readonly Color ShipTrailStart = Hex("#FFCC4D", 0.8f); // 트레일 시작 (선명)
        public static readonly Color ShipTrailEnd   = Hex("#FF801A", 0.0f); // 트레일 끝 (완전 투명)

        // ── 슬링샷 원점 인디케이터 (ShipVisual.CreateOriginIndicator) ──

        public static readonly Color OriginDot     = Hex("#FFD959", 0.9f);   // 발사 원점 점
        public static readonly Color GateRing      = Hex("#FFD966", 0.35f);  // 클릭 게이트 링 (PullGateRadius 영역)
        public static readonly Color TrajectoryDot = Hex("#FFF299", 0.8f);   // 궤적 미리보기 점 (알파는 거리에 따라 페이드)

        // ── Cosmos 패널 (CosmosPanelChrome / CosmosInventoryArea) ─────

        public static readonly Color CosmosPanelBg     = Hex("#0D1019", 0.92f); // 깊은 청흑 — 패널 본체
        public static readonly Color CosmosInventoryBg = Hex("#191F29", 0.85f); // 어두운 슬레이트 — 인벤토리 영역
        public static readonly Color CosmosTitle       = Hex("#FFF299");        // 따뜻한 크림 — 타이틀 텍스트
        public static readonly Color CosmosLabelText   = Hex("#D9D9E6");        // 차분한 보조 라벨
        public static readonly Color CosmosCloseBg     = Hex("#993333", 0.9f);  // 닫기 X 배경 (붉은 톤)

        // ── Reward 카드 타입 (RewardManager) ───────────────────────────

        public static readonly Color RewardOrbit  = Hex("#8CE6FF");  // 시안 — 궤도
        public static readonly Color RewardPlanet = Hex("#FFD966");  // 금빛 — 행성
        public static readonly Color RewardRelic  = Hex("#E68CFF");  // 보라 — 유물

        // ── 시너지 토스트 (SynergyToastView) ───────────────────────────

        /// <summary>시너지 발동 토스트 텍스트 — 황금빛으로 강조.</summary>
        public static readonly Color SynergyToastText = Hex("#FFE64D");

        // ── 시너지 원소 (SynergyVisualElementPalette가 이 색들을 dispatch) ──
        // Element enum과 1:1. None은 ElementDefault 폴백.

        public static readonly Color ElementFire     = Hex("#FF732E");  // 주황빛 빨강
        public static readonly Color ElementWater    = Hex("#4DA6FF");  // 차분한 파랑
        public static readonly Color ElementWind     = Hex("#8CF2F2");  // 청록 (시안)
        public static readonly Color ElementEarth    = Hex("#A67338");  // 갈색
        public static readonly Color ElementDarkness = Hex("#994DD9");  // 보라

        /// <summary>None / 기타 — 따뜻한 금빛 폴백.</summary>
        public static readonly Color ElementDefault  = Hex("#FFD94D");
    }
}
