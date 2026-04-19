using UnityEngine;

/// <summary>
/// 시너지 규칙 에셋. "언제 무엇을 발동할지"를 데이터로 선언.
///
/// 필드는 triggerType에 따라 다르게 해석됨:
/// - FamilyAccumulation: family, threshold 사용
/// - SequencePosition: positionKey, planetKey 사용 (Phase 4~)
/// - PlanetCombo: sequenceKeys 사용 (Phase 5~)
///
/// synergyEffectId는 SynergyRegistry에서 실제 실행 로직을 찾는 키.
/// </summary>
[CreateAssetMenu(fileName = "NewSynergyRule", menuName = "Synergy/Rule")]
public class SynergyRuleSO : ScriptableObject
{
    [Tooltip("발동할 ISynergyEffect의 [SynergyId]. SynergyRegistry에서 해당 ID 구현체를 찾아 실행.")]
    public string synergyEffectId;

    [Header("발동 조건")]
    public SynergyTriggerType triggerType = SynergyTriggerType.FamilyAccumulation;

    [Header("FamilyAccumulation 모드")]
    public SynergyFamily family = SynergyFamily.Fire;

    [Tooltip("이 family가 N회 이상 누적되면 발동.")]
    public int threshold = 1;

    [Header("SequencePosition 모드 (Phase 4+)")]
    [Tooltip("Leading=맨 앞, Trailing=맨 뒤, Any=어디든")]
    public SequencePosition positionKey = SequencePosition.Any;

    [Tooltip("해당 위치에 와야 하는 행성의 bodyName 또는 keyword.")]
    public string planetKey;

    [Header("PlanetCombo 모드 (Phase 5+)")]
    [Tooltip("시퀀스에 모두 포함되어야 하는 bodyName/keyword 리스트.")]
    public string[] sequenceKeys;

    [Header("스폰 영역 (AllySpawner/StructureSpawner 용)")]
    [Tooltip("이 rule이 유닛/구조물을 소환할 때 사용할 랜덤 영역. (x, y, width, height) 월드 좌표.")]
    public Rect spawnArea = new Rect(-3f, -2f, 6f, 1f);

    [Tooltip("한 번 발동 시 소환할 개수.")]
    public int spawnCount = 1;

    [Header("스폰 대상 (Ally/Structure 시너지 전용)")]
    [Tooltip("War 계열이 소환할 아군 SO.")]
    public AllySO allyToSpawn;

    [Tooltip("Earth_3/Civilization 계열이 설치할 구조물 SO.")]
    public StructureSO structureToSpawn;

    [Header("효과 파라미터 (시너지별 해석)")]
    [Tooltip("기본 데미지 (AoE/단일/Chain/Sweep 등).")]
    public float damage = 10f;

    [Tooltip("범위 (AoE 반경 / Chain 점프 반경 / Sweep 폭).")]
    public float radius = 2f;

    [Tooltip("지속시간 (DoT/Slow/Buff/Debuff).")]
    public float duration = 3f;

    [Tooltip("보조 값 — 시너지별 해석: DoT tickInterval / Slow factor / Knockback 강도 / Execute hpRatio / Buff multiplier 등.")]
    public float secondary = 0.5f;

    [Tooltip("카운트 — Chain 점프 수 / DoT tick 수 / Ally 소환 수 / 임계 카운트 등.")]
    public int count = 1;

    [Tooltip("피해 속성. 기본은 family에 대응.")]
    public Element element = Element.None;

    [Header("메타")]
    public string displayName;
    [TextArea] public string description;
}

/// <summary>시퀀스 내 위치 지정자.</summary>
public enum SequencePosition
{
    Any,
    Leading,
    Trailing
}
