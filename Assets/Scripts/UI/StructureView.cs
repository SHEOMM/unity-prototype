using UnityEngine;

/// <summary>
/// 구조물 UI. EnemyView 패턴 미러링 (이동 없으므로 Label + HP 바).
/// StructureSpawner.SpawnAt에서 AddComponent.
/// </summary>
public class StructureView : MonoBehaviour
{
    private Structure _structure;
    private UIFactory.HPBarHandle _hpBar;

    void Start()
    {
        _structure = GetComponent<Structure>();
        if (_structure == null || _structure.Data == null) return;

        UIFactory.CreateLabel(transform, _structure.Data.structureName,
            GameConstants.EnemyUI.LabelYOffset,
            GameConstants.EnemyUI.LabelScale,
            new Color(0.8f, 0.9f, 0.6f),
            GameConstants.SortingOrder.EnemyLabel);

        _hpBar = UIFactory.CreateHPBar(transform, GameConstants.EnemyUI.HPBarYOffset);

        _structure.OnDamaged += HandleDamaged;
    }

    void Update()
    {
        if (_structure == null) return;
        UIFactory.UpdateHPBar(_hpBar, _structure.currentHP, _structure.maxHP);
    }

    void HandleDamaged(float damage, Element element)
    {
        DamagePopup.Spawn(transform.position, damage, element);
    }

    void OnDestroy()
    {
        if (_structure != null) _structure.OnDamaged -= HandleDamaged;
    }
}
