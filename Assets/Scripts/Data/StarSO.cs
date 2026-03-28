using UnityEngine;

/// <summary>
/// 항성(Star) = 덱. 고정 위치에 N개의 궤도를 가진다.
/// 행성을 궤도에 배치하여 천구를 구성한다.
/// </summary>
[CreateAssetMenu(fileName = "NewStar", menuName = "Celestial/Star")]
public class StarSO : CelestialBodySO
{
    [Header("항성 고유")]
    [Tooltip("이 항성이 제공하는 궤도들")]
    public OrbitDefinition[] orbits;

    [Tooltip("천구 위 고정 위치")]
    public Vector2 celestialPosition;

    [Tooltip("항성의 시각적 크기")]
    public float visualScale = 0.5f;
}
