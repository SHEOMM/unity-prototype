using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전투 후 보상 3 선택지 구성 + 적용 관리.
/// Phase 9b: 자동 선택 프로토타입 제거, RewardSceneBoot가 카드 UI를 띄우는 구조로 전환.
///
/// 역할:
///  - BuildChoices: GameManager의 풀에서 중복 제외 후 3개 선택지 구성
///  - Apply: 플레이어가 카드 클릭 시 payload.ApplyAsReward 호출 + OnRewardChosen
///  - ApplyReward(RewardOption): 혜성 보상 응용 (기존 파이프라인 유지)
/// </summary>
public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }

    /// <summary>보상 적용이 끝났을 때 (스킵 포함). Boot가 구독 → EnterPhase(Map).</summary>
    public System.Action OnRewardChosen;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    /// <summary>
    /// 3 카테고리 풀에서 보유하지 않은 것을 모아 무작위 `count`개 선택지 생성.
    /// 풀이 합쳐서 count보다 적으면 가능한 만큼만 반환.
    /// </summary>
    public List<RewardChoice> BuildChoices(OrbitSO[] orbitPool, PlanetSO[] planetPool, RelicSO[] relicPool, int count = 3)
    {
        var available = new List<RewardChoice>();
        var run = RunState.Instance;
        var player = PlayerState.Instance;

        if (orbitPool != null)
            foreach (var o in orbitPool)
                if (o != null && (run == null || !run.unlockedOrbits.Contains(o)))
                    available.Add(MakeOrbitChoice(o));

        if (planetPool != null)
            foreach (var p in planetPool)
                if (p != null && (run == null || !run.planetDeck.Contains(p)))
                    available.Add(MakePlanetChoice(p));

        if (relicPool != null)
            foreach (var r in relicPool)
                if (r != null && (player == null || !player.HasRelic(r)))
                    available.Add(MakeRelicChoice(r));

        // Fisher-Yates 셔플 후 앞에서 count개
        for (int i = available.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (available[i], available[j]) = (available[j], available[i]);
        }
        int take = Mathf.Min(count, available.Count);
        return available.GetRange(0, take);
    }

    /// <summary>선택된 보상 적용 → OnRewardChosen 이벤트 발행.</summary>
    public void Apply(RewardChoice choice)
    {
        if (choice?.Payload != null)
        {
            choice.Payload.ApplyAsReward(PlayerState.Instance, RunState.Instance);
            Debug.Log($"[보상] '{choice.DisplayName}' ({choice.TypeLabel}) 획득");
        }
        else
        {
            Debug.Log("[보상] 선택 없음 — 스킵");
        }
        OnRewardChosen?.Invoke();
    }

    /// <summary>선택지가 비어있을 때(풀 고갈) 스킵.</summary>
    public void Skip()
    {
        Debug.Log("[보상] 풀 고갈 — 스킵");
        OnRewardChosen?.Invoke();
    }

    // ── 혜성 즉시 보상 (기존 경로 유지) ────────────────────────────
    public void ApplyReward(RewardOption reward)
    {
        switch (reward.type)
        {
            case CometRewardType.DamageBoost:
                if (PlayerState.Instance != null)
                    PlayerState.Instance.bonusDamageMultiplier += reward.value;
                break;
            case CometRewardType.ExtraSlashWidth:
                var detector = GetComponent<SlashDetector>();
                if (detector != null) detector.slashWidth += reward.value;
                break;
            case CometRewardType.HealPlayer:
                PlayerState.Instance?.Heal(reward.value);
                break;
            case CometRewardType.GrantRelic:
                if (reward.relicSO != null)
                    PlayerState.Instance?.AddRelic(reward.relicSO);
                break;
        }
    }

    // ── Choice 팩토리 ─────────────────────────────────────────────

    static readonly Color OrbitColor  = new Color(0.55f, 0.9f, 1f, 1f);   // 시안
    static readonly Color PlanetColor = new Color(1f, 0.85f, 0.4f, 1f);   // 금빛
    static readonly Color RelicColor  = new Color(0.9f, 0.55f, 1f, 1f);   // 보라

    RewardChoice MakeOrbitChoice(OrbitSO o) => new RewardChoice
    {
        Payload = o,
        DisplayName = string.IsNullOrEmpty(o.orbitName) ? o.name : o.orbitName,
        Description = o.description,
        Icon = null,   // 궤도는 별도 렌더 (반경 원 그리기) — 카드 측에서 처리
        TypeLabel = "궤도",
        TypeColor = OrbitColor,
    };

    RewardChoice MakePlanetChoice(PlanetSO p) => new RewardChoice
    {
        Payload = p,
        DisplayName = string.IsNullOrEmpty(p.bodyName) ? p.name : p.bodyName,
        Description = p.description,
        Icon = PlanetSpriteResolver.Resolve(p),
        TypeLabel = "행성",
        TypeColor = PlanetColor,
    };

    RewardChoice MakeRelicChoice(RelicSO r) => new RewardChoice
    {
        Payload = r,
        DisplayName = string.IsNullOrEmpty(r.relicName) ? r.name : r.relicName,
        Description = r.description,
        Icon = r.icon,
        TypeLabel = "유물",
        TypeColor = RelicColor,
    };
}
