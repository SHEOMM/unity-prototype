/// <summary>
/// 구조물 행동 전략. 정적(이동 없음)이며 주변에 오라/방어/공격 등 지속 효과를 제공.
/// </summary>
public interface IStructureBehavior
{
    /// <summary>매 프레임 호출. 주변 버프/투사체 생성 등의 로직.</summary>
    void OnTick(Structure structure, float deltaTime);

    /// <summary>파괴 시 호출. 잔존 효과/폭발 등.</summary>
    void OnDestroyed(Structure structure);
}
