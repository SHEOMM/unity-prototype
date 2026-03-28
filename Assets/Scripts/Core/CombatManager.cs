using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 전투 루프 담당. GameManager에서 추출.
/// 슬래시 입력 → 판정 → 효과 → 웨이브 → 혜성을 관리한다.
/// </summary>
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public SynergyDefinitionSO[] synergies;
    public CometSO[] cometPool;
    public float celestialYCenter = 2.5f;
    public float celestialRadius = 3f;

    private DeckManager _deck;
    private SlashInput _slashInput;
    private SlashDetector _slashDetector;
    private SlashResolver _slashResolver;
    private SlashVisual _slashVisual;
    private SpellEffectManager _spellFx;
    private EnemySpawner _spawner;
    private CometSpawner _cometSpawner;

    private Vector2 _dragStart;
    private bool _combatActive;

    public System.Action OnCombatComplete;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void Initialize()
    {
        _deck = GetOrAdd<DeckManager>();
        _slashInput = GetOrAdd<SlashInput>();
        _slashDetector = GetOrAdd<SlashDetector>();
        _slashResolver = GetOrAdd<SlashResolver>();
        _slashVisual = GetOrAdd<SlashVisual>();
        _spellFx = GetOrAdd<SpellEffectManager>();
        _spawner = GetOrAdd<EnemySpawner>();
        _cometSpawner = GetOrAdd<CometSpawner>();

        _slashResolver.synergies = synergies;
        _slashInput.celestialYMin = celestialYCenter - celestialRadius;
    }

    public void StartCombat(WaveDefinitionSO[] waves, StarSO[] stars = null, PlanetSO[] planets = null)
    {
        _combatActive = true;

        if (stars != null && planets != null)
            SetupCelestialBodies(stars, planets);

        // 혜성
        if (cometPool != null && cometPool.Length > 0)
        {
            _cometSpawner.possibleComets = cometPool;
            _cometSpawner.OnCometCaptured += OnCometCaptured;
        }

        // 웨이브
        _spawner.waves = waves;
        _spawner.OnWaveStart += i => PlayerState.Instance?.NotifyWaveStart(i);
        _spawner.OnWaveComplete += i => PlayerState.Instance?.NotifyWaveComplete(i);
        _spawner.OnAllWavesComplete += OnAllWavesCleared;
        _spawner.StartWaves();

        // 입력
        _slashInput.OnDragStart += OnDragStart;
        _slashInput.OnDragUpdate += OnDragUpdate;
        _slashInput.OnDragEnd += OnDragEnd;
    }

    public void EndCombat()
    {
        _combatActive = false;
        _slashInput.OnDragStart -= OnDragStart;
        _slashInput.OnDragUpdate -= OnDragUpdate;
        _slashInput.OnDragEnd -= OnDragEnd;
        _cometSpawner.OnCometCaptured -= OnCometCaptured;
        _spawner.OnAllWavesComplete -= OnAllWavesCleared;
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

    void OnDragStart(Vector2 pos) { _dragStart = pos; }

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

        var comets = _slashDetector.DetectComets(start, end);
        foreach (var comet in comets)
            comet.Capture();

        var hits = _slashDetector.DetectHits(start, end);
        if (hits.Count == 0) return;

        var result = _slashResolver.Resolve(hits);
        _spellFx.ExecuteSpells(result);

        foreach (var syn in result.activatedSynergies)
            SynergyPopup.Show(syn.synergyName);

        PlayerState.Instance?.NotifySlashPerformed(result);
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
