using UnityEngine;

/// <summary>
/// 우두머리: 5초 주기 정지, HP 30% 이하 광폭화, 20% 데미지 감소.
/// </summary>
[EnemyBehaviorId("boss")]
public class BossBehavior : IEnemyBehavior
{
    private float _phaseTimer;
    private bool _enraged;

    public bool Tick(Enemy enemy, float dt)
    {
        _phaseTimer += dt;

        // HP 30% 이하에서 광폭화
        if (!_enraged && enemy.currentHP < enemy.maxHP * 0.3f)
        {
            _enraged = true;
            enemy.moveSpeed *= 1.5f;
        }

        // 5초 주기: 4초 이동 + 1초 정지
        bool paused = (_phaseTimer % 5f) > 4f;
        return !paused;
    }

    public float ModifyIncomingDamage(Enemy enemy, float dmg, Element el) => dmg * 0.8f;
    public bool OnDeath(Enemy enemy) => true;
}
