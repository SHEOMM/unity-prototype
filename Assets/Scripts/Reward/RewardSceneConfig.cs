using UnityEngine;

/// <summary>
/// RewardScene의 Inspector 튜닝 가능 설정.
/// RewardSceneBoot가 [SerializeField]로 참조. .asset 인스턴스는 Assets/Data/SceneConfigs/.
/// </summary>
[CreateAssetMenu(menuName = "Game/Scene Config/Reward", fileName = "RewardSceneConfig")]
public class RewardSceneConfig : SceneConfig
{
    [Header("카드 레이아웃")]
    [Tooltip("카드 사이 가로 간격 (월드 단위)")]
    public float cardSpacing = 3f;

    [Tooltip("카드 Y 오프셋 (카메라 중심 y에 더해짐)")]
    public float cardsY = 0f;

    [Tooltip("타이틀 Y 오프셋 (카메라 중심 y에 더해짐)")]
    public float titleY = 2.2f;

    [Header("타이틀 텍스트")]
    [Tooltip("화면 상단에 표시될 안내 문구")]
    public string titleText = "보상을 선택하세요";

    [Tooltip("타이틀 색상")]
    public Color titleColor = new Color(1f, 0.9f, 0.5f, 1f);

    [Tooltip("타이틀 폰트 크기 (픽셀)")]
    public int titleFontSize = 48;

    [Tooltip("타이틀 TextMesh.characterSize (월드 스케일)")]
    public float titleCharacterSize = 0.1f;
}
