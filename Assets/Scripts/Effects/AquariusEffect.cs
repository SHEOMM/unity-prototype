using UnityEngine;

/// <summary>
/// 물병별: 물이 일정 속도로 차오른다. 꽉 차면 비워진 후 다시 차오른다.
/// 시저지에 포함될 시 물을 소진해 적들을 관통하는 파도를 생성한다.
/// 데미지는 담겨 있던 물의 양에 비례한다.
/// </summary>
[EffectId("aquarius")]
[PlanetState(typeof(AquariusState))]
[PlanetHUD(typeof(AquariusHUD))]
public class AquariusEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        var state = ctx.source.GetState<AquariusState>();
        float waterRatio = 0.5f;

        if (state != null)
        {
            float consumed = state.ConsumeWater();
            waterRatio = state.maxWater > 0f ? consumed / state.maxWater : 0f;
        }

        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier * Mathf.Max(0.1f, waterRatio);
        int pierceCount = Mathf.Max(1, ctx.enemies.Count);

        ctx.result.commands.Add(new SpellCommand
        {
            element = Element.Water,
            damage = dmg,
            hitCount = pierceCount + ctx.extraHits,
            sourceName = ctx.source.Planet.bodyName,
            visualType = SpellVisualType.AreaOfEffect,
            visualId = "aquarius"
        });
    }
}
