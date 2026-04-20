/// <summary>
/// 궤도·배치 상태 변이 전담 정적 서비스. 기존 RunState 내부의 UnlockOrbit/Assign/Unassign/Swap/TryAutoAssign*를 이전.
/// 정적 클래스로 구현해 PersistentScene GameObject 추가를 피하고, RunState를 인자로 받는 형태.
/// 자동 배치 진입점은 TryAutoPlace — RewardManager.Apply가 ApplyAsReward 직후 호출.
/// </summary>
public static class CosmosService
{
    /// <summary>
    /// 보상 payload 종류에 따라 자동 배치를 시도. OrbitSO면 미배치 행성 탐색, PlanetSO면 빈 궤도 탐색.
    /// 그 외 payload는 무시. ApplyAsReward 직후 호출되는 단일 정책 진입점.
    /// </summary>
    public static void TryAutoPlace(RunState run, IRewardApplicable payload)
    {
        if (run == null || payload == null) return;
        switch (payload)
        {
            case OrbitSO orbit: TryAutoAssignPlanetToOrbit(run, orbit); break;
            case PlanetSO planet: TryAutoAssignOrbitToPlanet(run, planet); break;
        }
    }

    public static void UnlockOrbit(RunState run, OrbitSO orbit)
    {
        if (run == null || orbit == null) return;
        if (!run.unlockedOrbits.Contains(orbit)) run.unlockedOrbits.Add(orbit);
    }

    /// <summary>
    /// 궤도에 행성을 배치한다. 이 행성이 다른 궤도에 있었으면 제거, 이 궤도의 기존 점유자도 제거.
    /// 양쪽 모두 제거된 상태에서 새 배치 1건을 추가하는 원자적 변이.
    /// </summary>
    public static void Assign(RunState run, OrbitSO orbit, PlanetSO planet)
    {
        if (run == null || orbit == null || planet == null) return;
        if (string.IsNullOrEmpty(orbit.orbitName) || string.IsNullOrEmpty(planet.bodyName)) return;

        run.orbitAssignments.RemoveAll(a => a.planetName == planet.bodyName);
        run.orbitAssignments.RemoveAll(a => a.orbitName == orbit.orbitName);
        run.orbitAssignments.Add(new OrbitAssignment { orbitName = orbit.orbitName, planetName = planet.bodyName });
    }

    public static void Unassign(RunState run, PlanetSO planet)
    {
        if (run == null || planet == null) return;
        run.orbitAssignments.RemoveAll(a => a.planetName == planet.bodyName);
    }

    /// <summary>두 궤도의 점유 행성을 맞바꾼다. 한쪽이 비어있으면 단순 이동.</summary>
    public static void Swap(RunState run, OrbitSO a, OrbitSO b)
    {
        if (run == null || a == null || b == null || a == b) return;
        string planetA = OrbitAssignmentQuery.FindPlanetForOrbit(run, a.orbitName);
        string planetB = OrbitAssignmentQuery.FindPlanetForOrbit(run, b.orbitName);

        run.orbitAssignments.RemoveAll(x => x.orbitName == a.orbitName || x.orbitName == b.orbitName);

        if (!string.IsNullOrEmpty(planetA))
            run.orbitAssignments.Add(new OrbitAssignment { orbitName = b.orbitName, planetName = planetA });
        if (!string.IsNullOrEmpty(planetB))
            run.orbitAssignments.Add(new OrbitAssignment { orbitName = a.orbitName, planetName = planetB });
    }

    public static bool TryAutoAssignPlanetToOrbit(RunState run, OrbitSO orbit)
    {
        if (run == null || orbit == null || string.IsNullOrEmpty(orbit.orbitName)) return false;
        if (OrbitAssignmentQuery.FindPlanetForOrbit(run, orbit.orbitName) != null) return false;

        foreach (var planet in run.planetDeck)
        {
            if (planet == null) continue;
            if (OrbitAssignmentQuery.FindOrbitForPlanet(run, planet.bodyName) != null) continue;
            run.orbitAssignments.Add(new OrbitAssignment { orbitName = orbit.orbitName, planetName = planet.bodyName });
            return true;
        }
        return false;
    }

    public static bool TryAutoAssignOrbitToPlanet(RunState run, PlanetSO planet)
    {
        if (run == null || planet == null || string.IsNullOrEmpty(planet.bodyName)) return false;
        if (OrbitAssignmentQuery.FindOrbitForPlanet(run, planet.bodyName) != null) return false;

        foreach (var orbit in run.unlockedOrbits)
        {
            if (orbit == null) continue;
            if (OrbitAssignmentQuery.FindPlanetForOrbit(run, orbit.orbitName) != null) continue;
            run.orbitAssignments.Add(new OrbitAssignment { orbitName = orbit.orbitName, planetName = planet.bodyName });
            return true;
        }
        return false;
    }
}
