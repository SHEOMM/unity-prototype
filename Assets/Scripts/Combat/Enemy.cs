using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;
    public float moveSpeed = 1f;
    private float _baseSpeed;
    private SpriteRenderer _sr;
    private Color _origColor;
    private float _flashTimer;
    private float _deathTimer = -1f;
    private readonly List<StatusEffect> _statuses = new List<StatusEffect>();

    void OnEnable() { if (EnemyRegistry.Instance != null) EnemyRegistry.Instance.Register(this); }
    void OnDisable() { if (EnemyRegistry.Instance != null) EnemyRegistry.Instance.Unregister(this); }

    public void Initialize(float hp, float speed, Sprite sprite)
    {
        maxHP = hp;
        currentHP = hp;
        moveSpeed = speed;
        _baseSpeed = speed;
        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null) _sr = gameObject.AddComponent<SpriteRenderer>();
        if (sprite != null) _sr.sprite = sprite;
        _sr.color = new Color(0.8f, 0.2f, 0.2f);
        _sr.sortingOrder = 5;
        _origColor = _sr.color;
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

        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        if (_flashTimer > 0)
        {
            _flashTimer -= Time.deltaTime;
            _sr.color = Color.Lerp(_origColor, Color.white, _flashTimer * 5f);
        }
        if (transform.position.x < -12f) _deathTimer = 0.01f;
    }

    public void TakeDamage(float dmg, Element element = Element.None)
    {
        if (_deathTimer >= 0) return;
        currentHP -= dmg;
        _flashTimer = 0.2f;
        DamagePopup.Spawn(transform.position, dmg, element);
        if (currentHP <= 0) _deathTimer = 0.1f;
    }

    public void ApplyStatus(StatusEffect status)
    {
        _statuses.Add(status);
    }
}
