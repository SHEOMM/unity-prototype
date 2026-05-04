/// <summary>
/// UI 레이어 상수 — 라벨 / HP바 / 데미지 팝업 / 시너지 토스트.
/// World Space TextMesh + SpriteRenderer 기반 (Canvas 미사용).
/// </summary>
public static partial class GameConstants
{
    /// <summary>천체(행성/항성) 라벨 — PlanetLabelView가 참조.</summary>
    public static class CelestialLabel
    {
        public const int FontSize = 64;
        public const float CharacterSize = 0.12f;

        /// <summary>행성 라벨 Y 오프셋 — 행성 중심 아래로 표시.</summary>
        public const float PlanetYOffset = -1.2f;

        /// <summary>항성 라벨 Y 오프셋 (Phase 9 이전 잔존).</summary>
        public const float StarYOffset = -1.5f;

        /// <summary>거리에 따른 라벨 스케일 보정 곱 (멀어질수록 작아짐).</summary>
        public const float ScaleMultiplier = 0.5f;

        /// <summary>이 임계값보다 작은 스케일은 라벨을 숨김 (가독성 한계).</summary>
        public const float MinScaleThreshold = 0.01f;
    }

    /// <summary>
    /// EnemyView 전용 UI — 적 머리 위 라벨 + HP 바.
    /// 같은 레이아웃을 AllyView/StructureView가 미러링.
    /// </summary>
    public static class EnemyUI
    {
        public const float LabelYOffset = -0.8f;
        public const float LabelScale = 0.4f;
        public const int LabelFontSize = 48;

        public const float HPBarYOffset = 0.7f;
        public const float HPBarWidth = 0.7f;
        public const float HPBarHeight = 0.06f;

        /// <summary>HP 텍스트("123/200") 스케일.</summary>
        public const float HPTextScale = 0.3f;
    }

    /// <summary>피해 숫자 팝업 — DamagePopup.Spawn이 참조하는 스폰/이동 파라미터.</summary>
    public static class Popup
    {
        /// <summary>팝업 총 수명(초). 이 시간 후 Destroy.</summary>
        public const float Lifetime = 0.8f;

        /// <summary>스폰 위치 Y 오프셋 (피격 위치보다 위로 살짝 띄움).</summary>
        public const float SpawnYOffset = 0.5f;

        /// <summary>스폰 X에 더해질 무작위 흔들림 ±값. 동시에 여러 팝업이 겹치지 않게.</summary>
        public const float SpawnXJitter = 0.3f;

        /// <summary>팝업 시작 스케일.</summary>
        public const float InitialScale = 0.12f;

        /// <summary>초기 상승 속도 (월드 단위/초).</summary>
        public const float VelocityY = 2f;

        /// <summary>중력 가속도 — VelocityY가 점차 줄어 떨어짐.</summary>
        public const float Gravity = 3f;

        /// <summary>스케일 자연 증가율 — 시간에 따라 조금씩 커짐 (강조).</summary>
        public const float ScaleGrowth = 0.3f;
    }

    /// <summary>
    /// 시너지 발동 토스트 (SynergyToastView).
    /// t = elapsed/Lifetime ∈ [0, 1] 기준 정규화 타이밍.
    /// </summary>
    public static class SynergyToast
    {
        public const float Lifetime  = 1.5f;   // 초
        public const float RiseSpeed = 0.3f;   // 월드 단위/초 (위로 떠오름)

        // ── 정규화 타이밍 ──

        /// <summary>t &lt; 0.2: 스케일 업 구간 (0 → 0.22).</summary>
        public const float ScaleUpEnd = 0.2f;

        /// <summary>t &gt; 0.7: 페이드 아웃 시작.</summary>
        public const float FadeOutStart = 0.7f;

        /// <summary>페이드 지속 비율 (= 1 - FadeOutStart).</summary>
        public const float FadeOutWindow = 0.3f;

        // ── 스케일 ──

        /// <summary>스폰 시 시작 스케일 (작게 등장).</summary>
        public const float ScaleStart = 0.1f;

        /// <summary>스케일 업 완료 후 유지값.</summary>
        public const float ScaleEnd = 0.22f;

        // ── 텍스트 ──

        public const int FontSize = 48;
        public const float CharacterSize = 0.1f;

        /// <summary>화면 상단 배치 — 카메라 ortho 위쪽 70% 위치.</summary>
        public const float CameraYRatio = 0.7f;
    }
}
