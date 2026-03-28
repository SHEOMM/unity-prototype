using UnityEngine;

/// <summary>
/// 적 UI 표시 전담. 라벨, HP바, 데미지 팝업을 관리한다.
/// Enemy(모델)는 이벤트만 발행하고, 이 View가 표시를 담당한다.
/// </summary>
public class EnemyView : MonoBehaviour
{
    private Enemy _enemy;
    private UIFactory.HPBarHandle _hpBar;

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        if (_enemy == null || _enemy.Data == null) return;

        UIFactory.CreateLabel(transform, _enemy.Data.enemyName,
            GameConstants.EnemyUI.LabelYOffset,
            GameConstants.EnemyUI.LabelScale,
            GameConstants.Colors.EnemyLabel,
            GameConstants.SortingOrder.EnemyLabel);

        _hpBar = UIFactory.CreateHPBar(transform, GameConstants.EnemyUI.HPBarYOffset);

        _enemy.OnDamaged += HandleDamaged;
    }

    void Update()
    {
        if (_enemy == null) return;
        UIFactory.UpdateHPBar(_hpBar, _enemy.currentHP, _enemy.maxHP);
    }

    void HandleDamaged(float damage, Element element)
    {
        DamagePopup.Spawn(transform.position, damage, element);
    }

    void OnDestroy()
    {
        if (_enemy != null)
            _enemy.OnDamaged -= HandleDamaged;
    }
}
