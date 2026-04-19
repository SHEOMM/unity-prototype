using UnityEngine;

/// <summary>
/// 전투 루프 담당. 천체 배치, 웨이브, 혜성을 관리한다.
/// 우주선 발사 입력 처리는 ShipController에 위임.
/// </summary>
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public SynergyDefinitionSO[] synergies;
    public SynergyRuleSO[] synergyRules;
    public CometSO[] cometPool;
    public float celestialYCenter = 2.5f;
    public float celestialRadius = 3f;

    private DeckManager _deck;
    private ShipController _shipController;
    private EnemySpawner _spawner;
    private CometSpawner _cometSpawner;

    public System.Action OnCombatComplete;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void Initialize()
    {
        _deck = GetOrAdd<DeckManager>();
        _spawner = GetOrAdd<EnemySpawner>();
        _cometSpawner = GetOrAdd<CometSpawner>();
        GetOrAdd<GravitySourceRegistry>();
        GetOrAdd<EnemyRegistry>();

        var input = GetOrAdd<ShipInput>();
        var resolver = GetOrAdd<SlashResolver>();
        var visual = GetOrAdd<ShipVisual>();
        var spellFx = GetOrAdd<SpellEffectManager>();

        resolver.synergies = synergies;
        input.celestialYMin = celestialYCenter - celestialRadius;

        _shipController = GetOrAdd<ShipController>();
        _shipController.Initialize(input, resolver, spellFx, visual);
        _shipController.OnShipComplete += result => PlayerState.Instance?.NotifySlashPerformed(result);

        // 신규 시너지 시스템 (Phase 0) — 기존 synergies 배열은 SlashResolver가 계속 사용 (레거시)
        var synergyDispatcher = GetOrAdd<SynergyDispatcher>();
        synergyDispatcher.SetRules(synergyRules);
        synergyDispatcher.Bind(_shipController);

        GetOrAdd<ShipFeedbackView>();
    }

    public void StartCombat(WaveDefinitionSO[] waves, StarSO[] stars = null, PlanetSO[] planets = null)
    {
        if (stars != null && planets != null)
            SetupCelestialBodies(stars, planets);

        var divider = new GameObject("DividerLine").AddComponent<CombatDividerView>();
        divider.Initialize(celestialYCenter - celestialRadius);

        if (cometPool != null && cometPool.Length > 0)
        {
            _cometSpawner.possibleComets = cometPool;
            _cometSpawner.OnCometCaptured += OnCometCaptured;
        }

        _spawner.waves = waves;
        _spawner.OnWaveStart += i => PlayerState.Instance?.NotifyWaveStart(i);
        _spawner.OnWaveComplete += i => PlayerState.Instance?.NotifyWaveComplete(i);
        _spawner.OnAllWavesComplete += OnAllWavesCleared;
        _spawner.StartWaves();

        _shipController.Activate();
    }

    public void EndCombat()
    {
        _shipController.Deactivate();
        _cometSpawner.OnCometCaptured -= OnCometCaptured;
        _spawner.OnAllWavesComplete -= OnAllWavesCleared;
        _deck.ClearAll();
    }

    void SetupCelestialBodies(StarSO[] starDeck, PlanetSO[] planetDeck)
    {
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

    void OnAllWavesCleared()
    {
        EndCombat();
        OnCombatComplete?.Invoke();
    }

    void OnCometCaptured(CometBody comet)
    {
        if (comet.Data.rewards == null || comet.Data.rewards.Length == 0) return;
        var reward = comet.Data.rewards[0];
        RewardManager.Instance?.ApplyReward(reward);
        Debug.Log($"[혜성] {comet.Data.bodyName} 포착! 보상: {reward.rewardName}");
    }

    T GetOrAdd<T>() where T : Component
    {
        var c = GetComponent<T>();
        return c != null ? c : gameObject.AddComponent<T>();
    }
}
