using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 구조물 런타임. 이동 없음(IMoveable 미구현). HP 있음(파괴 가능).
/// IStructureBehavior가 매 프레임 오라/공격/버프 등 고유 로직을 수행.
/// </summary>
public class Structure : MonoBehaviour, IStatusHost
{
    public StructureSO Data { get; private set; }
    public float maxHP;
    public float currentHP;

    public event Action<float, Element> OnDamaged;
    public event Action OnDeath;

    private SpriteRenderer _sr;
    private float _flashTimer;
    private float _deathTimer = -1f;
    private readonly List<StatusEffect> _statuses = new List<StatusEffect>();
    public IReadOnlyList<StatusEffect> ActiveStatuses => _statuses;

    private IStructureBehavior _behavior;

    // ── IDamageable ────────────────────────────────────────────
    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public Vector3 Position => transform.position;
    public bool IsAlive => _deathTimer < 0f;

    void OnEnable() { if (StructureRegistry.Instance != null) StructureRegistry.Instance.Register(this); }
    void OnDisable() { if (StructureRegistry.Instance != null) StructureRegistry.Instance.Unregister(this); }

    public void Initialize(StructureSO data, Sprite sprite)
    {
        Data = data;
        maxHP = data.baseHP;
        currentHP = data.baseHP;

        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null) _sr = gameObject.AddComponent<SpriteRenderer>();
        if (sprite != null) _sr.sprite = sprite;
        _sr.color = data.bodyColor;
        _sr.sortingOrder = GameConstants.SortingOrder.EnemyBody;
        transform.localScale = Vector3.one * data.scale;

        _behavior = StructureBehaviorRegistry.Get(data.behaviorId);
    }

    void Update()
    {
        if (_deathTimer >= 0f)
        {
            _deathTimer -= Time.deltaTime;
            if (_deathTimer <= 0f) Destroy(gameObject);
            return;
        }

        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            _statuses[i].Tick(this, Time.deltaTime);
            if (_statuses[i].IsExpired) _statuses.RemoveAt(i);
        }

        _behavior?.OnTick(this, Time.deltaTime);

        if (_flashTimer > 0)
        {
            _flashTimer -= Time.deltaTime;
            _sr.color = Color.Lerp(Data != null ? Data.bodyColor : Color.white, Color.white,
                                   _flashTimer * GameConstants.Combat.DamageFlashSpeed);
        }
    }

    public void TakeDamage(float dmg, Element element = Element.None)
    {
        if (_deathTimer >= 0) return;
        currentHP -= dmg;
        _flashTimer = GameConstants.Combat.DamageFlashDuration;
        OnDamaged?.Invoke(dmg, element);

        if (currentHP <= 0)
        {
            OnDeath?.Invoke();
            _behavior?.OnDestroyed(this);
            _deathTimer = GameConstants.Combat.DeathTimerNormal;
        }
    }

    public void ApplyStatus(StatusEffect status)
    {
        _statuses.Add(status);
    }
}
