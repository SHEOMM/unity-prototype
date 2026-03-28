using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 현재 런의 진행 상태. 층, 덱, 시드를 추적한다.
/// </summary>
public class RunState : MonoBehaviour
{
    public static RunState Instance { get; private set; }

    public int currentFloor;
    public int currentAct;
    public int runSeed;

    public List<StarSO> starDeck = new List<StarSO>();
    public List<PlanetSO> planetDeck = new List<PlanetSO>();
    public List<SatelliteSO> satellites = new List<SatelliteSO>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void InitializeRun(int seed, StarSO[] startingStars, PlanetSO[] startingPlanets)
    {
        runSeed = seed;
        currentFloor = 0;
        currentAct = 1;
        starDeck = new List<StarSO>(startingStars);
        planetDeck = new List<PlanetSO>(startingPlanets);
        satellites.Clear();
    }

    public void AddToDeck(CelestialBodySO item)
    {
        if (item is StarSO star) starDeck.Add(star);
        else if (item is PlanetSO planet) planetDeck.Add(planet);
        else if (item is SatelliteSO sat) satellites.Add(sat);
    }

    public void AdvanceFloor() { currentFloor++; }
}
