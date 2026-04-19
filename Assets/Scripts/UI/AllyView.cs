using UnityEngine;

/// <summary>
/// 아군 유닛 UI. EnemyView 패턴 미러링.
/// AllyUnit(모델)은 이벤트만 발행, View가 표시를 담당.
/// AllySpawner.SpawnAt에서 AddComponent.
/// </summary>
public class AllyView : MonoBehaviour
{
    private AllyUnit _ally;
    private UIFactory.HPBarHandle _hpBar;

    void Start()
    {
        _ally = GetComponent<AllyUnit>();
        if (_ally == null || _ally.Data == null) return;

        UIFactory.CreateLabel(transform, _ally.Data.allyName,
            GameConstants.EnemyUI.LabelYOffset,
            GameConstants.EnemyUI.LabelScale,
            new Color(0.6f, 0.9f, 1f),
            GameConstants.SortingOrder.EnemyLabel);

        _hpBar = UIFactory.CreateHPBar(transform, GameConstants.EnemyUI.HPBarYOffset);

        _ally.OnDamaged += HandleDamaged;
    }

    void Update()
    {
        if (_ally == null) return;
        UIFactory.UpdateHPBar(_hpBar, _ally.currentHP, _ally.maxHP);
    }

    void HandleDamaged(float damage, Element element)
    {
        DamagePopup.Spawn(transform.position, damage, element);
    }

    void OnDestroy()
    {
        if (_ally != null) _ally.OnDamaged -= HandleDamaged;
    }
}
