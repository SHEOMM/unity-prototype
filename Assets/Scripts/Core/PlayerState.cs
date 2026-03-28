using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 플레이어 상태 싱글턴. HP, 유물, 보너스를 관리하고 게임 이벤트를 유물에 전달한다.
/// </summary>
public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    [Header("체력")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("보너스 (유물에 의해 변경됨)")]
    public float bonusDamageMultiplier = 1f;
    private readonly Dictionary<Element, float> _elementBonuses = new Dictionary<Element, float>();

    private readonly List<RelicInstance> _relics = new List<RelicInstance>();
    public IReadOnlyList<RelicInstance> Relics => _relics;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        currentHP = maxHP;
    }

    public void ResetForNewRun()
    {
        currentHP = maxHP;
        bonusDamageMultiplier = 1f;
        _elementBonuses.Clear();
        _relics.Clear();
    }

    public void AddRelic(RelicSO data)
    {
        var effect = RelicEffectRegistry.Get(data.effectId);
        if (effect == null) return;
        var instance = new RelicInstance { data = data, effect = effect };
        _relics.Add(instance);
        effect.OnAcquired(this);
        Debug.Log($"[유물] {data.relicName} 획득!");
    }

    public void TakeDamage(float damage)
    {
        for (int i = 0; i < _relics.Count; i++)
            _relics[i].effect.OnBeforeDamage(ref damage, this);

        currentHP -= damage;
        DamagePopup.Spawn(Camera.main.transform.position + Vector3.down * 3f, damage, Element.None);

        for (int i = 0; i < _relics.Count; i++)
            _relics[i].effect.OnAfterDamage(damage, this);

        if (currentHP <= 0)
        {
            currentHP = 0;
            GameManager.Instance?.EnterPhase(GamePhase.GameOver);
        }
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }

    public void AddElementBonus(Element element, float bonus)
    {
        if (_elementBonuses.ContainsKey(element))
            _elementBonuses[element] += bonus;
        else
            _elementBonuses[element] = bonus;
    }

    public float GetElementBonus(Element element)
    {
        return _elementBonuses.TryGetValue(element, out float bonus) ? bonus : 0f;
    }

    public void NotifySlashPerformed(SlashResult result)
    {
        for (int i = 0; i < _relics.Count; i++)
            _relics[i].effect.OnSlashPerformed(result, this);
    }

    public void NotifyEnemyKilled(Enemy enemy)
    {
        for (int i = 0; i < _relics.Count; i++)
            _relics[i].effect.OnEnemyKilled(enemy, this);
    }

    public void NotifyWaveStart(int waveIndex)
    {
        for (int i = 0; i < _relics.Count; i++)
            _relics[i].effect.OnWaveStart(waveIndex, this);
    }

    public void NotifyWaveComplete(int waveIndex)
    {
        for (int i = 0; i < _relics.Count; i++)
            _relics[i].effect.OnWaveComplete(waveIndex, this);
    }
}

public class RelicInstance
{
    public RelicSO data;
    public IRelicEffect effect;
}
