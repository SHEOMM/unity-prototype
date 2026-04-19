using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 아군 유닛 런타임. Enemy와 대칭: HP/피격/상태이상/넉백 공통, 행동은 IAllyBehavior.
/// IStatusHost(=IDamageable) + IMoveable 구현 → 기존 primitive 재사용 가능.
///
/// 기본 이동: 가장 가까운 적으로 접근. 공격 범위 내면 주기적으로 데미지.
/// 세부 로직은 IAllyBehavior 구현체가 override (Tick이 false 반환 시 기본 이동/공격 스킵).
/// </summary>
public class AllyUnit : MonoBehaviour, IStatusHost, IMoveable
{
    public AllySO Data { get; private set; }
    public float maxHP;
    public float currentHP;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float attackInterval;
    private float _attackCooldown;

    public event Action<float, Element> OnDamaged;
    public event Action OnDeath;

    private Vector2 _knockback;
    private float _baseSpeed;
    private SpriteRenderer _sr;
    private float _flashTimer;
    private float _deathTimer = -1f;
    private readonly List<StatusEffect> _statuses = new List<StatusEffect>();
    public IReadOnlyList<StatusEffect> ActiveStatuses => _statuses;

    private IAllyBehavior _behavior;

    // ── IDamageable ────────────────────────────────────────────
    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public Vector3 Position => transform.position;
    public bool IsAlive => _deathTimer < 0f;

    // ── IMoveable ─────────────────────────────────────────────
    public float BaseSpeed => _baseSpeed;
    public void ApplyKnockback(Vector2 delta) { _knockback += delta; }

    void OnEnable() { if (AllyRegistry.Instance != null) AllyRegistry.Instance.Register(this); }
    void OnDisable() { if (AllyRegistry.Instance != null) AllyRegistry.Instance.Unregister(this); }

    public void Initialize(AllySO data, Sprite sprite)
    {
        Data = data;
        maxHP = data.baseHP;
        currentHP = data.baseHP;
        moveSpeed = data.moveSpeed;
        _baseSpeed = data.moveSpeed;
        attackDamage = data.attackDamage;
        attackRange = data.attackRange;
        attackInterval = data.attackInterval;

        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null) _sr = gameObject.AddComponent<SpriteRenderer>();
        if (sprite != null) _sr.sprite = sprite;
        _sr.color = data.bodyColor;
        _sr.sortingOrder = GameConstants.SortingOrder.EnemyBody;
        transform.localScale = Vector3.one * data.scale;

        _behavior = AllyBehaviorRegistry.Get(data.behaviorId);
    }

    void Update()
    {
        if (_deathTimer >= 0f)
        {
            _deathTimer -= Time.deltaTime;
            if (_deathTimer <= 0f) Destroy(gameObject);
            return;
        }

        moveSpeed = _baseSpeed;

        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            _statuses[i].Tick(this, Time.deltaTime);
            if (_statuses[i].IsExpired) _statuses.RemoveAt(i);
        }

        bool doDefault = _behavior?.Tick(this, Time.deltaTime) ?? true;
        if (doDefault) DefaultTick(Time.deltaTime);

        // 넉백
        if (_knockback.sqrMagnitude > 0.0001f)
        {
            transform.position += (Vector3)(_knockback * Time.deltaTime);
            _knockback *= Mathf.Exp(-5f * Time.deltaTime);
        }

        if (_flashTimer > 0)
        {
            _flashTimer -= Time.deltaTime;
            _sr.color = Color.Lerp(Data != null ? Data.bodyColor : Color.white, Color.white,
                                   _flashTimer * GameConstants.Combat.DamageFlashSpeed);
        }
    }

    /// <summary>기본 AI: 가장 가까운 적으로 접근 + 범위 내면 공격. Stun(moveSpeed≤0) 시 스킵.</summary>
    void DefaultTick(float dt)
    {
        if (moveSpeed <= 0f) return; // Stun 가드

        var target = FindNearestEnemy();
        if (target == null) return;

        Vector2 toTarget = (Vector2)target.transform.position - (Vector2)transform.position;
        float dist = toTarget.magnitude;

        if (dist > attackRange)
        {
            // 접근
            Vector2 dir = toTarget / Mathf.Max(dist, 1e-4f);
            transform.Translate(dir * moveSpeed * dt);
        }
        else
        {
            // 공격
            _attackCooldown -= dt;
            if (_attackCooldown <= 0f)
            {
                _attackCooldown = attackInterval;
                target.TakeDamage(attackDamage);
            }
        }
    }

    Enemy FindNearestEnemy()
    {
        if (EnemyRegistry.Instance == null) return null;
        var list = EnemyRegistry.Instance.GetAll();
        Enemy best = null;
        float bestDistSq = float.MaxValue;
        foreach (var e in list)
        {
            if (e == null || !e.IsAlive) continue;
            float d = ((Vector2)e.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (d < bestDistSq) { bestDistSq = d; best = e; }
        }
        return best;
    }

    public void TakeDamage(float dmg, Element element = Element.None)
    {
        if (_deathTimer >= 0) return;

        // Enemy와 동일한 DamageModifier 순회 (Weakness 등)
        for (int i = 0; i < _statuses.Count; i++)
            if (_statuses[i].effect is IDamageModifier mod)
                dmg = mod.ModifyIncoming(dmg, element);

        if (_behavior != null)
            dmg = _behavior.ModifyIncomingDamage(this, dmg, element);

        currentHP -= dmg;
        _flashTimer = GameConstants.Combat.DamageFlashDuration;
        OnDamaged?.Invoke(dmg, element);

        if (currentHP <= 0)
        {
            OnDeath?.Invoke();
            bool normalDeath = _behavior?.OnDeath(this) ?? true;
            if (normalDeath) _deathTimer = GameConstants.Combat.DeathTimerNormal;
        }
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }

    public void ApplyStatus(StatusEffect status)
    {
        _statuses.Add(status);
    }
}
