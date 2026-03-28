/// <summary>
/// 유물 효과 인터페이스. Slay the Spire 유물처럼 패시브 효과를 제공한다.
/// 구현 시 RelicEffectBase를 상속하고 [RelicEffectId]를 붙이면 된다.
/// </summary>
public interface IRelicEffect
{
    void OnAcquired(PlayerState player);
    void OnSlashPerformed(SlashResult result, PlayerState player);
    void OnEnemyKilled(Enemy enemy, PlayerState player);
    void OnWaveStart(int waveIndex, PlayerState player);
    void OnWaveComplete(int waveIndex, PlayerState player);
    void OnBeforeDamage(ref float damage, PlayerState player);
    void OnAfterDamage(float damage, PlayerState player);
}
