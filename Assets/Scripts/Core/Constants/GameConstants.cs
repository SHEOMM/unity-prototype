using UnityEngine;

/// <summary>
/// 게임 전체에서 공유되는 상수 정의의 진입점.
///
/// <para>이 클래스는 <c>partial</c>로 선언되어 역할별 파일로 분할 관리된다.
/// 새 카테고리 추가 시: <c>Assets/Scripts/Core/Constants/</c> 폴더에 새 파일을
/// 만들어 <c>public static partial class GameConstants</c> 안에 nested static class를
/// 추가한다. 호출 측은 항상 <c>GameConstants.X.Value</c> 형태로 동일.</para>
///
/// <para>현재 분할된 영역:
/// <list type="bullet">
/// <item><c>Constants/Layout.cs</c> — 화면 경계, SortingOrder (렌더 z 레이어)</item>
/// <item><c>Constants/Colors.cs</c> — 모든 명명 색상 (헥스 표기)</item>
/// <item><c>Constants/ShipPhysics.cs</c> — 발사체 물리 / 슬링샷 / 월드 경계</item>
/// <item><c>Constants/Combat.cs</c> — 데미지 플래시 / 사망 타이머 / 적 스포너</item>
/// <item><c>Constants/UI.cs</c> — 라벨 / HP바 / 데미지 팝업 / 시너지 토스트</item>
/// <item><c>Constants/Celestial.cs</c> — 궤도 LineRenderer / 행성 박동</item>
/// <item><c>Constants/Map.cs</c> — 맵 방 타입 분포 / 노드 스페이싱</item>
/// <item><c>Constants/VFX.cs</c> — 마법 이펙트 / Perlin·Sin 노이즈 / 시너지 비주얼</item>
/// </list></para>
/// </summary>
public static partial class GameConstants
{
    // ── 화면 레이아웃 (전역) ────────────────────────────────────────

    /// <summary>월드 좌표계에서 게임 영역 좌측 끝(스폰 가드 등에 사용). 카메라 좌측보다 약간 안쪽.</summary>
    public const float ScreenBoundaryLeft = -12f;

    /// <summary>지/천 구분선(<c>CombatDividerView</c>)의 좌우 길이 절반(=총 길이 30).</summary>
    public const float DividerLineExtent = 15f;

    // ── 캐싱된 공유 리소스 ─────────────────────────────────────────

    private static Material _spriteMaterial;

    /// <summary>
    /// 모든 절차 생성 SpriteRenderer/LineRenderer가 공유하는 머티리얼.
    /// URP 2D Lit 셰이더가 있으면 그것을 사용, 없으면 기본 Sprites/Default로 폴백.
    /// 최초 접근 시 한 번만 생성되어 캐싱됨 (씬 전환 시에도 재사용).
    /// </summary>
    public static Material SpriteMaterial
    {
        get
        {
            if (_spriteMaterial == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                _spriteMaterial = new Material(shader);
            }
            return _spriteMaterial;
        }
    }
}
