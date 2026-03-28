using UnityEngine;

/// <summary>
/// 전령별: 약한 데미지 + 이번 슬래시 내 모든 명령에 추가 타격.
/// 위상: 시간왜곡 (적 전체 둔화 - 다수 타격으로 표현)
/// </summary>
[EffectId("messenger")]
public class MessengerEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;

        if (ctx.isPhaseActive)
        {
            int enemyCount = Mathf.Max(1, ctx.enemies.Count);
            ctx.result.commands.Add(new SpellCommand
            {
                element = Element.Wind,
                damage = dmg * 1.5f,
                hitCount = enemyCount * 2,
                sourceName = ctx.source.Planet.bodyName + " (시간왜곡)",
                visualType = SpellVisualType.AreaOfEffect
            });
        }
        else
        {
            // 기존 명령에 추가 타격 부여
            foreach (var cmd in ctx.result.commands)
                cmd.hitCount += 1;

            ctx.result.commands.Add(new SpellCommand
            {
                element = ctx.source.Planet.element,
                damage = dmg,
                hitCount = 1 + ctx.extraHits,
                sourceName = ctx.source.Planet.bodyName,
                visualType = SpellVisualType.Projectile
            });
        }
    }
}
