using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 우주선 발사 오케스트레이터. 상태 전이는 <see cref="ShipPhase"/>가 단일 진입점.
///
/// 흐름: Disabled → (Activate) → Ready → (OnLaunch) → InFlight
///       → ShipModel 이벤트로 PendingLanding/PendingFlightEnd → 다음 Update에서 처리
///       → Docked → (OnDockedClick) → InFlight … → (에너지 0 또는 OOB) → Ready.
///
/// 라운드 전체 조우 시퀀스를 누적하여 SpellResolver에 전달.
/// </summary>
public class ShipController : MonoBehaviour
{
    public static ShipController Instance { get; private set; }

    enum ShipPhase
    {
        Disabled,         // 전투 외 또는 Deactivate 후. 입력 미구독.
        Ready,            // 슬링샷 입력 대기.
        InFlight,         // 비행 중 (_activeShip != null).
        PendingLanding,   // ShipModel.OnLanding 발생 → 다음 Update에서 ProcessLanding.
        Docked,           // 행성 표면 도킹 중 (_dockedPlanet != null).
        PendingFlightEnd, // 에너지 0 또는 OOB → 다음 Update에서 ProcessFlightEnd.
    }

    private ShipPhase _phase = ShipPhase.Disabled;

    private ShipInput _input;
    private SpellResolver _resolver;
    private SpellEffectManager _spellFx;
    private ShipVisual _visual;

    private ShipModel _activeShip;
    private PlanetBody _dockedPlanet;
    private float _remainingEnergy;
    private readonly List<PlanetBody> _roundEncounters = new List<PlanetBody>();

    public System.Action<SpellResult> OnShipComplete;

    /// <summary>발사체가 천체에 접촉할 때마다 발행 (per-hit). SynergyDispatcher 등이 구독.</summary>
    public event System.Action<PlanetBody> OnPlanetHit;

    /// <summary>라운드 첫 발사(지면) 시 발행. 시너지 누적기 초기화용.</summary>
    public event System.Action OnFlightStarted;

    /// <summary>라운드 종료 시 전체 조우 시퀀스와 함께 발행.</summary>
    public event System.Action<IReadOnlyList<PlanetBody>> OnFlightEnded;

    /// <summary>외부에서 현재 비행 중인 ShipModel을 조회 (SynergyContext 등 구성용).</summary>
    public ShipModel ActiveShip => _activeShip;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void Initialize(ShipInput input, SpellResolver resolver,
                          SpellEffectManager spellFx, ShipVisual visual)
    {
        _input = input;
        _resolver = resolver;
        _spellFx = spellFx;
        _visual = visual;
        _visual.Initialize();
    }

    public void Activate()
    {
        if (_phase != ShipPhase.Disabled || _input == null) return;
        SubscribeInput();
        _phase = ShipPhase.Ready;
        _visual.ShowOriginIndicator(_input.LaunchOrigin, GameConstants.ShipPhysics.PullGateRadius);
    }

    public void Deactivate()
    {
        if (_phase == ShipPhase.Disabled || _input == null) return;
        UnsubscribeInput();
        if (_input.IsDockedMode)
        {
            _input.IsDockedMode = false;
            _input.OnDockedClick -= OnDockedClick;
        }
        _dockedPlanet?.Undock();
        _dockedPlanet = null;
        foreach (var p in _roundEncounters) { p.ReactivateGravity(); p.Highlight(false); }
        _roundEncounters.Clear();
        _activeShip = null;
        _phase = ShipPhase.Disabled;
        _visual.HideSlingshotBand();
        _visual.HideTrajectoryPreview();
        _visual.HideOriginIndicator();
    }

    void Update()
    {
        switch (_phase)
        {
            case ShipPhase.PendingLanding:    ProcessLanding();   break;
            case ShipPhase.PendingFlightEnd:  ProcessFlightEnd(); break;
            case ShipPhase.Docked:            UpdateDocked();     break;
            case ShipPhase.InFlight:          UpdateInFlight();   break;
            // Disabled / Ready: 아무것도 안 함 (입력 콜백이 전이 트리거)
        }
    }

    void UpdateInFlight()
    {
        if (_activeShip == null || _visual == null) return;
        _activeShip.Tick(Time.deltaTime);
        // Tick 도중 OnLanding/OnFlightEnded이 발행되면 _phase가 Pending*으로 바뀜.
        // _activeShip 자체는 ProcessLanding/End에서 null화되므로 여기선 마지막 프레임 렌더만.
        if (_activeShip == null) return;
        _visual.UpdateShipPosition(_activeShip.Position, _activeShip.Velocity);
        _visual.UpdateEnergyGauge(_activeShip.Position, _activeShip.Energy / GameConstants.ShipPhysics.DefaultEnergy);
    }

    void UpdateDocked()
    {
        _remainingEnergy = Mathf.Max(0f, _remainingEnergy - GameConstants.ShipPhysics.DefaultEnergyDrain * Time.deltaTime);

        Vector2 shipPos = _dockedPlanet.GetDockedShipPosition();
        Vector2 launchDir = _dockedPlanet.GetDockedLaunchDirection();
        _visual.UpdateShipPosition(shipPos, launchDir);
        _visual.UpdateEnergyGauge(shipPos, _remainingEnergy / GameConstants.ShipPhysics.DefaultEnergy);
        _visual.ShowTrajectoryPreview(shipPos, launchDir * GameConstants.ShipPhysics.RelaunchPower, _remainingEnergy);

        if (_remainingEnergy <= 0f) _phase = ShipPhase.PendingFlightEnd;
    }

    // ── 콜백 라이프사이클 ────────────────────────────────────────────

    void SubscribeInput()
    {
        _input.OnAimStart  += OnAimStart;
        _input.OnAimUpdate += OnAimUpdate;
        _input.OnLaunch    += OnLaunch;
        _input.OnAimCancel += OnAimCancel;
    }

    void UnsubscribeInput()
    {
        _input.OnAimStart  -= OnAimStart;
        _input.OnAimUpdate -= OnAimUpdate;
        _input.OnLaunch    -= OnLaunch;
        _input.OnAimCancel -= OnAimCancel;
    }

    /// <summary>
    /// ShipModel 인스턴스마다 1회 구독. ShipModel은 매 발사마다 새로 만들어지므로 -=가 불필요하나,
    /// 명시적 명명 메서드로 두어 라이프사이클을 코드로 보이게 함 (구 lambda 클로저 대비).
    /// </summary>
    void SubscribeShipEvents(ShipModel ship)
    {
        ship.OnFlightEnded       += OnShipFlightEnded;
        ship.OnLanding           += OnShipLanding;
        ship.OnPlanetEncountered += OnPlanetEncountered;
    }

    void OnShipFlightEnded() => _phase = ShipPhase.PendingFlightEnd;

    void OnShipLanding(PlanetBody planet)
    {
        _dockedPlanet = planet;
        _phase = ShipPhase.PendingLanding;
    }

    void OnPlanetEncountered(PlanetBody planet)
    {
        planet.Highlight(true);
        OnPlanetHit?.Invoke(planet);
    }

    // ── 입력 핸들러 (Ready/Docked에서만 의미) ───────────────────────

    bool IsReadyToLaunch => _phase == ShipPhase.Ready;

    void OnAimStart(Vector2 pos)
    {
        if (!IsReadyToLaunch) return;
        CameraService.Instance?.ZoomToAim(_input.LaunchOrigin);
    }

    void OnAimUpdate(Vector2 origin, Vector2 clampedPullPos, float pullRatio)
    {
        if (!IsReadyToLaunch) return;
        _visual.ShowSlingshotBand(origin, clampedPullPos, pullRatio);
        Vector2 pullVec = origin - clampedPullPos;
        Vector2 launchVelocity = pullVec * GameConstants.ShipPhysics.LaunchPowerMultiplier;
        _visual.ShowTrajectoryPreview(origin, launchVelocity);
    }

    void OnAimCancel()
    {
        _visual.HideSlingshotBand();
        _visual.HideTrajectoryPreview();
        CameraService.Instance?.ZoomToNormal();
    }

    void OnLaunch(Vector2 origin, Vector2 direction, float power)
    {
        if (!IsReadyToLaunch) return;

        _visual.HideSlingshotBand();
        _visual.HideTrajectoryPreview();
        _roundEncounters.Clear();

        CreateAndLaunchShip(origin, direction * power, GameConstants.ShipPhysics.DefaultEnergy);
        _visual.SpawnShip(origin);
        OnFlightStarted?.Invoke();
    }

    void OnDockedClick()
    {
        if (_phase != ShipPhase.Docked) return;

        Vector2 origin = _dockedPlanet.GetDockedShipPosition();
        Vector2 launchDir = _dockedPlanet.GetDockedLaunchDirection();

        _input.IsDockedMode = false;
        _input.OnDockedClick -= OnDockedClick;
        _visual.HideTrajectoryPreview();

        _dockedPlanet.Undock();
        _dockedPlanet = null;

        _visual.DestroyShip();
        _visual.SpawnShip(origin);

        CreateAndLaunchShip(origin, launchDir * GameConstants.ShipPhysics.RelaunchPower, _remainingEnergy);
    }

    /// <summary>
    /// ShipModel 생성 + 이벤트 구독 + Launch + 상태 전이를 한 메서드로 묶음.
    /// 최초 발사(OnLaunch)와 도킹 후 재발사(OnDockedClick) 양쪽이 호출.
    /// </summary>
    void CreateAndLaunchShip(Vector2 origin, Vector2 velocity, float energy)
    {
        _activeShip = new ShipModel();
        SubscribeShipEvents(_activeShip);
        _activeShip.Launch(origin, velocity, energy);
        _phase = ShipPhase.InFlight;
    }

    // ── 전이 처리 (Pending* → 다음 안정 상태) ─────────────────────────

    void ProcessLanding()
    {
        if (_activeShip == null || _dockedPlanet == null)
        {
            // 비정상 상태 — 안전하게 라운드 종료로 전이
            _phase = ShipPhase.PendingFlightEnd;
            return;
        }

        _remainingEnergy = Mathf.Min(
            _activeShip.Energy + GameConstants.ShipPhysics.DefaultEnergy * 0.3f,
            GameConstants.ShipPhysics.DefaultEnergy);
        _roundEncounters.AddRange(_activeShip.Encounters);

        Vector2 contactPos = _activeShip.Position;
        _activeShip = null;

        _dockedPlanet.Dock(contactPos);
        _visual.UpdateShipPosition(contactPos, Vector2.zero);
        _visual.SetTrailEnabled(false);

        _input.IsDockedMode = true;
        _input.OnDockedClick += OnDockedClick;

        _phase = ShipPhase.Docked;
    }

    void ProcessFlightEnd()
    {
        if (_activeShip != null)
            _roundEncounters.AddRange(_activeShip.Encounters);

        _visual.DestroyShip();
        CameraService.Instance?.ZoomToNormal();

        _input.LaunchOrigin = new Vector2(_input.launchOriginX, _input.celestialYMin);
        _visual.ShowOriginIndicator(_input.LaunchOrigin, GameConstants.ShipPhysics.PullGateRadius);

        _activeShip = null;

        var allEncounters = new List<PlanetBody>(_roundEncounters);
        _roundEncounters.Clear();

        foreach (var p in allEncounters)
        {
            p.ReactivateGravity();
            p.Highlight(false);
        }

        // 다음 라운드를 위해 Ready로 복귀
        _phase = ShipPhase.Ready;

        OnFlightEnded?.Invoke(allEncounters);

        if (allEncounters.Count == 0) return;

        var result = _resolver.Resolve(allEncounters);
        _spellFx.ExecuteSpells(result);
        OnShipComplete?.Invoke(result);
    }
}
