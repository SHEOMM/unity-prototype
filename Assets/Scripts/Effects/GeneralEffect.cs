using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 장군별: 시저지에 포함되지 않은 반경 5 이내의 별들을 징집한다.
/// 징집된 별은 장군별 바로 뒤에 삽입되며, 장군별에서 멀수록 뒤에 배치된다.
/// 징집된 별들은 자신의 고유 효과를 정상 발동한다.
/// </summary>
[EffectId("general")]
public class GeneralEffect : IStarEffect, ISlashModifier
{
    public const float ConscriptRadius = 5f;

    public List<PlanetBody> ModifyHitList(List<PlanetBody> currentHits, int sourceIndex, PlanetBody source)
    {
        if (PlanetRegistry.Instance == null) return currentHits;

        var allPlanets = PlanetRegistry.Instance.GetAll();
        Vector2 generalPos = source.transform.position;

        var conscripts = new List<(PlanetBody planet, float distance)>();
        foreach (var p in allPlanets)
        {
            if (currentHits.Contains(p)) continue;
            float dist = Vector2.Distance(generalPos, (Vector2)p.transform.position);
            if (dist <= ConscriptRadius)
                conscripts.Add((p, dist));
        }

        if (conscripts.Count == 0) return currentHits;

        conscripts.Sort((a, b) => a.distance.CompareTo(b.distance));

        var modified = new List<PlanetBody>(currentHits);
        int insertAt = sourceIndex + 1;
        foreach (var (planet, _) in conscripts)
            modified.Insert(insertAt++, planet);

        return modified;
    }

    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        ctx.result.commands.Add(new SpellCommand
        {
            element = ctx.source.Planet.element,
            damage = dmg,
            hitCount = 1 + ctx.trailing.Count,
            sourceName = ctx.source.Planet.bodyName,
            visualType = SpellVisualType.Strike
        });
    }
}
