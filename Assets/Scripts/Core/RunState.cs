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

    /// <summary>
    /// 새 궤도 획득 시 — 미배치 행성 중 하나를 이 궤도에 자동 배치.
    /// Cosmos 씬이 없는 동안 궤도 보상을 즉시 활용 가능하게 하는 폴백.
    /// </summary>
    public bool TryAutoAssignPlanetToOrbit(OrbitSO orbit)
    {
        if (orbit == null || string.IsNullOrEmpty(orbit.orbitName)) return false;
        if (FindPlanetForOrbit(orbit.orbitName) != null) return false;

        foreach (var planet in planetDeck)
        {
            if (planet == null) continue;
            if (FindOrbitForPlanet(planet.bodyName) != null) continue;
            orbitAssignments.Add(new OrbitAssignment { orbitName = orbit.orbitName, planetName = planet.bodyName });
            return true;
        }
        return false;
    }

    /// <summary>새 행성 획득 시 — 빈 궤도 중 하나에 이 행성을 자동 배치.</summary>
    public bool TryAutoAssignOrbitToPlanet(PlanetSO planet)
    {
        if (planet == null || string.IsNullOrEmpty(planet.bodyName)) return false;
        if (FindOrbitForPlanet(planet.bodyName) != null) return false;

        foreach (var orbit in unlockedOrbits)
        {
            if (orbit == null) continue;
            if (FindPlanetForOrbit(orbit.orbitName) != null) continue;
            orbitAssignments.Add(new OrbitAssignment { orbitName = orbit.orbitName, planetName = planet.bodyName });
            return true;
        }
        return false;
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

    // ── Phase 9c: Cosmos 패널 수동 편집 헬퍼 ────────────────────────

    /// <summary>
    /// 궤도에 행성을 배치한다. 기존에 이 행성이 다른 궤도에 있었으면 그곳에서 제거.
    /// 기존에 이 궤도에 다른 행성이 있었으면 그 행성도 제거(호출자가 처리 — 교환은 SwapOrbitAssignments).
    /// 원자적 상태 변경: 모든 Cosmos 드롭 케이스의 기반이 되는 헬퍼.
    /// </summary>
    public void AssignPlanetToOrbit(OrbitSO orbit, PlanetSO planet)
    {
        if (orbit == null || planet == null) return;
        if (string.IsNullOrEmpty(orbit.orbitName) || string.IsNullOrEmpty(planet.bodyName)) return;

        // 같은 행성이 다른 궤도에 있었으면 해제
        orbitAssignments.RemoveAll(a => a.planetName == planet.bodyName);
        // 이 궤도에 다른 행성이 있었으면 해제
        orbitAssignments.RemoveAll(a => a.orbitName == orbit.orbitName);
        orbitAssignments.Add(new OrbitAssignment { orbitName = orbit.orbitName, planetName = planet.bodyName });
    }

    /// <summary>행성을 어떤 궤도에서도 분리 (인벤토리로 되돌림).</summary>
    public void UnassignPlanet(PlanetSO planet)
    {
        if (planet == null) return;
        orbitAssignments.RemoveAll(a => a.planetName == planet.bodyName);
    }

    /// <summary>두 궤도의 점유 행성을 맞바꾼다. 한쪽이 비어있으면 단순 이동.</summary>
    public void SwapOrbitAssignments(OrbitSO a, OrbitSO b)
    {
        if (a == null || b == null || a == b) return;
        string planetA = FindPlanetForOrbit(a.orbitName);
        string planetB = FindPlanetForOrbit(b.orbitName);

        orbitAssignments.RemoveAll(x => x.orbitName == a.orbitName || x.orbitName == b.orbitName);

        if (!string.IsNullOrEmpty(planetA))
            orbitAssignments.Add(new OrbitAssignment { orbitName = b.orbitName, planetName = planetA });
        if (!string.IsNullOrEmpty(planetB))
            orbitAssignments.Add(new OrbitAssignment { orbitName = a.orbitName, planetName = planetB });
    }

    /// <summary>현재 어떤 궤도에 배치된 행성 목록.</summary>
    public List<PlanetSO> GetAssignedPlanets()
    {
        var result = new List<PlanetSO>();
        foreach (var p in planetDeck)
            if (p != null && FindOrbitForPlanet(p.bodyName) != null) result.Add(p);
        return result;
    }

    /// <summary>덱에 있으나 어떤 궤도에도 배치되지 않은 행성 목록.</summary>
    public List<PlanetSO> GetUnassignedPlanets()
    {
        var result = new List<PlanetSO>();
        foreach (var p in planetDeck)
            if (p != null && FindOrbitForPlanet(p.bodyName) == null) result.Add(p);
        return result;
    }

    /// <summary>이름으로 덱에서 PlanetSO 찾기.</summary>
    public PlanetSO FindPlanetByName(string planetName)
    {
        if (string.IsNullOrEmpty(planetName)) return null;
        foreach (var p in planetDeck)
            if (p != null && p.bodyName == planetName) return p;
        return null;
    }
}

[Serializable]
public struct OrbitAssignment
{
    public string orbitName;
    public string planetName;
}
