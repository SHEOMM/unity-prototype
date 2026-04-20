using UnityEngine;

/// <summary>
/// 한 캐릭터의 상태별 애니메이션 클립 묶음. 상태 머신이 Get(state)로 조회.
/// 미정의 상태는 null 반환 → 호출자가 fallback 결정 (예: Death 없으면 Idle 유지).
///
/// 파이프라인: CharacterAnimationGenerator가 Assets/art/character/{name}/*.png 순회해 자동 구성.
/// </summary>
[CreateAssetMenu(fileName = "NewCharacterSet", menuName = "Data/Character Animation Set")]
public class CharacterAnimationSet : ScriptableObject
{
    [Tooltip("캐릭터 식별 이름 (예: \"B_witch\").")]
    public string characterName;

    [Header("상태별 클립")]
    public CharacterAnimationClip idle;
    public CharacterAnimationClip run;
    public CharacterAnimationClip charge;
    public CharacterAnimationClip attack;
    public CharacterAnimationClip takeDamage;
    public CharacterAnimationClip death;

    public CharacterAnimationClip Get(CharacterAnimationState state)
    {
        switch (state)
        {
            case CharacterAnimationState.Idle:       return idle;
            case CharacterAnimationState.Run:        return run;
            case CharacterAnimationState.Charge:     return charge;
            case CharacterAnimationState.Attack:     return attack;
            case CharacterAnimationState.TakeDamage: return takeDamage;
            case CharacterAnimationState.Death:      return death;
        }
        return null;
    }
}
