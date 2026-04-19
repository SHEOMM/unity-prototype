/// <summary>
/// SynergyRuleSO와 SynergyContext를 받아 발동 여부를 판정한다.
/// triggerType에 따라 분기. 순수 함수 — 상태 없음.
/// </summary>
public static class SynergyRuleMatcher
{
    public static bool Matches(SynergyRuleSO rule, SynergyContext ctx)
    {
        if (rule == null || ctx == null) return false;
        switch (rule.triggerType)
        {
            case SynergyTriggerType.FamilyAccumulation: return MatchFamilyAccumulation(rule, ctx);
            case SynergyTriggerType.SequencePosition:   return MatchSequencePosition(rule, ctx);
            case SynergyTriggerType.PlanetCombo:        return MatchPlanetCombo(rule, ctx);
            case SynergyTriggerType.PerHitPlanet:       return MatchPerHitPlanet(rule, ctx);
            default: return false;
        }
    }

    /// <summary>현재 터치 중인 행성의 bodyName 또는 keywords가 planetKey와 일치하면 true.</summary>
    static bool MatchPerHitPlanet(SynergyRuleSO rule, SynergyContext ctx)
    {
        if (ctx.CurrentPlanet?.Planet == null || string.IsNullOrEmpty(rule.planetKey)) return false;
        return MatchesPlanetKey(ctx.CurrentPlanet.Planet, rule.planetKey);
    }

    static bool MatchFamilyAccumulation(SynergyRuleSO rule, SynergyContext ctx)
    {
        if (ctx.Families == null) return false;
        return ctx.Families.Count(rule.family) >= rule.threshold;
    }

    static bool MatchSequencePosition(SynergyRuleSO rule, SynergyContext ctx)
    {
        var seq = ctx.HitSequence;
        if (seq == null || seq.Count == 0 || string.IsNullOrEmpty(rule.planetKey)) return false;

        bool HasKey(PlanetBody p) => p?.Planet != null && MatchesPlanetKey(p.Planet, rule.planetKey);

        switch (rule.positionKey)
        {
            case SequencePosition.Leading:
                return HasKey(seq[0]);
            case SequencePosition.Trailing:
                return HasKey(seq[seq.Count - 1]);
            case SequencePosition.Any:
                for (int i = 0; i < seq.Count; i++) if (HasKey(seq[i])) return true;
                return false;
            default: return false;
        }
    }

    static bool MatchPlanetCombo(SynergyRuleSO rule, SynergyContext ctx)
    {
        if (ctx.HitSequence == null || rule.sequenceKeys == null || rule.sequenceKeys.Length == 0) return false;
        foreach (var key in rule.sequenceKeys)
        {
            if (string.IsNullOrEmpty(key)) continue;
            bool found = false;
            foreach (var p in ctx.HitSequence)
            {
                if (p?.Planet != null && MatchesPlanetKey(p.Planet, key)) { found = true; break; }
            }
            if (!found) return false;
        }
        return true;
    }

    /// <summary>bodyName 일치 또는 keywords 중 하나와 일치.</summary>
    static bool MatchesPlanetKey(PlanetSO planet, string key)
    {
        if (planet.bodyName == key) return true;
        if (planet.keywords != null)
        {
            foreach (var kw in planet.keywords)
                if (kw == key) return true;
        }
        return false;
    }
}
