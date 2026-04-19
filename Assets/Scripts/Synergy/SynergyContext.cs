using System.Collections.Generic;

/// <summary>
/// 시너지 실행 시 전달되는 컨텍스트. 발사체 비행 1회의 스냅샷 + 누적기.
///
/// OnHit에서: HitIndex/CurrentPlanet은 현재 조우 상황, HitSequence는 지금까지의 순서.
/// OnFlightEnd에서: HitIndex = HitSequence.Count - 1, HitSequence는 완성된 전체 시퀀스.
/// </summary>
public class SynergyContext
{
    /// <summary>조우한 천체들 (순서대로). OnHit에서는 현재 hit 포함된 상태까지.</summary>
    public IReadOnlyList<PlanetBody> HitSequence;

    /// <summary>OnHit 시 현재 index (0-based). OnFlightEnd 시에는 Count - 1.</summary>
    public int HitIndex;

    /// <summary>OnHit 시 현재 접촉 행성. OnFlightEnd 시에는 null 허용.</summary>
    public PlanetBody CurrentPlanet;

    /// <summary>속성 계열별 누적 카운트.</summary>
    public FamilyAccumulator Families;

    /// <summary>현재 살아있는 적 목록 (스냅샷).</summary>
    public IReadOnlyList<Enemy> Enemies;

    /// <summary>플레이어 상태 참조.</summary>
    public PlayerState Player;

    /// <summary>발사체 모델 (OnHit에서 에너지 등 조작 가능).</summary>
    public ShipModel Projectile;

    /// <summary>시너지별 랜덤 결정 (재현성 필요 시 seed 주입 가능).</summary>
    public System.Random Rng;

    /// <summary>현재 발동 중인 rule. Dispatcher가 effect 호출 직전에 세팅.
    /// Effect 구현체는 여기서 damage/radius/duration 등 데이터 파라미터를 읽는다.</summary>
    public SynergyRuleSO CurrentRule;
}
