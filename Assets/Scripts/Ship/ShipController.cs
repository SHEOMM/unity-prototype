using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 우주선 발사 오케스트레이터. 입력→시뮬레이션→SpellResolver→효과 발동.
/// 조준 중에는 슬링샷 밴드와 궤적 프리뷰를 렌더.
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
    private bool _pendingFlightEnd; // 비행 종료를 Update 끝에서 안전하게 처리

    public System.Action<SpellResult> OnShipComplete;

    /// <summary>발사체가 천체에 접촉할 때마다 발행 (per-hit). SynergyDispatcher 등이 구독.</summary>
    public event System.Action<PlanetBody> OnPlanetHit;

    /// <summary>비행 시작 시 발행. 시너지 누적기 초기화용.</summary>
    public event System.Action OnFlightStarted;

    /// <summary>비행 종료 시 최종 조우 시퀀스와 함께 발행.</summary>
    public event System.Action<System.Collections.Generic.IReadOnlyList<PlanetBody>> OnFlightEnded;

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
        _visual.HideSlingshotBand();
        _visual.HideTrajectoryPreview();
        _visual.HideOriginIndicator();
    }

    void Update()
    {
        if (_pendingFlightEnd)
        {
            _pendingFlightEnd = false;
            ProcessFlightEnd();
            return;
        }

        if (_visual == null || _activeShip == null || !_activeShip.IsAlive) return;

        _activeShip.Tick(Time.deltaTime);

        if (_activeShip != null && !_activeShip.IsAlive)
        {
            _pendingFlightEnd = true;
            return;
        }

        if (_activeShip != null)
            _visual.UpdateShipPosition(_activeShip.Position, _activeShip.Velocity);
    }

    bool IsReadyToLaunch => _activeShip == null;

    void OnAimStart(Vector2 pos)
    {
        if (!IsReadyToLaunch)
        {
            Debug.Log($"[Ship] 조준 시작 차단: 비행 중 (IsAlive={_activeShip?.IsAlive})");
            return;
        }
        // 지속 표시 중인 OriginIndicator가 이미 있으므로 별도 처리 불필요
    }

    void OnAimUpdate(Vector2 origin, Vector2 clampedPullPos, float pullRatio)
    {
        if (!IsReadyToLaunch) return;

        _visual.ShowSlingshotBand(origin, clampedPullPos, pullRatio);

        // 발사 방향/속도 = 당김의 반대 × LaunchPowerMultiplier
        Vector2 pullVec = origin - clampedPullPos;
        Vector2 launchVelocity = pullVec * GameConstants.ShipPhysics.LaunchPowerMultiplier;
        _visual.ShowTrajectoryPreview(origin, launchVelocity);
    }

    void OnAimCancel()
    {
        _visual.HideSlingshotBand();
        _visual.HideTrajectoryPreview();
        // OriginIndicator는 다음 조준을 위해 그대로 유지
    }

    void OnLaunch(Vector2 origin, Vector2 direction, float power)
    {
        if (!IsReadyToLaunch)
        {
            Debug.Log($"[Ship] 발사 차단: 비행 중 (IsAlive={_activeShip?.IsAlive})");
            return;
        }

        _visual.HideSlingshotBand();
        _visual.HideTrajectoryPreview();
        // SpawnShip이 내부에서 HideOriginIndicator 호출

        Debug.Log($"[Ship] 발사! origin={origin}, dir={direction}, power={power:F1}");

        _activeShip = new ShipModel();
        _activeShip.OnFlightEnded += () => _pendingFlightEnd = true;
        _activeShip.OnPlanetEncountered += OnPlanetEncountered;
        _activeShip.Launch(origin, direction * power, GameConstants.ShipPhysics.DefaultEnergy);

        _visual.SpawnShip(origin);
        OnFlightStarted?.Invoke();
    }

    void OnPlanetEncountered(PlanetBody planet)
    {
        planet.Highlight(true);
        Debug.Log($"[Ship] 행성 조우: {planet.Planet.bodyName}");
        OnPlanetHit?.Invoke(planet);
    }

    /// <summary>외부에서 현재 비행 중인 ShipModel을 조회 (SynergyContext 등 구성용).</summary>
    public ShipModel ActiveShip => _activeShip;

    void ProcessFlightEnd()
    {
        if (_activeShip == null) return;

        Debug.Log($"[Ship] 비행 종료. 조우 행성 {_activeShip.Encounters.Count}개");

        _visual.DestroyShip();
        _visual.ShowOriginIndicator(_input.LaunchOrigin, GameConstants.ShipPhysics.PullGateRadius);

        var encounters = new List<PlanetBody>(_activeShip.Encounters);
        _activeShip = null;

        foreach (var p in encounters)
            p.Highlight(false);

        // 시너지 구독자에게 최종 시퀀스 전달 (빈 시퀀스도 포함)
        OnFlightEnded?.Invoke(encounters);

        if (encounters.Count == 0) return;

        var result = _resolver.Resolve(encounters);
        _spellFx.ExecuteSpells(result);
        OnShipComplete?.Invoke(result);
    }
}
