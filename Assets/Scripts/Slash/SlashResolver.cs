using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 관통된 행성 목록을 해석하여 SlashResult를 생성한다.
/// 파이프라인: PreProcess(히트리스트 변경) → Execute(효과 실행) → PostProcess(시저지/합산)
/// </summary>
public class SlashResolver : MonoBehaviour
{
    public SynergyDefinitionSO[] synergies;

    public SlashResult Resolve(List<PlanetBody> hits)
    {
        if (hits.Count == 0) return new SlashResult();

        // Phase 1: PreProcess — ISlashModifier가 히트리스트를 변경할 수 있다
        hits = ApplyModifiers(hits);

        // Phase 2: Execute — 각 별의 IStarEffect를 순서대로 실행
        var result = new SlashResult { hitPlanets = hits };
        var enemies = EnemyRegistry.Instance != null ? EnemyRegistry.Instance.GetAll() : new List<Enemy>();

        for (int i = 0; i < hits.Count; i++)
        {
            var planet = hits[i];
            var leading = hits.GetRange(0, i);
            var trailing = (i + 1 < hits.Count) ? hits.GetRange(i + 1, hits.Count - i - 1) : new List<PlanetBody>();

            bool phaseActive = !string.IsNullOrEmpty(planet.Planet.phaseEffectId)
                               && trailing.Count >= planet.Planet.phaseThreshold;
            string effectId = phaseActive ? planet.Planet.phaseEffectId : planet.Planet.effectId;
            var effect = EffectRegistry.Get(effectId);

            var ctx = new EffectContext
            {
                source = planet,
                positionIndex = i,
                allHits = hits,
                leading = leading,
                trailing = trailing,
                enemies = enemies,
                damageMultiplier = SatelliteBuffCalculator.GetDamageMultiplier(planet),
                extraHits = SatelliteBuffCalculator.GetExtraHits(planet),
                areaMultiplier = SatelliteBuffCalculator.GetAreaMultiplier(planet),
                isPhaseActive = phaseActive,
                result = result
            };

            effect.Execute(ctx);
        }

        // Phase 3: PostProcess — 키워드 시저지 보너스 + 합산
        ApplySynergies(hits, result);
        CalculateTotal(result);
        return result;
    }

    List<PlanetBody> ApplyModifiers(List<PlanetBody> hits)
    {
        var modified = new List<PlanetBody>(hits);
        for (int i = 0; i < modified.Count; i++)
        {
            var effect = EffectRegistry.Get(modified[i].Planet.effectId);
            if (effect is ISlashModifier modifier)
                modified = modifier.ModifyHitList(modified, i, modified[i]);
        }
        return modified;
    }

    void ApplySynergies(List<PlanetBody> hits, SlashResult result)
    {
        if (synergies == null) return;
        var kwCount = new Dictionary<string, int>();
        foreach (var p in hits)
        {
            if (p.Planet.keywords == null) continue;
            foreach (var kw in p.Planet.keywords)
            {
                if (!kwCount.ContainsKey(kw)) kwCount[kw] = 0;
                kwCount[kw]++;
            }
        }

        foreach (var syn in synergies)
        {
            if (kwCount.ContainsKey(syn.requiredKeyword) && kwCount[syn.requiredKeyword] >= syn.requiredCount)
            {
                result.activatedSynergies.Add(syn);
                foreach (var cmd in result.commands)
                    cmd.damage *= syn.damageMultiplier;
            }
        }
    }

    void CalculateTotal(SlashResult result)
    {
        float total = 0;
        foreach (var cmd in result.commands)
            total += cmd.damage * cmd.hitCount;
        result.totalDamage = total;
    }
}
