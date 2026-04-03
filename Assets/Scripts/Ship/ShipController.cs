using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 우주선 발사 오케스트레이터. 입력→시뮬레이션→SlashResolver→효과 발동.
/// </summary>
public class ShipController : MonoBehaviour
{
    public static ShipController Instance { get; private set; }

    private ShipInput _input;
    private SlashResolver _resolver;
    private SpellEffectManager _spellFx;
    private ShipVisual _visual;

    private ShipModel _activeShip;
    private bool _active;
    private bool _pendingFlightEnd; // 비행 종료를 Update 끝에서 안전하게 처리

    public System.Action<SlashResult> OnShipComplete;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void Initialize(ShipInput input, SlashResolver resolver,
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
    }

    public void Deactivate()
    {
        if (!_active || _input == null) return;
        _active = false;
        _input.OnAimStart -= OnAimStart;
        _input.OnAimUpdate -= OnAimUpdate;
        _input.OnLaunch -= OnLaunch;
        _input.OnAimCancel -= OnAimCancel;
        _visual.HideAimLine();
        _visual.HideLaunchMarker();
    }

    void Update()
    {
        // 비행 종료 지연 처리 — Tick 중이 아닌 Update 끝에서 안전하게 정리
        if (_pendingFlightEnd)
        {
            _pendingFlightEnd = false;
            ProcessFlightEnd();
            return;
        }

        if (_visual == null || _activeShip == null || !_activeShip.IsAlive) return;

        _activeShip.Tick(Time.deltaTime);

        // Tick 중 비행이 끝났으면 다음 프레임에서 처리
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
        _visual.ShowLaunchMarker(pos);
    }

    void OnAimUpdate(Vector2 start, Vector2 current)
    {
        if (!IsReadyToLaunch) return;
        _visual.ShowAimLine(start, current);
    }

    void OnAimCancel()
    {
        _visual.HideAimLine();
        _visual.HideLaunchMarker();
    }

    void OnLaunch(Vector2 origin, Vector2 direction, float power)
    {
        if (!IsReadyToLaunch)
        {
            Debug.Log($"[Ship] 발사 차단: 비행 중 (IsAlive={_activeShip?.IsAlive})");
            return;
        }

        _visual.HideAimLine();
        _visual.HideLaunchMarker();

        Debug.Log($"[Ship] 발사! origin={origin}, dir={direction}, power={power:F1}");

        _activeShip = new ShipModel();
        _activeShip.OnFlightEnded += () => _pendingFlightEnd = true;
        _activeShip.OnPlanetEncountered += OnPlanetEncountered;
        _activeShip.Launch(origin, direction * power, GameConstants.ShipPhysics.DefaultEnergy);

        _visual.SpawnShip(origin);
    }

    void OnPlanetEncountered(PlanetBody planet)
    {
        planet.Highlight(true);
        Debug.Log($"[Ship] 행성 조우: {planet.Planet.bodyName}");
    }

    void ProcessFlightEnd()
    {
        if (_activeShip == null) return;

        Debug.Log($"[Ship] 비행 종료. 조우 행성 {_activeShip.Encounters.Count}개");

        _visual.DestroyShip();

        var encounters = new List<PlanetBody>(_activeShip.Encounters);
        _activeShip = null;

        foreach (var p in encounters)
            p.Highlight(false);

        if (encounters.Count == 0) return;

        var result = _resolver.Resolve(encounters);
        _spellFx.ExecuteSpells(result);
        OnShipComplete?.Invoke(result);
    }
}
