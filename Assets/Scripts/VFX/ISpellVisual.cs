using System.Collections;
using UnityEngine;

/// <summary>
/// 마법 비주얼 전략 인터페이스. IStarEffect(로직)에 대응하는 비주얼 측.
/// 새 별의 고유 비주얼은 이 인터페이스를 구현하고 [VisualId]를 붙이면 된다.
/// </summary>
public interface ISpellVisual
{
    IEnumerator Play(SpellVisualContext ctx);
}

/// <summary>
/// 마법 비주얼 실행에 필요한 컨텍스트.
/// </summary>
public class SpellVisualContext
{
    public SpellCommand command;
    public Enemy target;
    public Vector3 targetPosition;
    public Color elementColor;
    public Transform vfxRoot;
}
