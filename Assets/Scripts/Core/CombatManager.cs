using UnityEngine;

/// <summary>
/// 전투 루프 담당. 천체 배치, 웨이브, 혜성을 관리한다.
/// 원형 스코프 입력 처리는 ScopeController에 위임.
/// </summary>
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public SynergyDefinitionSO[] synergies;
    public CometSO[] cometPool;
    public float celestialYCenter = 2.5f;
    public float celestialRadius = 3f;

    private DeckManager _deck;
    private ScopeController _scopeController;
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

        var input = GetOrAdd<ScopeInput>();
        var detector = GetOrAdd<ScopeDetector>();
        var resolver = GetOrAdd<SlashResolver>();
        var visual = GetOrAdd<ScopeVisual>();
        var spellFx = GetOrAdd<SpellEffectManager>();

        resolver.synergies = synergies;
        input.celestialYMin = celestialYCenter - celestialRadius;

        _scopeController = GetOrAdd<ScopeController>();
        _scopeController.Initialize(input, detector, resolver, visual, spellFx);
        _scopeController.OnScopeComplete += result => PlayerState.Instance?.NotifySlashPerformed(result);

        GetOrAdd<ScopeFeedbackView>();
    }

    public void StartCombat(WaveDefinitionSO[] waves, PlanetSO[] planets = null)
    {
        if (planets != null)
            SetupCelestialBodies(planets);

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

        _scopeController.Activate();
    }

    public void EndCombat()
    {
        _scopeController.Deactivate();
        _cometSpawner.OnCometCaptured -= OnCometCaptured;
        _spawner.OnAllWavesComplete -= OnAllWavesCleared;
    }

    void SetupCelestialBodies(PlanetSO[] planetDeck)
    {
        foreach (var data in planetDeck)
        {
            if (data == null) continue;
            _deck.CreateCelestialBody(data);
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
