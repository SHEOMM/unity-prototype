/// <summary>
/// 맵 생성 (StS 스타일 분기형) 파라미터.
/// 방 타입 분포 / 노드 좌표 / 재시도 정책.
/// </summary>
public static partial class GameConstants
{
    /// <summary>
    /// 방 타입 누적 확률 분포 (CDF). 0~1 난수가 어느 구간에 속하는지로 결정.
    /// 분포: Combat 55% | Elite 10% | Rest 12% | Shop 8% | Event 15%.
    /// 변경 시 RollRoomType의 if-else 순서와 일치 유지 필요.
    /// </summary>
    public static class MapGeneration
    {
        // ── 방 타입 누적 확률 (CDF) ─────────────────────────────────

        /// <summary>0.00 ~ 0.55 → Combat (가장 흔한 방).</summary>
        public const float CombatRoomCdf = 0.55f;

        /// <summary>0.55 ~ 0.65 → Elite (강적, 좋은 보상).</summary>
        public const float EliteRoomCdf = 0.65f;

        /// <summary>0.65 ~ 0.77 → Rest (HP 회복 / 유물 강화).</summary>
        public const float RestRoomCdf = 0.77f;

        /// <summary>0.77 ~ 0.85 → Shop (상점).</summary>
        public const float ShopRoomCdf = 0.85f;

        // 0.85 ~ 1.00 → Event (이상 이벤트, 명시 상수 없음 — else 분기)

        // ── 재시도 ──────────────────────────────────────────────────

        /// <summary>방 타입 재롤 최대 시도 — 연속 규칙(예: 같은 층에 Elite 연속 금지) 위반 시 다시 굴림.</summary>
        public const int RoomTypeMaxAttempts = 10;

        // ── 노드 좌표 (월드 단위) ──────────────────────────────────

        /// <summary>같은 층에서 노드 사이 가로 간격.</summary>
        public const float NodeColSpacing = 2f;

        /// <summary>층 사이 세로 간격 (위로 갈수록 진행).</summary>
        public const float NodeFloorSpacing = 1.2f;
    }
}
