/// <summary>
/// 천체(행성·궤도) 시각·애니메이션 상수.
/// OrbitBody / PlanetBody가 참조.
/// </summary>
public static partial class GameConstants
{
    /// <summary>궤도 LineRenderer 렌더링 파라미터.</summary>
    public static class Orbit
    {
        /// <summary>궤도 원을 분할하는 세그먼트 수. 클수록 부드럽지만 GPU 부하 증가.</summary>
        public const int PathSegments = 48;

        /// <summary>LineRenderer 두께.</summary>
        public const float PathWidth = 0.02f;

        /// <summary>궤도선 알파 — 가독성을 해치지 않을 정도로 옅게.</summary>
        public const float PathAlpha = 0.15f;
    }

    /// <summary>행성 자체의 시각 애니메이션 (스케일 박동 + 충돌 콜라이더).</summary>
    public static class PlanetAnim
    {
        /// <summary>박동 주파수 (Hz). Sin 기반 스케일 진동.</summary>
        public const float PulseFrequency = 3f;

        /// <summary>박동 진폭 — 1 ± 0.1 사이로 스케일 변동.</summary>
        public const float PulseAmplitude = 0.1f;

        /// <summary>행성 클릭 충돌 영역 반지름 (CircleCollider2D).</summary>
        public const float ColliderRadius = 0.34f;
    }
}
