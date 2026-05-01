using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 우주선 물리 모델. 각 관심사를 전문 클래스에 위임한다.
/// SimulateStep은 4줄: 중력 합산 → 적분 → 에너지 소모 → 충돌 감지.
/// </summary>
public class ShipModel
{
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; private set; }
    public float Energy { get; private set; }
    public bool IsAlive => Energy > 0f;
    public IReadOnlyList<PlanetBody> Encounters => _encounters.EncounterOrder;

    private float _drag;
    private float _energyDrain;

    private readonly FixedTimestepSimulator _timestep = new FixedTimestepSimulator();
    private readonly GravityAccumulator _gravity = new GravityAccumulator();
    private readonly EncounterDetector _encounters = new EncounterDetector();

    private IReadOnlyList<IGravitySource> _sources;
    private int _sourceCount;

    public event System.Action<PlanetBody> OnPlanetEncountered;
    public event System.Action<PlanetBody> OnLanding;
    public event System.Action OnFlightEnded;

    private bool _landed;

    public void Launch(Vector2 origin, Vector2 velocity, float energy,
                       float drag = GameConstants.ShipPhysics.DefaultDrag,
                       float energyDrain = GameConstants.ShipPhysics.DefaultEnergyDrain)
    {
        Position = origin;
        Velocity = velocity;
        Energy = energy;
        _drag = drag;
        _energyDrain = energyDrain;
        _landed = false;
        _timestep.Reset();
        _encounters.Reset();

        _sources = GravitySourceRegistry.Instance.Sources;
        _sourceCount = GravitySourceRegistry.Instance.Count;
    }

    public void Tick(float deltaTime)
    {
        if (_landed || !IsAlive) return;

        _timestep.Accumulate(deltaTime);
        while (_timestep.ConsumeStep())
        {
            SimulateStep(GameConstants.ShipPhysics.FixedDt);
            if (_landed || Energy <= 0f || IsOutOfBounds()) break;
        }

        // 착지 없이 에너지/바운드 초과 시만 라운드 종료 신호
        if (!_landed && (Energy <= 0f || IsOutOfBounds()))
        {
            Energy = 0f;
            OnFlightEnded?.Invoke();
        }
    }

    private void SimulateStep(float dt)
    {
        Vector2 oldPos = Position;

        Vector2 totalForce = _gravity.Calculate(Position, _sources, _sourceCount);

        Vector2 pos = Position;
        Vector2 vel = Velocity;
        ShipIntegrator.Integrate(ref pos, ref vel, totalForce, _drag, dt);
        Position = pos;
        Velocity = vel;

        Energy -= _energyDrain * dt;

        _encounters.DetectEncounters(oldPos, Position, _sources, _sourceCount,
            HandlePlanetEncounter);
    }

    private void HandlePlanetEncounter(PlanetBody planet)
    {
        float cost = planet.Planet.gravityStrength * planet.Planet.gravityEnergyRatio;
        Energy = Mathf.Max(0f, Energy - cost);
        planet.DeactivateGravity();
        OnPlanetEncountered?.Invoke(planet);
        _landed = true;
        OnLanding?.Invoke(planet);
    }

    private bool IsOutOfBounds()
    {
        // 카메라 ortho size 6 기준: 높이 ±6, 너비 ±8 (aspect ~1.3)
        float m = GameConstants.ShipPhysics.BoundsMargin;
        return Position.x < -9f - m || Position.x > 9f + m
            || Position.y < -7f - m || Position.y > 7f + m;
    }
}
