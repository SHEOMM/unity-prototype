using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 현재 런의 진행 상태. 층, 덱, 시드, 보유 궤도·배치를 추적.
/// Phase 9: Star 제거 후 unlockedOrbits/orbitAssignments로 천체-궤도 관계 관리.
/// </summary>
public class RunState : MonoBehaviour
{
    public static RunState Instance { get; private set; }

    public int currentFloor;
    public int currentAct;
    public int runSeed;

    public List<PlanetSO> planetDeck = new List<PlanetSO>();
    public List<SatelliteSO> satellites = new List<SatelliteSO>();

    [Header("Phase 9 — Orbit 시스템")]
    [Tooltip("보유 중인 궤도 (보상으로 획득).")]
    public List<OrbitSO> unlockedOrbits = new List<OrbitSO>();

    [Tooltip("어떤 행성이 어느 궤도에 배치됐는지. 1 궤도 = 1 행성.")]
    public List<OrbitAssignment> orbitAssignments = new List<OrbitAssignment>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void InitializeRun(int seed, PlanetSO[] startingPlanets, OrbitSO[] startingOrbits, OrbitAssignment[] defaultAssignments)
    {
        runSeed = seed;
        currentFloor = 0;
        currentAct = 1;
        planetDeck = new List<PlanetSO>(startingPlanets ?? Array.Empty<PlanetSO>());
        satellites.Clear();
        unlockedOrbits = new List<OrbitSO>(startingOrbits ?? Array.Empty<OrbitSO>());
        orbitAssignments = new List<OrbitAssignment>(defaultAssignments ?? Array.Empty<OrbitAssignment>());
    }

    public void AddToDeck(CelestialBodySO item)
    {
        if (item is PlanetSO planet) planetDeck.Add(planet);
        else if (item is SatelliteSO sat) satellites.Add(sat);
    }

    public void UnlockOrbit(OrbitSO orbit)
    {
        if (orbit != null && !unlockedOrbits.Contains(orbit)) unlockedOrbits.Add(orbit);
    }

    /// <summary>planetName이 배치된 궤도 이름을 찾는다. 없으면 null.</summary>
    public OrbitSO FindOrbitForPlanet(string planetName)
    {
        foreach (var a in orbitAssignments)
            if (a.planetName == planetName)
                foreach (var o in unlockedOrbits)
                    if (o.orbitName == a.orbitName) return o;
        return null;
    }

    /// <summary>orbitName에 배치된 행성 이름을 찾는다. 없으면 null.</summary>
    public string FindPlanetForOrbit(string orbitName)
    {
        foreach (var a in orbitAssignments)
            if (a.orbitName == orbitName) return a.planetName;
        return null;
    }

    public void AdvanceFloor() { currentFloor++; }
}

[Serializable]
public struct OrbitAssignment
{
    public string orbitName;
    public string planetName;
}
