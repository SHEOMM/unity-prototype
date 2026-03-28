using UnityEngine;

/// <summary>
/// 천상/지상 구분선 + 라벨 표시.
/// CombatManager에서 분리된 View.
/// </summary>
public class CombatDividerView : MonoBehaviour
{
    public void Initialize(float dividerY)
    {
        var lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, new Vector3(-GameConstants.DividerLineExtent, dividerY, 0));
        lr.SetPosition(1, new Vector3(GameConstants.DividerLineExtent, dividerY, 0));
        lr.startWidth = GameConstants.Orbit.PathWidth;
        lr.endWidth = GameConstants.Orbit.PathWidth;
        lr.material = GameConstants.SpriteMaterial;
        lr.startColor = GameConstants.Colors.DividerLine;
        lr.endColor = GameConstants.Colors.DividerLine;
        lr.sortingOrder = GameConstants.SortingOrder.Background;

        UIFactory.CreateLabel(transform, "천상", 0.3f, 0.4f,
            GameConstants.Colors.SkyLabel, GameConstants.SortingOrder.Background);
        UIFactory.CreateLabel(transform, "지상", -0.3f, 0.4f,
            GameConstants.Colors.GroundLabel, GameConstants.SortingOrder.Background);
    }
}
