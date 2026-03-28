/// <summary>별의 축복: 웨이브 종료 시 HP 5 회복.</summary>
[RelicEffectId("stellar_blessing")]
public class StellarBlessingEffect : RelicEffectBase
{
    public override void OnWaveComplete(int waveIndex, PlayerState player)
    {
        player.Heal(5f);
    }
}
