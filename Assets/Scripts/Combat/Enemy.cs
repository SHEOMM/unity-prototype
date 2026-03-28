using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class Enemy : MonoBehaviour
{
    public EnemySO Data { get; private set; }
    public float maxHP;
    public float currentHP;
    public float moveSpeed;
    private float _baseSpeed;
    private SpriteRenderer _sr;
    private Color _origColor;
    private float _flashTimer;
    private float _deathTimer = -1f;
    private readonly List<StatusEffect> _statuses = new List<StatusEffect>();

    private IEnemyBehavior _behavior;
    private IEnemyState _state;

    void OnEnable() { if (EnemyRegistry.Instance != null) EnemyRegistry.Instance.Register(this); }
    void OnDisable() { if (EnemyRegistry.Instance != null) EnemyRegistry.Instance.Unregister(this); }

    public void Initialize(EnemySO data, Sprite sprite)
    {
        Data = data;
        maxHP = data.baseHP;
        currentHP = data.baseHP;
        moveSpeed = data.moveSpeed;
        _baseSpeed = data.moveSpeed;

        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null) _sr = gameObject.AddComponent<SpriteRenderer>();
        if (sprite != null) _sr.sprite = sprite;
        _sr.color = Color.white;
        _sr.sortingOrder = 5;
        transform.localScale = Vector3.one * data.scale;
        _origColor = Color.white;

        _behavior = EnemyBehaviorRegistry.Get(data.behaviorId);
        AttachStateIfNeeded(data.behaviorId);

        CreateLabel(data.enemyName, data.scale);
    }

    void AttachStateIfNeeded(string behaviorId)
    {
        var behaviorType = EnemyBehaviorRegistry.GetBehaviorType(behaviorId);
        if (behaviorType == null) return;
        var stateAttr = behaviorType.GetCustomAttribute<EnemyStateAttribute>();
        if (stateAttr?.StateType == null) return;
        _state = (IEnemyState)gameObject.AddComponent(stateAttr.StateType);
    }

    void CreateLabel(string text, float parentScale)
    {
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(transform);
        labelGo.transform.localPosition = new Vector3(0, -0.8f, 0);
        float invScale = parentScale > 0.01f ? 1f / parentScale : 1f;
        labelGo.transform.localScale = Vector3.one * invScale * 0.4f;

        var tm = labelGo.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 48;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1f, 0.8f, 0.8f, 0.85f);
        tm.characterSize = 0.1f;

        var mr = labelGo.GetComponent<MeshRenderer>();
        mr.sortingOrder = 12;
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
        _state?.Tick(Time.deltaTime);

        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            _statuses[i].Tick(this, Time.deltaTime);
            if (_statuses[i].IsExpired) _statuses.RemoveAt(i);
        }

        bool doDefaultMove = _behavior?.Tick(this, Time.deltaTime) ?? true;
        if (doDefaultMove)
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        if (_flashTimer > 0)
        {
            _flashTimer -= Time.deltaTime;
            _sr.color = Color.Lerp(_origColor, Color.white, _flashTimer * 5f);
        }
        if (transform.position.x < -12f)
        {
            if (PlayerState.Instance != null && Data != null)
                PlayerState.Instance.TakeDamage(Data.attackDamage);
            _deathTimer = 0.01f;
        }
    }

    public void TakeDamage(float dmg, Element element = Element.None)
    {
        if (_deathTimer >= 0) return;

        dmg = ApplyElementResistance(dmg, element);

        if (_behavior != null)
            dmg = _behavior.ModifyIncomingDamage(this, dmg, element);

        currentHP -= dmg;
        _flashTimer = 0.2f;
        DamagePopup.Spawn(transform.position, dmg, element);

        if (currentHP <= 0)
        {
            PlayerState.Instance?.NotifyEnemyKilled(this);
            bool normalDeath = _behavior?.OnDeath(this) ?? true;
            if (normalDeath) _deathTimer = 0.1f;
        }
    }

    float ApplyElementResistance(float dmg, Element element)
    {
        if (Data?.resistances == null || element == Element.None) return dmg;
        foreach (var r in Data.resistances)
            if (r.element == element) return dmg * r.multiplier;
        return dmg;
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
