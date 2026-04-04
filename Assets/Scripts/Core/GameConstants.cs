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
        public const int SynergyPopup = 25;
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
        public const float ColliderRadius = 0.5f;
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
    public static class Colors
    {
        public static readonly Color PlanetHighlight = new Color(1f, 1f, 0.5f, 1f);
        public static readonly Color HPFull = new Color(0.2f, 0.85f, 0.2f, 0.9f);
        public static readonly Color HPEmpty = new Color(0.85f, 0.2f, 0.2f, 0.9f);
        public static readonly Color HPBarBg = new Color(0.2f, 0.2f, 0.2f, 0.7f);
        public static readonly Color DividerLine = new Color(1f, 1f, 1f, 0.2f);
        public static readonly Color OrbitPath = new Color(1f, 1f, 1f, 0.15f);
        public static readonly Color SkyLabel = new Color(0.7f, 0.8f, 1f, 0.3f);
        public static readonly Color GroundLabel = new Color(1f, 0.7f, 0.7f, 0.3f);
        public static readonly Color StarLabel = new Color(1f, 1f, 0.8f, 0.8f);
        public static readonly Color PlanetLabel = new Color(1f, 1f, 1f, 0.9f);
        public static readonly Color EnemyLabel = new Color(1f, 0.8f, 0.8f, 0.85f);
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
        public const float DefaultEnergy = 100f;
        public const float DefaultEnergyDrain = 15f;
        public const float DefaultDrag = 0.05f;
        public const float LaunchPowerMultiplier = 5f;
        public const float BoundsMargin = 0.5f;
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
