using UnityEngine;

/// <summary>
/// 항성 이름 라벨 표시 전담. StarSystem(모델)에서 분리된 View.
/// </summary>
public class StarLabelView : MonoBehaviour
{
    void Start()
    {
        var star = GetComponent<StarSystem>();
        if (star == null || star.Data == null) return;

        UIFactory.CreateLabel(transform, star.Data.bodyName,
            GameConstants.CelestialLabel.StarYOffset,
            GameConstants.CelestialLabel.ScaleMultiplier,
            GameConstants.Colors.StarLabel);
    }
}
