using UnityEngine;

/// <summary>
/// 씬별 카메라/조명/배경 설정을 담는 데이터. PersistentScene의 단일 Camera/Light에
/// 씬 진입 시 SceneBootBase가 이 값을 적용한다. Inspector에서 편집 가능.
/// </summary>
[CreateAssetMenu(fileName = "SceneEnvironment", menuName = "Core/Scene Environment")]
public class SceneEnvironment : ScriptableObject
{
    [Header("카메라")]
    [Tooltip("카메라 월드 위치 (2D: z=-10 권장)")]
    public Vector3 cameraPosition = new Vector3(0f, 0f, -10f);

    [Tooltip("직교 카메라 반(半)높이")]
    public float orthographicSize = 6f;

    [Tooltip("배경 색상 (ClearFlags = SolidColor)")]
    public Color backgroundColor = new Color(0.05f, 0.05f, 0.10f, 1f);

    [Header("조명")]
    [Tooltip("Global Light 2D 강도 (0~2)")]
    public float lightIntensity = 1f;

    [Tooltip("Global Light 2D 색상")]
    public Color lightColor = Color.white;
}
