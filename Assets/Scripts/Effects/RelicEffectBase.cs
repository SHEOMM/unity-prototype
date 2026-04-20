/// <summary>
/// 유물 효과 추상 베이스. 모든 훅이 no-op이므로 필요한 것만 override하면 된다.
/// </summary>
public abstract class RelicEffectBase : IRelicEffect
{
    public virtual void OnAcquired(PlayerState player) { }
    public virtual void OnSpellPerformed(SpellResult result, PlayerState player) { }
    public virtual void OnEnemyKilled(Enemy enemy, PlayerState player) { }
    public virtual void OnWaveStart(int waveIndex, PlayerState player) { }
    public virtual void OnWaveComplete(int waveIndex, PlayerState player) { }
    public virtual void OnBeforeDamage(ref float damage, PlayerState player) { }
    public virtual void OnAfterDamage(float damage, PlayerState player) { }
}
