using UnityEngine;

/// <summary>
/// [Legacy] 키워드 시너지 정의. 슬래시 내 동일 키워드가 N개 이상 모이면 데미지 배율 적용.
/// Phase 3에서 <see cref="SynergyRuleSO"/> + <see cref="ISynergyEffect"/>로 마이그레이션 후 삭제 예정.
/// 새 시너지는 Synergy/Rule 메뉴를 사용하세요.
/// </summary>
[System.Obsolete("SynergyRuleSO + ISynergyEffect 기반 신규 시너지 시스템 사용. Phase 3에서 이 클래스 제거 예정.")]
[CreateAssetMenu(fileName = "LegacySynergy", menuName = "Celestial/Synergy (Legacy)")]
public class SynergyDefinitionSO : ScriptableObject
{
    public string synergyName;
    [TextArea] public string description;
    public string requiredKeyword;
    public int requiredCount = 2;

    [Header("보너스")]
    public float damageMultiplier = 1.5f;
    public Element bonusElement = Element.None;
}
