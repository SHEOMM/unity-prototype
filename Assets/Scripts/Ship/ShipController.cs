using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 우주선 발사 오케스트레이터. 입력→시뮬레이션→SlashResolver→효과 발동.
/// SlashController와 동일한 구조적 역할.
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
    }

    public void Deactivate()
    {
        if (!_active || _input == null) return;
        _active = false;
        _input.OnAimStart -= OnAimStart;
        _input.OnAimUpdate -= OnAimUpdate;
        _input.OnLaunch -= OnLaunch;
        _visual.HideAimLine();
    }

    void Update()
    {
        if (_activeShip == null || !_activeShip.IsAlive) return;

        _activeShip.Tick(Time.deltaTime);
        _visual.UpdateShipPosition(_activeShip.Position, _activeShip.Velocity);
    }

    void OnAimStart(Vector2 pos) { }

    void OnAimUpdate(Vector2 start, Vector2 current)
    {
        _visual.ShowAimLine(start, current);
    }

    void OnLaunch(Vector2 origin, Vector2 direction, float power)
    {
        if (_activeShip != null && _activeShip.IsAlive) return;

        _visual.HideAimLine();

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

        // 하이라이트 해제
        foreach (var p in encounters)
            p.Highlight(false);

        if (encounters.Count == 0) return;

        // SlashResolver 재사용 — 우주선이 수집한 행성 리스트로 효과 발동
        var result = _resolver.Resolve(encounters);
        _spellFx.ExecuteSpells(result);
        OnShipComplete?.Invoke(result);
    }
}
