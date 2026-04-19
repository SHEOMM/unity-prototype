using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RewardScene 진입 시 3 카드 UI를 스폰하고 플레이어 선택 대기.
/// Phase 9b: null 풀 버그 수정 + 수동 선택 UI 도입.
/// 카드 클릭 → RewardManager.Apply → Map 씬으로 복귀.
/// </summary>
public class RewardSceneBoot : SceneBootBase
{
    const float CardSpacing = 3f;    // 카드 사이 가로 간격
    const float CardsY = 0f;         // 카메라 중심 y 기준 오프셋
    const float TitleY = 2.2f;

    private RewardManager _reward;
    private readonly List<RewardCardView> _cards = new List<RewardCardView>();

    protected override void OnBoot()
    {
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
        var cam = CameraService.Instance?.Camera;
        float y = (cam?.transform.position.y ?? 0) + TitleY;
        var go = new GameObject("RewardTitle");
        go.transform.position = new Vector3(0, y, 0);
        var tm = go.AddComponent<TextMesh>();
        tm.text = "보상을 선택하세요";
        tm.fontSize = 48;
        tm.characterSize = 0.1f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1f, 0.9f, 0.5f, 1f);
        var mr = go.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = 25;
    }

    void SpawnCards(List<RewardChoice> choices)
    {
        var cam = CameraService.Instance?.Camera;
        float baseX = 0f;
        float baseY = (cam?.transform.position.y ?? 0) + CardsY;

        int n = choices.Count;
        float startX = baseX - (n - 1) * 0.5f * CardSpacing;

        for (int i = 0; i < n; i++)
        {
            var go = new GameObject($"RewardCard_{i}");
            go.transform.position = new Vector3(startX + i * CardSpacing, baseY, 0);
            var card = go.AddComponent<RewardCardView>();
            card.Initialize(choices[i]);
            card.OnClicked += HandleCardClicked;
            _cards.Add(card);
        }
    }

    void HandleCardClicked(RewardCardView card)
    {
        // 중복 클릭 방지: 모든 카드 비활성화
        foreach (var c in _cards) if (c != null) c.enabled = false;
        _reward.Apply(card.Choice);
    }

    void HandleRewardChosen()
    {
        // 보상 적용 → 층 진행 + 맵 복귀
        RunState.Instance?.AdvanceFloor();
        GameManager.Instance?.EnterPhase(GamePhase.Map);
    }
}
