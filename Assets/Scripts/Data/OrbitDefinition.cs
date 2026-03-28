using UnityEngine;

/// <summary>
/// 궤도 정의. 항성이 보유하며, 행성이 배치되어 공전한다.
/// </summary>
[System.Serializable]
public class OrbitDefinition
{
    [Tooltip("궤도 반지름")]
    public float radius = 1f;

    [Tooltip("공전 속도 (도/초)")]
    public float angularSpeed = 45f;

    [Tooltip("궤도 타원 비율 (1 = 원, <1 = 납작)")]
    public float eccentricity = 1f;

    [Tooltip("궤도 시작 각도 (도)")]
    public float startAngle = 0f;

    [Tooltip("궤도 회전 방향 (true = 시계방향)")]
    public bool clockwise = false;
}
