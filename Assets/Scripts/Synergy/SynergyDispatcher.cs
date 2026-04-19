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

        // Phase 4: 모든 trigger 타입은 end-of-flight에서 일괄 판정한다.
        // SequencePosition/PlanetCombo를 per-hit에 매칭하면 매칭이 성립된 hit 이후 모든 hit에서 중복 발동하기 때문.
        // OnHit 훅은 ISynergyEffect 인터페이스에 남겨 두되 현재 아무도 호출하지 않는다.
        // 미래 per-hit 전용 TriggerType을 추가할 때 여기서 분기하면 된다.
    }

    void HandleFlightEnded(IReadOnlyList<PlanetBody> finalSequence)
    {
        // finalSequence는 Ship.Encounters — _sequence와 동일해야 하지만 ShipController가 authoritative
        var ctx = BuildContext(null, finalSequence.Count - 1);
        ctx.HitSequence = finalSequence;

        // 1) 비-FamilyAccumulation: 기존 그대로 모두 매칭 시 발동
        foreach (var rule in _rules)
        {
            if (rule == null) continue;
            if (rule.triggerType == SynergyTriggerType.FamilyAccumulation) continue;
            if (!SynergyRuleMatcher.Matches(rule, ctx)) continue;
            var effect = SynergyRegistry.Get(rule.synergyEffectId);
            if (effect == null) continue;
            ctx.CurrentRule = rule;
            effect.OnFlightEnd(ctx);
        }

        // 2) FamilyAccumulation: family별로 임계 충족한 rule 중 최고 threshold 1개만 발동
        FireHighestPerFamily(ctx);
    }

    /// <summary>
    /// 같은 family에 여러 tier(threshold=1/2/3)의 rule이 있을 때 임계 넘은 것 중
    /// threshold가 가장 큰 1개만 발동한다 (highest-only 시맨틱).
    /// 같은 threshold 복수 존재 시 _rules 등록 순서 상 먼저 만난 것을 사용.
    /// </summary>
    void FireHighestPerFamily(SynergyContext ctx)
    {
        var best = new Dictionary<SynergyFamily, SynergyRuleSO>();
        foreach (var rule in _rules)
        {
            if (rule == null) continue;
            if (rule.triggerType != SynergyTriggerType.FamilyAccumulation) continue;
            if (!SynergyRuleMatcher.Matches(rule, ctx)) continue;

            if (!best.TryGetValue(rule.family, out var cur) || rule.threshold > cur.threshold)
                best[rule.family] = rule;
        }

        foreach (var kv in best)
        {
            var rule = kv.Value;
            var effect = SynergyRegistry.Get(rule.synergyEffectId);
            if (effect == null) continue;
            ctx.CurrentRule = rule;
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
