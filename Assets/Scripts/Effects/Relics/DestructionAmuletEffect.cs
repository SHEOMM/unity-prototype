/// <summary>파괴의 부적: 슬래시 데미지 +15%.</summary>
[RelicEffectId("destruction_amulet")]
public class DestructionAmuletEffect : RelicEffectBase
{
    public override void OnAcquired(PlayerState player)
    {
        player.bonusDamageMultiplier += 0.15f;
    }
}
