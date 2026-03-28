using UnityEngine;

/// <summary>
/// 행성 이름 라벨 표시 전담.
/// PlanetBody(모델)에서 분리된 View.
/// </summary>
public class PlanetLabelView : MonoBehaviour
{
    void Start()
    {
        var planet = GetComponent<PlanetBody>();
        if (planet == null || planet.Planet == null) return;

        UIFactory.CreateLabel(transform, planet.Planet.bodyName,
            GameConstants.CelestialLabel.PlanetYOffset,
            GameConstants.CelestialLabel.ScaleMultiplier,
            GameConstants.Colors.PlanetLabel);
    }
}
