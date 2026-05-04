using UnityEngine;

/// <summary>
/// 게임 전체에서 공유되는 상수 정의.
/// 매직 넘버를 이름 있는 상수로 관리하여 의도를 명확히 한다.
/// </summary>
public static class GameConstants
{
    // ── 화면 레이아웃 ──
    public const float ScreenBoundaryLeft = -12f;
    public const float DividerLineExtent = 15f;

    // ── 정렬 순서 (낮을수록 뒤에 렌더링) ──
    public static class SortingOrder
    {
        // 배경 (게임 요소보다 항상 뒤)
        public const int BackgroundGroundBase = -10;  // 지상 layer 0 (가장 뒤). 최대 8레이어까지 ~ -3
        public const int BackgroundSpace = -2;         // 천상 배경 (지상 위에 덮여 sky 부분 가림)

        public const int Background = 0;
        public const int OrbitPath = 1;
        public const int StarBody = 2;
        public const int PlanetBody = 5;
        public const int EnemyBody = 5;
        public const int SatelliteBody = 6;
        public const int CometBody = 8;
        public const int HPBarBackground = 8;
        public const int HPBarFill = 9;
        public const int Label = 10;
        public const int HPBarText = 11;
        public const int EnemyLabel = 12;
        public const int SpellEffect = 15;
        public const int LightningEffect = 16;
        public const int DamagePopup = 20;

        // Reward 카드 (20-25 대역)
        public const int RewardCardBg = 20;
        public const int RewardCardMid = 21;        // Badge/Icon/OrbitRing
        public const int RewardCardText = 22;
        public const int RewardCardTitle = 25;      // RewardSceneBoot 타이틀

        // Synergy / Toast (24-25 대역)
        public const int SynergyScreenFlash = 24;   // Toast 바로 아래
        public const int SynergyToast = 25;         // SynergyToastView 전용

        // Cosmos 패널 (28-46 대역)
        public const int CosmosPanelBg = 28;
        public const int CosmosSlotBg = 30;
        public const int CosmosSlotPreview = 31;
        public const int CosmosInventoryBg = 30;
        public const int CosmosTokenBg = 42;
        public const int CosmosTokenIcon = 43;
        public const int CosmosLabel = 44;          // 패널 타이틀 + 슬롯/토큰/인벤토리 라벨 공용
        public const int CosmosCloseBg = 45;
        public const int CosmosCloseText = 46;

        // Cosmos Map 버튼 (50-51 대역, MapView 노드보다 위)
        public const int CosmosMapButtonBg = 50;
        public const int CosmosMapButtonLabel = 51;
    }

    // ── 천체 라벨 ──
    public static class CelestialLabel
    {
        public const int FontSize = 64;
        public const float CharacterSize = 0.12f;
        public const float PlanetYOffset = -1.2f;
        public const float StarYOffset = -1.5f;
        public const float ScaleMultiplier = 0.5f;
        public const float MinScaleThreshold = 0.01f;
    }

    // ── 적 UI ──
    public static class EnemyUI
    {
        public const float LabelYOffset = -0.8f;
        public const float LabelScale = 0.4f;
        public const int LabelFontSize = 48;
        public const float HPBarYOffset = 0.7f;
        public const float HPBarWidth = 0.7f;
        public const float HPBarHeight = 0.06f;
        public const float HPTextScale = 0.3f;
    }

    // ── 전투 ──
    public static class Combat
    {
        public const float DamageFlashDuration = 0.2f;
        public const float DamageFlashSpeed = 5f;
        public const float DeathTimerNormal = 0.1f;
        public const float DeathTimerBoundary = 0.01f;
    }

    // ── 데미지 팝업 ──
    public static class Popup
    {
        public const float Lifetime = 0.8f;
        public const float SpawnYOffset = 0.5f;
        public const float SpawnXJitter = 0.3f;
        public const float InitialScale = 0.12f;
        public const float VelocityY = 2f;
        public const float Gravity = 3f;
        public const float ScaleGrowth = 0.3f;
    }

    // ── 궤도 ──
    public static class Orbit
    {
        public const int PathSegments = 48;
        public const float PathWidth = 0.02f;
        public const float PathAlpha = 0.15f;
    }

    // ── 행성 애니메이션 ──
    public static class PlanetAnim
    {
        public const float PulseFrequency = 3f;
        public const float PulseAmplitude = 0.1f;
        public const float ColliderRadius = 0.34f;
    }

    // ── VFX ──
    public static class VFX
    {
        public const float DefaultSpellOriginHeight = 4f;
        public const float DefaultSpellDuration = 0.2f;
        public const float ArrowFlightDuration = 0.2f;
        public const float ArrowSpawnHeight = 5f;
        public const float ArrowArcHeight = 2f;
        public const float LightningHeight = 8f;
        public const float LightningDuration = 0.25f;
        public const int LightningSegments = 8;
        public const float LightningZigzag = 0.4f;
    }

    // ── 색상 ──
    /// <summary>
    /// 명명된 헥스 팔레트. 인라인 `new Color(r,g,b,a)` 대신 의도가 보이는 이름을 참조.
    /// 추가 시: <see cref="Hex"/> 헬퍼로 hex 문자열 + α를 받아 readonly Color로 정의.
    /// </summary>
    public static class Colors
    {
        // ── 헬퍼 ──
        /// <summary>
        /// 헥스 문자열(예: "#FFB340")과 α(0~1)로 Color 생성. 잘못된 헥스는 magenta 폴백.
        /// </summary>
        static Color Hex(string hex, float alpha = 1f)
        {
            if (!ColorUtility.TryParseHtmlString(hex, out var c)) c = Color.magenta;
            c.a = alpha;
            return c;
        }

        // ── HP / 라벨 / 궤도 (기존 유지) ──
        public static readonly Color PlanetHighlight = Hex("#FFFF80");        // 노란 강조
        public static readonly Color HPFull          = Hex("#33D933", 0.9f);
        public static readonly Color HPEmpty         = Hex("#D93333", 0.9f);
        public static readonly Color HPBarBg         = Hex("#333333", 0.7f);
        public static readonly Color DividerLine     = Hex("#FFFFFF", 0.2f);
        public static readonly Color OrbitPath       = Hex("#FFFFFF", 0.15f);
        public static readonly Color SkyLabel        = Hex("#B3CCFF", 0.3f);
        public static readonly Color GroundLabel     = Hex("#FFB3B3", 0.3f);
        public static readonly Color StarLabel       = Hex("#FFFFCC", 0.8f);
        public static readonly Color PlanetLabel     = Hex("#FFFFFF", 0.9f);
        public static readonly Color EnemyLabel      = Hex("#FFCCCC", 0.85f);

        // ── 슬링샷 밴드 (ShipVisual.ShowSlingshotBand) ──
        public static readonly Color SlingshotBandIdle    = Hex("#FFFFB3", 0.35f);  // 옅은 크림
        public static readonly Color SlingshotBandCharged = Hex("#FFB340", 0.90f);  // 호박
        public static readonly Color SlingshotBandMaxStart = Hex("#FF4033", 0.95f); // 최대 충전 빨강
        public static readonly Color SlingshotBandMaxEnd   = Hex("#FF6633", 0.55f); // 최대 충전 끝(주황 페이드)
        public static readonly Color SlingshotBandRest     = Hex("#FFE666", 0.50f); // 비활성 시작
        public static readonly Color SlingshotBandRestEnd  = Hex("#FFE666", 0.15f); // 비활성 끝

        // ── 에너지 게이지 (ShipVisual.UpdateEnergyGauge) ──
        public static readonly Color EnergyGaugeBg    = Hex("#262626", 0.75f);
        public static readonly Color EnergyGaugeHigh  = Hex("#4DE666", 0.90f);  // 50% 초과: 초록
        public static readonly Color EnergyGaugeMid   = Hex("#F2D919", 0.90f);  // ~50%: 노랑
        public static readonly Color EnergyGaugeLow   = Hex("#E63326", 0.90f);  // 0~50%: 빨강

        // ── 발사체 본체·트레일 (ShipVisual.SpawnShip) ──
        public static readonly Color ShipBody        = Hex("#FFE680");
        public static readonly Color ShipTrailStart  = Hex("#FFCC4D", 0.8f);
        public static readonly Color ShipTrailEnd    = Hex("#FF801A", 0.0f);

        // ── 슬링샷 원점 인디케이터 (ShipVisual.CreateOriginIndicator) ──
        public static readonly Color OriginDot       = Hex("#FFD959", 0.9f);
        public static readonly Color GateRing        = Hex("#FFD966", 0.35f);
        public static readonly Color TrajectoryDot   = Hex("#FFF299", 0.8f);

        // ── Cosmos 패널 (CosmosPanelChrome / CosmosInventoryArea) ──
        public static readonly Color CosmosPanelBg     = Hex("#0D1019", 0.92f);  // 깊은 청흑
        public static readonly Color CosmosInventoryBg = Hex("#191F29", 0.85f);  // 어두운 슬레이트
        public static readonly Color CosmosTitle       = Hex("#FFF299");         // 따뜻한 크림 타이틀
        public static readonly Color CosmosLabelText   = Hex("#D9D9E6");         // 차분한 보조 라벨
        public static readonly Color CosmosCloseBg     = Hex("#993333", 0.9f);   // 닫기 X 배경(붉은 톤)

        // ── Reward 카드 타입 (RewardManager) ──
        public static readonly Color RewardOrbit  = Hex("#8CE6FF");  // 시안 — 궤도
        public static readonly Color RewardPlanet = Hex("#FFD966");  // 금빛 — 행성
        public static readonly Color RewardRelic  = Hex("#E68CFF");  // 보라 — 유물

        // ── 시너지 토스트 (SynergyToastView) ──
        public static readonly Color SynergyToastText = Hex("#FFE64D");

        // ── 시너지 원소 (SynergyVisualElementPalette가 위임) ──
        public static readonly Color ElementFire     = Hex("#FF732E");
        public static readonly Color ElementWater    = Hex("#4DA6FF");
        public static readonly Color ElementWind     = Hex("#8CF2F2");
        public static readonly Color ElementEarth    = Hex("#A67338");
        public static readonly Color ElementDarkness = Hex("#994DD9");
        public static readonly Color ElementDefault  = Hex("#FFD94D");  // None=gold 폴백
    }

    // ── VFX 애니메이션 (Perlin·Sin 노이즈) ──
    public static class VFXAnimation
    {
        // 슬링샷 최대 충전 시 미세 떨림 (ShipVisual.ShowSlingshotBand)
        public const float SlingshotJitterFrequency = 15f;     // Hz
        public const float SlingshotJitterAmplitude = 0.05f;   // 월드 단위

        // 혜성 박동 — Sin 기반 스케일 펄스 (CometBody.Update)
        public const float CometPulseFrequency = 8f;     // Hz
        public const float CometPulseAmplitude = 0.15f;  // 스케일 배수 (1 ± 0.15)
        public const float CometColliderRadius = 0.5f;   // CircleCollider2D 반지름
    }

    // ── 시너지 비주얼 (공용 6 visual 공유 상수) ──
    public static class SynergyVisuals
    {
        // 공통: 펄스 시작 반경 (모든 visual이 0.1f부터 maxR로 확장)
        public const float PulseInitialRadius = 0.1f;

        // AreaPulseVisual — Rule.radius로 스케일링
        public const float AreaPulseRadiusFallback   = 2f;
        public const float AreaPulseRadiusMin        = 0.5f;
        public const float AreaPulseDurationPerRadius = 0.12f;  // 반경 1 = 0.12초
        public const float AreaPulseDurationMin      = 0.25f;
        public const float AreaPulseDurationMax      = 0.6f;
        public const float AreaPulseStartWidth       = 0.22f;
        public const float AreaPulseEndWidth         = 0.04f;
        public const int   AreaPulseSegments         = 40;

        // DefaultSynergyVisual — 폴백 펄스
        public const float DefaultDuration   = 0.3f;
        public const float DefaultMaxRadius  = 1.2f;
        public const float DefaultStartWidth = 0.1f;
        public const float DefaultEndWidth   = 0.02f;
        public const int   DefaultSegments   = 24;

        // SpawnBurstVisual — 다중 펄스
        public const float SpawnBurstDuration       = 0.35f;
        public const float SpawnBurstDelayBetween   = 0.08f;
        public const float SpawnBurstMaxRadius      = 0.9f;
        public const float SpawnBurstStartWidth     = 0.18f;
        public const float SpawnBurstEndWidth       = 0.03f;
        public const int   SpawnBurstSegments       = 20;
        public const int   SpawnBurstCountMax       = 5;
        public const float SpawnBurstSpanMin        = 1.5f;     // 가로 분산 최소 폭
        public const float SpawnBurstSpawnAreaScale = 0.5f;     // rule.spawnArea.width × 이 비율
        public const float SpawnBurstSpawnAreaFallback = 2f;
    }

    // ── 맵 생성 (방 타입 누적 분포 / 레이아웃) ──
    public static class MapGeneration
    {
        // 방 타입 누적 확률 (CDF). 0~1 난수가 어느 구간에 속하는지로 결정.
        // Combat 55% | Elite 10% | Rest 12% | Shop 8% | Event 15%
        public const float CombatRoomCdf = 0.55f;
        public const float EliteRoomCdf  = 0.65f;
        public const float RestRoomCdf   = 0.77f;
        public const float ShopRoomCdf   = 0.85f;

        // 방 타입 재시도 (연속 규칙 위반 회피)
        public const int RoomTypeMaxAttempts = 10;

        // 맵 좌표 배치
        public const float NodeColSpacing   = 2f;
        public const float NodeFloorSpacing = 1.2f;
    }

    // ── 시너지 토스트 (SynergyToastView) ──
    public static class SynergyToast
    {
        public const float Lifetime         = 1.5f;   // 초
        public const float RiseSpeed        = 0.3f;   // 월드 단위/초

        // t = elapsed/Lifetime ∈ [0, 1] 기준 정규화 타이밍
        public const float ScaleUpEnd       = 0.2f;   // t<0.2: 스케일 업
        public const float FadeOutStart     = 0.7f;   // t>0.7: 페이드 아웃 시작
        public const float FadeOutWindow    = 0.3f;   // 페이드 지속 (1 - FadeOutStart)

        // 스케일 값
        public const float ScaleStart       = 0.1f;
        public const float ScaleEnd         = 0.22f;

        // 텍스트
        public const int   FontSize         = 48;
        public const float CharacterSize    = 0.1f;
        public const float CameraYRatio     = 0.7f;   // 카메라 ortho 위쪽 70% 위치에 배치
    }

    // ── 적 스포너 기본값 ──
    public static class Spawner
    {
        public const float DefaultSpawnY = -3f;
        public const float DefaultSpawnXMin = 7f;
        public const float DefaultSpawnXMax = 10f;
        public const float DefaultDelayBetweenWaves = 5f;
    }

    // ── 우주선 물리 ──
    public static class ShipPhysics
    {
        public const float FixedDt = 1f / 120f;
        public const int MaxSubSteps = 8;
        public const float MinGravityDistance = 0.8f;
        public const float MaxGravityForce = 50f;
        public const float MaxSpeed = 30f;
        public const float ShipCollisionRadius = 0.1f;
        public const float GravityEnergyRatio = 0.35f;  // 행성 충돌 에너지 소모 비율 (전역)
        public const float DefaultEnergy = 100f;
        public const float DefaultEnergyDrain = 33.33f;
        public const float DefaultDrag = 0.05f;
        public const float LaunchPowerMultiplier = 5f;
        public const float RelaunchPower = 20f;              // 행성 착지 후 재발사 고정 속도
        public const float WorldBoundsX = 50f;   // 발사체 월드 경계 반폭 (카메라 무관)
        public const float WorldBoundsYMin = -15f; // 하단 경계
        public const float WorldBoundsYMax = 50f;  // 상단 경계

        // 슬링샷 (앵그리버드 스타일)
        public const float MaxPullDistance = 4f;
        public const float MinPullDistance = 0.3f;
        public const float PullGateRadius = 1.5f;            // 조준 시작 히트 반경 (슬링샷 잡기)
        public const int TrajectoryPreviewSteps = 300;       // 2.5초 @ FixedDt
        public const int TrajectoryPreviewDotCount = 40;
    }

    // ── 캐싱된 공유 리소스 ──
    private static Material _spriteMaterial;
    public static Material SpriteMaterial
    {
        get
        {
            if (_spriteMaterial == null)
            {
                // URP 2D Lit 셰이더 우선, 없으면 기본 스프라이트 셰이더
                var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default");
                if (shader == null)
                    shader = Shader.Find("Sprites/Default");
                _spriteMaterial = new Material(shader);
            }
            return _spriteMaterial;
        }
    }
}
