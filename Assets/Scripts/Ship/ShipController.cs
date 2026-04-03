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
        if (_visual == null || _activeShip == null || !_activeShip.IsAlive) return;

        _activeShip.Tick(Time.deltaTime);
        if (_activeShip != null && _activeShip.IsAlive)
            _visual.UpdateShipPosition(_activeShip.Position, _activeShip.Velocity);
    }

    void OnAimStart(Vector2 pos)
    {
        _visual.ShowLaunchMarker(pos);
    }

    void OnAimUpdate(Vector2 start, Vector2 current)
    {
        _visual.ShowAimLine(start, current);
    }

    void OnAimCancel()
    {
        _visual.HideAimLine();
        _visual.HideLaunchMarker();
    }

    void OnLaunch(Vector2 origin, Vector2 direction, float power)
    {
        // 비행 중이면 발사 불가. 비행 종료된 잔여 모델은 정리.
        if (_activeShip != null && _activeShip.IsAlive) return;
        if (_activeShip != null && !_activeShip.IsAlive)
        {
            _activeShip = null;
            _visual.DestroyShip();
        }

        _visual.HideAimLine();
        _visual.HideLaunchMarker();

        _activeShip = new ShipModel();
        _activeShip.OnPlanetEncountered += OnPlanetEncountered;
        _activeShip.OnFlightEnded += OnFlightEnded;
        _activeShip.Launch(origin, direction * power, GameConstants.ShipPhysics.DefaultEnergy);

        _visual.SpawnShip(origin);
    }

    void OnPlanetEncountered(PlanetBody planet)
    {
        planet.Highlight(true);
    }

    void OnFlightEnded()
    {
        if (_activeShip == null) return;

        _visual.DestroyShip();

        var encounters = new List<PlanetBody>(_activeShip.Encounters);
        _activeShip.OnPlanetEncountered -= OnPlanetEncountered;
        _activeShip.OnFlightEnded -= OnFlightEnded;
        _activeShip = null;

        foreach (var p in encounters)
            p.Highlight(false);

        if (encounters.Count == 0) return;

        var result = _resolver.Resolve(encounters);
        _spellFx.ExecuteSpells(result);
        OnShipComplete?.Invoke(result);
    }
}
