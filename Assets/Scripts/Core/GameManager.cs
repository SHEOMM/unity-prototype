using UnityEngine;

/// <summary>
/// 게임 루프 오케스트레이터.
/// 천상(항성/행성) 생성 → 슬래시 입력 → 효과 해석 → 지상 전투를 연결한다.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("천체 덱")]
    [SerializeField] private StarSO[] starDeck;
    [SerializeField] private PlanetSO[] planetDeck;

    [Header("시저지")]
    [SerializeField] private SynergyDefinitionSO[] synergies;

    [Header("혜성")]
    [SerializeField] private CometSO[] cometPool;

    [Header("웨이브")]
    [SerializeField] private WaveDefinitionSO[] waveDefs;

    [Header("배치 설정")]
    [SerializeField] private float celestialYCenter = 2.5f;
    [SerializeField] private float celestialRadius = 3f;

    private DeckManager _deck;
    private SlashInput _slashInput;
    private SlashDetector _slashDetector;
    private SlashResolver _slashResolver;
    private SlashVisual _slashVisual;
    private SpellEffectManager _spellFx;
    private CometSpawner _cometSpawner;

    private Vector2 _dragStart;

    void Awake()
    {
        GetOrAdd<PlanetRegistry>();
        GetOrAdd<EnemyRegistry>();
        _deck = GetOrAdd<DeckManager>();
        _slashInput = GetOrAdd<SlashInput>();
        _slashDetector = GetOrAdd<SlashDetector>();
        _slashResolver = GetOrAdd<SlashResolver>();
        _slashVisual = GetOrAdd<SlashVisual>();
        _spellFx = GetOrAdd<SpellEffectManager>();
        var spawner = GetOrAdd<EnemySpawner>();
        if (waveDefs != null && waveDefs.Length > 0)
            spawner.waves = waveDefs;
        _cometSpawner = GetOrAdd<CometSpawner>();
    }

    void Start()
    {
        _slashResolver.synergies = synergies;
        _slashInput.celestialYMin = celestialYCenter - celestialRadius;

        if (cometPool != null && cometPool.Length > 0)
        {
            _cometSpawner.possibleComets = cometPool;
            _cometSpawner.OnCometCaptured += OnCometCaptured;
        }

        SetupCelestialBodies();

        _slashInput.OnDragStart += p => _dragStart = p;
        _slashInput.OnDragUpdate += OnDragUpdate;
        _slashInput.OnDragEnd += OnDragEnd;
    }

    void SetupCelestialBodies()
    {
        if (starDeck == null) return;
        int pi = 0;
        foreach (var sd in starDeck)
        {
            if (sd == null) continue;
            var star = _deck.CreateStar(sd);
            if (planetDeck == null || sd.orbits == null) continue;
            for (int o = 0; o < sd.orbits.Length && pi < planetDeck.Length; o++, pi++)
            {
                if (planetDeck[pi] == null) continue;
                var planet = _deck.CreatePlanet(planetDeck[pi]);
                _deck.PlacePlanetOnStar(planet, star, o);
            }
        }
    }

    void OnDragUpdate(Vector2 current)
    {
        var hits = _slashDetector.DetectHits(_dragStart, current);
        _slashVisual.ShowLine(_dragStart, current, hits.Count);
        _slashDetector.HighlightHits(_dragStart, current);
    }

    void OnDragEnd(Vector2 start, Vector2 end)
    {
        _slashVisual.HideLine();
        _slashDetector.ClearHighlights();

        // 혜성 포착 체크
        var comets = _slashDetector.DetectComets(start, end);
        foreach (var comet in comets)
            comet.Capture();

        // 행성 효과 실행
        var hits = _slashDetector.DetectHits(start, end);
        if (hits.Count == 0) return;

        var result = _slashResolver.Resolve(hits);
        _spellFx.ExecuteSpells(result);

        foreach (var syn in result.activatedSynergies)
            SynergyPopup.Show(syn.synergyName);
    }

    void OnCometCaptured(CometBody comet)
    {
        if (comet.Data.rewards == null || comet.Data.rewards.Length == 0) return;

        // 프로토타입: 첫 번째 보상을 자동 적용
        // TODO: 선택지 UI 구현 후 교체
        var reward = comet.Data.rewards[0];
        ApplyReward(reward);
        Debug.Log($"[혜성] {comet.Data.bodyName} 포착! 보상: {reward.rewardName}");
    }

    void ApplyReward(RewardOption reward)
    {
        switch (reward.type)
        {
            case CometRewardType.DamageBoost:
                // 전체 슬래시 폭 증가로 간접 버프
                _slashDetector.slashWidth += reward.value;
                break;
            case CometRewardType.ExtraSlashWidth:
                _slashDetector.slashWidth += reward.value;
                break;
        }
    }

    T GetOrAdd<T>() where T : Component
    {
        var c = GetComponent<T>();
        return c != null ? c : gameObject.AddComponent<T>();
    }
}
