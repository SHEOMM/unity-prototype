/// <summary>
/// 행성에 부착된 위성들의 패시브 버프를 집계한다.
/// </summary>
public static class SatelliteBuffCalculator
{
    public static float GetDamageMultiplier(PlanetBody planet)
    {
        float mult = 1f;
        foreach (var sat in planet.Satellites)
        {
            if (sat.Data.passiveType == PassiveType.DamageBoost)
                mult += sat.Data.passiveValue;
        }
        return mult;
    }

    public static float GetSpeedMultiplier(PlanetBody planet)
    {
        float mult = 1f;
        foreach (var sat in planet.Satellites)
        {
            if (sat.Data.passiveType == PassiveType.SpeedBoost)
                mult *= sat.Data.passiveValue;
        }
        return mult;
    }

    public static int GetExtraHits(PlanetBody planet)
    {
        int extra = 0;
        foreach (var sat in planet.Satellites)
        {
            if (sat.Data.passiveType == PassiveType.ExtraHit)
                extra += (int)sat.Data.passiveValue;
        }
        return extra;
    }

    public static float GetAreaMultiplier(PlanetBody planet)
    {
        float mult = 1f;
        foreach (var sat in planet.Satellites)
        {
            if (sat.Data.passiveType == PassiveType.AreaExpansion)
                mult *= sat.Data.passiveValue;
        }
        return mult;
    }
}
