using System.Collections.Generic;

/// <summary>
/// RunState의 orbitAssignments / unlockedOrbits / planetDeck에 대한 read-only 조회.
/// 기존 RunState.Find*/GetAssigned*/GetUnassigned*/FindPlanetByName 을 정적 이전.
/// 상태 변이는 CosmosService 담당.
/// </summary>
public static class OrbitAssignmentQuery
{
    public static OrbitSO FindOrbitForPlanet(RunState run, string planetName)
    {
        if (run == null) return null;
        foreach (var a in run.orbitAssignments)
            if (a.planetName == planetName)
                foreach (var o in run.unlockedOrbits)
                    if (o != null && o.orbitName == a.orbitName) return o;
        return null;
    }

    public static string FindPlanetForOrbit(RunState run, string orbitName)
    {
        if (run == null) return null;
        foreach (var a in run.orbitAssignments)
            if (a.orbitName == orbitName) return a.planetName;
        return null;
    }

    public static PlanetSO FindPlanetByName(RunState run, string planetName)
    {
        if (run == null || string.IsNullOrEmpty(planetName)) return null;
        foreach (var p in run.planetDeck)
            if (p != null && p.bodyName == planetName) return p;
        return null;
    }

    public static List<PlanetSO> GetAssignedPlanets(RunState run)
    {
        var result = new List<PlanetSO>();
        if (run == null) return result;
        foreach (var p in run.planetDeck)
            if (p != null && FindOrbitForPlanet(run, p.bodyName) != null) result.Add(p);
        return result;
    }

    public static List<PlanetSO> GetUnassignedPlanets(RunState run)
    {
        var result = new List<PlanetSO>();
        if (run == null) return result;
        foreach (var p in run.planetDeck)
            if (p != null && FindOrbitForPlanet(run, p.bodyName) == null) result.Add(p);
        return result;
    }
}
