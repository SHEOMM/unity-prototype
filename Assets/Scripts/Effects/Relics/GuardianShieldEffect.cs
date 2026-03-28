/// <summary>수호의 방패: 매 웨이브 첫 피격 데미지 50% 감소.</summary>
[RelicEffectId("guardian_shield")]
public class GuardianShieldEffect : RelicEffectBase
{
    private bool _shieldActive;

    public override void OnWaveStart(int waveIndex, PlayerState player)
    {
        _shieldActive = true;
    }

    public override void OnBeforeDamage(ref float damage, PlayerState player)
    {
        if (_shieldActive)
        {
            damage *= 0.5f;
            _shieldActive = false;
        }
    }
}
