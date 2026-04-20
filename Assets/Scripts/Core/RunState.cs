using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 현재 런의 진행 상태 컨테이너. 층·덱·시드·보유 궤도·배치를 저장한다.
/// Phase 9 리팩터링(PR2): Cosmos 배치 로직은 CosmosService / OrbitAssignmentQuery로 이전.
/// 이 파일은 직렬화 대상 필드 + 최소 헬퍼만 남는다.
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

    public void AdvanceFloor() { currentFloor++; }
}
