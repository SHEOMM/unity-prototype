/// <summary>
/// 캐릭터(플레이어·아군·적) 애니메이션 상태. CharacterAnimationSet이 각 상태별 clip을 보관.
/// PlayerCharacterView 등 상태 컨트롤러가 현재 상태를 계산해 Animator에 전달.
/// </summary>
public enum CharacterAnimationState
{
    Idle,
    Run,
    Charge,
    Attack,
    TakeDamage,
    Death,
}
