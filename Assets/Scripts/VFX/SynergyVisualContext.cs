using UnityEngine;

/// <summary>
/// 시너지 비주얼 실행에 필요한 컨텍스트. SynergyVisualHost가 구성해서 Visual.Play에 전달.
/// Visual 구현체는 Rule의 파라미터(radius/count/secondary/spawnArea 등)를 직접 참조해 애니메이션을 데이터로 조정.
/// </summary>
public class SynergyVisualContext
{
    public SynergyRuleSO Rule;
    public SynergyContext Synergy;
    public Vector3 Anchor;
    public Color ElementColor;
}
