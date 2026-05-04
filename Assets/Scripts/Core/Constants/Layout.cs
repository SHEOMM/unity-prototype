/// <summary>
/// 렌더링 z 순서 정의. 동일 카메라 안에서 어떤 객체가 어떤 객체 위에 그려질지 결정.
/// 낮을수록 뒤(배경), 높을수록 앞(UI). 모든 SpriteRenderer/LineRenderer가 이 값을 참조.
/// </summary>
public static partial class GameConstants
{
    /// <summary>
    /// SpriteRenderer.sortingOrder / LineRenderer.sortingOrder에 직접 대입.
    /// 같은 layer 안에서는 값이 큰 것이 위에 그려진다.
    /// 값 사이 간격을 두어 후속 추가가 쉽도록 했다 (예: 5와 10 사이에 6, 7, 8 삽입 가능).
    /// </summary>
    public static class SortingOrder
    {
        // ── 배경 (게임 요소보다 항상 뒤) ──

        /// <summary>지상 배경의 가장 뒤 레이어. 다층 패럴랙스가 -10부터 -3까지 8 레이어 사용.</summary>
        public const int BackgroundGroundBase = -10;

        /// <summary>천상(우주) 배경. 지상 위에 덮여 sky 부분을 가린다.</summary>
        public const int BackgroundSpace = -2;

        /// <summary>일반 배경 기본값.</summary>
        public const int Background = 0;

        // ── 천체 / 적 본체 ──

        /// <summary>궤도 LineRenderer (행성보다 뒤).</summary>
        public const int OrbitPath = 1;

        /// <summary>항성(레거시 — Phase 9에서 개념 제거됨).</summary>
        public const int StarBody = 2;

        /// <summary>행성/적 본체. 같은 값이라 공평하게 정렬되며, 화면 Y 위치로 자연 정렬됨.</summary>
        public const int PlanetBody = 5;
        public const int EnemyBody = 5;

        public const int SatelliteBody = 6;
        public const int CometBody = 8;

        // ── HP 바 / 라벨 (개체 위에) ──

        public const int HPBarBackground = 8;
        public const int HPBarFill = 9;

        /// <summary>일반 라벨 (행성 이름 등).</summary>
        public const int Label = 10;

        public const int HPBarText = 11;
        public const int EnemyLabel = 12;

        // ── 마법 이펙트 / 데미지 팝업 ──

        /// <summary>주문/시너지 비주얼 효과의 기준 레이어. 발사체 본체도 여기.</summary>
        public const int SpellEffect = 15;

        /// <summary>번개(체인) — SpellEffect 위. 다른 이펙트 위에 그려져 강조.</summary>
        public const int LightningEffect = 16;

        /// <summary>피해 숫자 팝업 — 모든 게임플레이 요소 위.</summary>
        public const int DamagePopup = 20;

        // ── Reward 카드 (20–25 대역) ──

        public const int RewardCardBg = 20;

        /// <summary>카드 중앙 영역 — 배지/아이콘/궤도 링.</summary>
        public const int RewardCardMid = 21;

        public const int RewardCardText = 22;

        /// <summary>RewardSceneBoot의 화면 상단 큰 타이틀.</summary>
        public const int RewardCardTitle = 25;

        // ── Synergy / Toast (24–25 대역) ──

        /// <summary>화면 전체 플래시 오버레이 — Toast보다 한 칸 아래로.</summary>
        public const int SynergyScreenFlash = 24;

        /// <summary>SynergyToastView 전용. 가장 위 레이어 중 하나.</summary>
        public const int SynergyToast = 25;

        // ── Cosmos 패널 (28–46 대역) ──
        // 패널 자체 / 슬롯 / 토큰 / 닫기 X 버튼이 z순으로 적층

        public const int CosmosPanelBg = 28;
        public const int CosmosSlotBg = 30;
        public const int CosmosSlotPreview = 31;
        public const int CosmosInventoryBg = 30;
        public const int CosmosTokenBg = 42;
        public const int CosmosTokenIcon = 43;

        /// <summary>패널 타이틀 / 슬롯·토큰·인벤토리 라벨 공통.</summary>
        public const int CosmosLabel = 44;

        public const int CosmosCloseBg = 45;
        public const int CosmosCloseText = 46;

        // ── Cosmos Map 버튼 (50–51, MapView 노드보다 위) ──

        public const int CosmosMapButtonBg = 50;
        public const int CosmosMapButtonLabel = 51;
    }
}
