using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시너지 실행 중재자. ShipController의 이벤트를 구독해 SynergyContext를 유지하고
/// 등록된 SynergyRuleSO들을 발동 시점에 검사 + ISynergyEffect를 호출한다.
///
/// 라이프사이클:
///   ShipController.OnFlightStarted → Families.Reset, _sequence 비움
///   ShipController.OnPlanetHit(planet) → Families.Record + per-hit synergies 호출
///   ShipController.OnFlightEnded(seq) → end-of-flight rules 검사 + ISynergyEffect.OnFlightEnd
///
/// 책임:
///   - 이벤트 중계 (Dispatcher)
///   - 규칙 순회 (Matcher에 위임)
///   - 효과 인스턴스 생성 (Registry에 위임)
/// 실행 로직은 각 ISynergyEffect 구현체 내부에만 존재 (SRP).
/// </summary>
public class SynergyDispatcher : MonoBehaviour
{
    public static SynergyDispatcher Instance { get; private set; }

    /// <summary>Inspector에서 지정 또는 런타임에 SetRules로 주입. 현재 라운드에서 고려할 시너지 규칙.</summary>
    [SerializeField] private List<SynergyRuleSO> _rules = new List<SynergyRuleSO>();

    private readonly FamilyAccumulator _families = new FamilyAccumulator();
    private readonly List<PlanetBody> _sequence = new List<PlanetBody>();
    private ShipController _ship;

    public FamilyAccumulator Families => _families;
    public IReadOnlyList<PlanetBody> Sequence => _sequence;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>ShipController 이벤트 구독. CombatManager가 Initialize 시 호출.</summary>
    public void Bind(ShipController ship)
    {
        if (_ship == ship) return;
        Unbind();
        _ship = ship;
        if (_ship == null) return;
        _ship.OnFlightStarted += HandleFlightStarted;
        _ship.OnPlanetHit += HandlePlanetHit;
        _ship.OnFlightEnded += HandleFlightEnded;
    }

    public void Unbind()
    {
        if (_ship == null) return;
        _ship.OnFlightStarted -= HandleFlightStarted;
        _ship.OnPlanetHit -= HandlePlanetHit;
        _ship.OnFlightEnded -= HandleFlightEnded;
        _ship = null;
    }

    public void SetRules(IEnumerable<SynergyRuleSO> rules)
    {
        _rules.Clear();
        if (rules != null) _rules.AddRange(rules);
    }

    public void AddRule(SynergyRuleSO rule)
    {
        if (rule != null && !_rules.Contains(rule)) _rules.Add(rule);
    }

    // ── 이벤트 핸들러 ──────────────────────────────────────────

    void HandleFlightStarted()
    {
        _families.Reset();
        _sequence.Clear();
    }

    void HandlePlanetHit(PlanetBody planet)
    {
        if (planet?.Planet == null) return;
        _families.Record(planet.Planet.synergyFamily);
        _sequence.Add(planet);

        var ctx = BuildContext(planet, _sequence.Count - 1);

        // per-hit 훅 — 현재 규칙에 매칭되는 효과 중 OnHit에만 반응하는 것들 실행.
        // 단순화: FamilyAccumulation으로 지금 이 hit이 threshold 도달 시에도 per-hit 발화 허용.
        // Phase 0에서는 모든 규칙에 대해 OnHit도 호출. 구현자가 필요한 훅만 override.
        foreach (var rule in _rules)
        {
            if (rule == null || !SynergyRuleMatcher.Matches(rule, ctx)) continue;
            var effect = SynergyRegistry.Get(rule.synergyEffectId);
            if (effect == null) continue;
            effect.OnHit(ctx);
        }
    }

    void HandleFlightEnded(IReadOnlyList<PlanetBody> finalSequence)
    {
        // finalSequence는 Ship.Encounters — _sequence와 동일해야 하지만 ShipController가 authoritative
        var ctx = BuildContext(null, finalSequence.Count - 1);
        ctx.HitSequence = finalSequence;

        foreach (var rule in _rules)
        {
            if (rule == null || !SynergyRuleMatcher.Matches(rule, ctx)) continue;
            var effect = SynergyRegistry.Get(rule.synergyEffectId);
            if (effect == null) continue;
            effect.OnFlightEnd(ctx);
        }
    }

    SynergyContext BuildContext(PlanetBody current, int hitIndex)
    {
        return new SynergyContext
        {
            HitSequence = _sequence,
            HitIndex = hitIndex,
            CurrentPlanet = current,
            Families = _families,
            Enemies = EnemyRegistry.Instance != null ? EnemyRegistry.Instance.GetAll() : new List<Enemy>(),
            Player = PlayerState.Instance,
            Projectile = _ship != null ? _ship.ActiveShip : null,
            Rng = new System.Random()
        };
    }
}
