using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RewardScene 진입 시 3 카드 UI를 스폰하고 플레이어 선택 대기.
/// 카드 클릭 → RewardManager.Apply → Map 씬으로 복귀.
///
/// <para>Prefab 마이그레이션: 카드 레이아웃은 <c>rewardCardPrefab</c>이 제공하고
/// 본 Boot는 위치 배치 + 데이터 바인딩만 담당. 튜닝 값은 <c>config</c> SO에서 가져옴.</para>
/// </summary>
public class RewardSceneBoot : SceneBootBase
{
    [Header("Prefab 참조")]
    [Tooltip("RewardCard Prefab — Assets/Prefabs/UI/RewardCard.prefab")]
    [SerializeField] GameObject rewardCardPrefab;

    [Header("Scene 설정")]
    [Tooltip("카드 간격·타이틀 등 디자이너 튜닝 값. Assets/Data/SceneConfigs/")]
    [SerializeField] RewardSceneConfig config;

    private RewardManager _reward;
    private readonly List<RewardCardView> _cards = new List<RewardCardView>();

    protected override void OnBoot()
    {
        EnsureHud();
        var gm = GameManager.Instance;
        _reward = gameObject.AddComponent<RewardManager>();
        _reward.OnRewardChosen += HandleRewardChosen;

        var choices = _reward.BuildChoices(
            gm?.RewardOrbitPool,
            gm?.RewardPlanetPool,
            gm?.RewardRelicPool,
            count: 3);

        if (choices == null || choices.Count == 0)
        {
            Debug.Log("[RewardSceneBoot] 선택지 없음 (풀 비었거나 모두 보유). 스킵.");
            _reward.Skip();
            return;
        }

        SpawnTitle();
        SpawnCards(choices);
    }

    void SpawnTitle()
    {
        if (config == null) return;
        var cam = CameraService.Instance?.Camera;
        float y = (cam?.transform.position.y ?? 0) + config.titleY;
        var go = new GameObject("RewardTitle");
        go.transform.position = new Vector3(0, y, 0);
        var tm = go.AddComponent<TextMesh>();
        tm.text = config.titleText;
        tm.fontSize = config.titleFontSize;
        tm.characterSize = config.titleCharacterSize;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = config.titleColor;
        var mr = go.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = GameConstants.SortingOrder.RewardCardTitle;
    }

    void SpawnCards(List<RewardChoice> choices)
    {
        var cam = CameraService.Instance?.Camera;
        float baseY = (cam?.transform.position.y ?? 0) + (config != null ? config.cardsY : 0f);
        float spacing = config != null ? config.cardSpacing : 3f;

        int n = choices.Count;
        float startX = -(n - 1) * 0.5f * spacing;

        for (int i = 0; i < n; i++)
        {
            var card = PrefabHelper.Spawn<RewardCardView>(rewardCardPrefab);
            if (card == null) continue;
            card.gameObject.name = $"RewardCard_{i}";
            card.transform.position = new Vector3(startX + i * spacing, baseY, 0);
            card.Bind(choices[i]);
            card.OnClicked += HandleCardClicked;
            _cards.Add(card);
        }
    }

    void HandleCardClicked(RewardCardView card)
    {
        // 중복 클릭 방지: 모든 카드 클릭 비활성화
        foreach (var c in _cards) if (c != null) c.SetClickable(false);
        _reward.Apply(card.Choice);
    }

    void HandleRewardChosen()
    {
        // 보상 적용 → 층 진행 + 맵 복귀
        RunState.Instance?.AdvanceFloor();
        GameManager.Instance?.EnterPhase(GamePhase.Map);
    }

    void OnValidate()
    {
        if (rewardCardPrefab == null)
            Debug.LogWarning("[RewardSceneBoot] rewardCardPrefab not assigned — Inspector에서 할당 필요", this);
        if (config == null)
            Debug.LogWarning("[RewardSceneBoot] config not assigned — Inspector에서 할당 필요", this);
    }
}
