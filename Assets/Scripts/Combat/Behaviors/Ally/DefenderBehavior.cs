using UnityEngine;

/// <summary>
/// 방어형 아군: 스폰 지점을 중심으로 patrolRadius 내에만 움직이며, 진입한 적에게 접근·공격.
/// 범위 밖으로 나가지 않음 (전선 유지). War 시너지의 기본 아군 유형으로 사용.
///
/// Tick 반환:
///   true — 기본 AI 허용 (하지만 아래에서 직접 이동 처리 후 false 반환할 수도 있음)
///   false — 스킵 (본 클래스가 이동·공격 모두 담당)
/// </summary>
[AllyBehaviorId("defender")]
public class DefenderBehavior : IAllyBehavior
{
    public float patrolRadius = 3f;
    private Vector3 _origin;
    private bool _originSet;
    private float _attackCooldown;

    public bool Tick(AllyUnit ally, float deltaTime)
    {
        if (!_originSet)
        {
            _origin = ally.transform.position;
            _originSet = true;
        }

        // Stun 체크 (moveSpeed ≤ 0)
        if (ally.moveSpeed <= 0f) return false;

        // patrolRadius 내 적 찾기
        var target = FindEnemyInRange(ally);
        if (target == null)
        {
            // 대기 위치로 복귀
            MoveToward(ally, _origin, deltaTime);
            return false;
        }

        float dist = Vector2.Distance(target.transform.position, ally.transform.position);
        if (dist > ally.attackRange)
        {
            // 적이 사거리 밖이면 접근 (단, origin 반경 내에서만)
            Vector3 nextPos = Vector3.MoveTowards(ally.transform.position, target.transform.position,
                                                   ally.moveSpeed * deltaTime);
            if (Vector3.Distance(nextPos, _origin) <= patrolRadius)
                ally.transform.position = nextPos;
        }
        else
        {
            _attackCooldown -= deltaTime;
            if (_attackCooldown <= 0f)
            {
                _attackCooldown = ally.attackInterval;
                target.TakeDamage(ally.attackDamage);
            }
        }
        return false; // 본 behavior가 이동·공격 완수, 기본 AI 스킵
    }

    public float ModifyIncomingDamage(AllyUnit ally, float dmg, Element element) => dmg;
    public bool OnDeath(AllyUnit ally) => true;

    Enemy FindEnemyInRange(AllyUnit ally)
    {
        if (EnemyRegistry.Instance == null) return null;
        var pos = (Vector2)ally.transform.position;
        Enemy best = null;
        float bestDistSq = float.MaxValue;
        foreach (var e in EnemyRegistry.Instance.GetAll())
        {
            if (e == null || !e.IsAlive) continue;
            if (Vector2.Distance(e.transform.position, _origin) > patrolRadius + ally.attackRange) continue;
            float d = ((Vector2)e.transform.position - pos).sqrMagnitude;
            if (d < bestDistSq) { bestDistSq = d; best = e; }
        }
        return best;
    }

    void MoveToward(AllyUnit ally, Vector3 point, float dt)
    {
        Vector3 delta = point - ally.transform.position;
        if (delta.sqrMagnitude < 0.01f) return;
        ally.transform.position = Vector3.MoveTowards(ally.transform.position, point, ally.moveSpeed * dt);
    }
}
