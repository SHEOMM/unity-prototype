using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 우주선 발사 오케스트레이터. 상태 머신: Ready → InFlight → Docked → InFlight → … → Ready.
/// Ready: 지면 슬링샷 대기 / InFlight: 자유 비행 / Docked: 행성 자전 중 클릭 재발사.
/// 라운드 전체 조우 시퀀스를 누적하여 SpellResolver에 전달.
/// </summary>
public class ShipController : MonoBehaviour
{
    public static ShipController Instance { get; private set; }

    private ShipInput _input;
    private SpellResolver _resolver;
    private SpellEffectManager _spellFx;
    private ShipVisual _visual;

    private ShipModel _activeShip;
    private bool _active;

    // Landing+relaunch state
    private readonly List<PlanetBody> _roundEncounters = new List<PlanetBody>();
    private PlanetBody _dockedPlanet;
    private float _remainingEnergy;
    private bool _pendingLanding;
    private bool _pendingFlightEnd;

    public System.Action<SpellResult> OnShipComplete;

    /// <summary>발사체가 천체에 접촉할 때마다 발행 (per-hit). SynergyDispatcher 등이 구독.</summary>
    public event System.Action<PlanetBody> OnPlanetHit;

    /// <summary>라운드 첫 발사(지면) 시 발행. 시너지 누적기 초기화용.</summary>
    public event System.Action OnFlightStarted;

    /// <summary>라운드 종료 시 전체 조우 시퀀스와 함께 발행.</summary>
    public event System.Action<IReadOnlyList<PlanetBody>> OnFlightEnded;

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
        if (_active || _input == null) return;
        _active = true;
        _input.OnAimStart += OnAimStart;
        _input.OnAimUpdate += OnAimUpdate;
        _input.OnLaunch += OnLaunch;
        _input.OnAimCancel += OnAimCancel;
        _visual.ShowOriginIndicator(_input.LaunchOrigin, GameConstants.ShipPhysics.PullGateRadius);
    }

    public void Deactivate()
    {
        if (!_active || _input == null) return;
        _active = false;
        _input.OnAimStart -= OnAimStart;
        _input.OnAimUpdate -= OnAimUpdate;
        _input.OnLaunch -= OnLaunch;
        _input.OnAimCancel -= OnAimCancel;
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
        _pendingLanding = false;
        _pendingFlightEnd = false;
        _visual.HideSlingshotBand();
        _visual.HideTrajectoryPreview();
        _visual.HideOriginIndicator();
    }

    void Update()
    {
        if (_pendingLanding)
        {
            _pendingLanding = false;
            ProcessLanding();
            return;
        }

        if (_pendingFlightEnd)
        {
            _pendingFlightEnd = false;
            ProcessFlightEnd();
            return;
        }

        // Docked: 행성 자전에 맞춰 선박 위치 갱신 + 실시간 궤적 미리보기
        if (_dockedPlanet != null)
        {
            _remainingEnergy = Mathf.Max(0f, _remainingEnergy - GameConstants.ShipPhysics.DefaultEnergyDrain * Time.deltaTime);

            Vector2 shipPos = _dockedPlanet.GetDockedShipPosition();
            Vector2 launchDir = _dockedPlanet.GetDockedLaunchDirection();
            _visual.UpdateShipPosition(shipPos, launchDir);
            _visual.UpdateEnergyGauge(shipPos, _remainingEnergy / GameConstants.ShipPhysics.DefaultEnergy);
            _visual.ShowTrajectoryPreview(shipPos, launchDir * GameConstants.ShipPhysics.RelaunchPower, _remainingEnergy);

            if (_remainingEnergy <= 0f)
                _pendingFlightEnd = true;

            return;
        }

        if (_activeShip == null || _visual == null) return;

        _activeShip.Tick(Time.deltaTime);

        if (_activeShip != null)
        {
            _visual.UpdateShipPosition(_activeShip.Position, _activeShip.Velocity);
            _visual.UpdateEnergyGauge(_activeShip.Position, _activeShip.Energy / GameConstants.ShipPhysics.DefaultEnergy);
        }
    }

    bool IsReadyToLaunch => _activeShip == null && _dockedPlanet == null;

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

        _activeShip = new ShipModel();
        _activeShip.OnFlightEnded += () => _pendingFlightEnd = true;
        _activeShip.OnLanding += planet => { _dockedPlanet = planet; _pendingLanding = true; };
        _activeShip.OnPlanetEncountered += OnPlanetEncountered;
        _activeShip.Launch(origin, direction * power, GameConstants.ShipPhysics.DefaultEnergy);

        _visual.SpawnShip(origin);
        OnFlightStarted?.Invoke();
    }

    void OnPlanetEncountered(PlanetBody planet)
    {
        planet.Highlight(true);
        OnPlanetHit?.Invoke(planet);
    }

    void ProcessLanding()
    {
        if (_activeShip == null || _dockedPlanet == null) return;

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
    }

    void OnDockedClick()
    {
        if (_dockedPlanet == null) return;

        Vector2 origin = _dockedPlanet.GetDockedShipPosition();
        Vector2 launchDir = _dockedPlanet.GetDockedLaunchDirection();

        _input.IsDockedMode = false;
        _input.OnDockedClick -= OnDockedClick;
        _visual.HideTrajectoryPreview();

        _dockedPlanet.Undock();
        _dockedPlanet = null;

        _visual.DestroyShip();
        _visual.SpawnShip(origin);

        _activeShip = new ShipModel();
        _activeShip.OnFlightEnded += () => _pendingFlightEnd = true;
        _activeShip.OnLanding += planet => { _dockedPlanet = planet; _pendingLanding = true; };
        _activeShip.OnPlanetEncountered += OnPlanetEncountered;
        _activeShip.Launch(origin, launchDir * GameConstants.ShipPhysics.RelaunchPower, _remainingEnergy);
    }

    /// <summary>외부에서 현재 비행 중인 ShipModel을 조회 (SynergyContext 등 구성용).</summary>
    public ShipModel ActiveShip => _activeShip;

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

        OnFlightEnded?.Invoke(allEncounters);

        if (allEncounters.Count == 0) return;

        var result = _resolver.Resolve(allEncounters);
        _spellFx.ExecuteSpells(result);
        OnShipComplete?.Invoke(result);
    }
}
