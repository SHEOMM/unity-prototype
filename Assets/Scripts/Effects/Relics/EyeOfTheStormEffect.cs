/// <summary>폭풍의 눈: 바람 속성 데미지 +30%.</summary>
[RelicEffectId("eye_of_storm")]
public class EyeOfTheStormEffect : RelicEffectBase
{
    public override void OnAcquired(PlayerState player)
    {
        player.AddElementBonus(Element.Wind, 0.3f);
    }
}
