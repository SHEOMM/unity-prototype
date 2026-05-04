using UnityEngine;

/// <summary>
/// 전투 루프 담당. 천체 배치, 웨이브, 혜성을 관리한다.
/// 우주선 발사 입력 처리는 ShipController에 위임.
/// </summary>
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public SynergyRuleSO[] synergyRules;
    public CometSO[] cometPool;
    [Tooltip("발사 원점 Y 좌표 (= 지상/천상 경계선). 천체 배치와 독립.")]
    public float launchOriginY = -0.5f;
    [Tooltip("천체 클러스터 중심 Y. launchOriginY보다 높게 설정.")]
    public float celestialYCenter = 5f;
    [Tooltip("천체 클러스터 기준 X = 발사 원점 X + 이 오프셋.")]
    public float celestialCenterXOffset = -3f;
    [Tooltip("궤도 간 수평 간격 (각 궤도가 기준점에서 이 거리씩 퍼짐).")]
    public float celestialSpreadX = 2.5f;
    [Tooltip("궤도 간 수직 간격 (홀짝 인덱스 교대로 ±이 값만큼 오프셋).")]
    public float celestialSpreadY = 1.5f;
    public float celestialRadius = 3f;

    private float _launchOriginX;
    private DeckManager _deck;
    private ShipController _shipController;
    private EnemySpawner _spawner;
    private CometSpawner _cometSpawner;
    private SynergyDispatcher _synergyDispatcher;

    /// <summary>UI/VFX 레이어가 OnSynergyFired 이벤트를 구독할 수 있도록 노출.</summary>
    public SynergyDispatcher SynergyDispatcher => _synergyDispatcher;

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
        GetOrAdd<AllyRegistry>();
        GetOrAdd<StructureRegistry>();

        var cam = CameraService.Instance?.Camera;
        _launchOriginX = cam != null
            ? cam.transform.position.x - cam.orthographicSize * cam.aspect + 1f
            : 0f;

        var input = GetOrAdd<ShipInput>();
        var resolver = GetOrAdd<SpellResolver>();
        var visual = GetOrAdd<ShipVisual>();
        var spellFx = GetOrAdd<SpellEffectManager>();

        input.launchOriginX = _launchOriginX;
        input.celestialYMin = launchOriginY;

        _shipController = GetOrAdd<ShipController>();
        _shipController.Initialize(input, resolver, spellFx, visual);
        _shipController.OnShipComplete += result => PlayerState.Instance?.NotifySpellPerformed(result);

        _synergyDispatcher = GetOrAdd<SynergyDispatcher>();
        _synergyDispatcher.SetRules(synergyRules);
        _synergyDispatcher.Bind(_shipController);
    }

    public void StartCombat(WaveDefinitionSO[] waves,
                            System.Collections.Generic.List<OrbitSO> orbits = null,
                            System.Collections.Generic.List<OrbitAssignment> assignments = null,
                            System.Collections.Generic.List<PlanetSO> planetDeck = null,
                            float spawnCountMultiplier = 1f)
    {
        if (orbits != null && orbits.Count > 0)
            SetupCosmos(orbits, assignments, planetDeck);

        var divider = new GameObject("DividerLine").AddComponent<CombatDividerView>();
        divider.Initialize(launchOriginY);

        if (cometPool != null && cometPool.Length > 0)
        {
            _cometSpawner.possibleComets = cometPool;
            _cometSpawner.OnCometCaptured += OnCometCaptured;
        }

        _spawner.waves = waves;
        _spawner.spawnCountMultiplier = spawnCountMultiplier;
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

        // 아군/구조물 전투 종료 시 정리 (Phase 2 플랜: 전투 내내 유지, combat end에 파괴)
        AllyRegistry.Instance?.DestroyAll();
        StructureRegistry.Instance?.DestroyAll();
    }

    /// <summary>
    /// Phase 9: 궤도들을 월드 원점의 y=celestialYCenter 동심원으로 생성.
    /// assignments에 매핑된 행성만 해당 궤도에 배치 — 매핑 없는 궤도는 빈 상태로 회전.
    /// </summary>
    void SetupCosmos(System.Collections.Generic.List<OrbitSO> orbits,
                     System.Collections.Generic.List<OrbitAssignment> assignments,
                     System.Collections.Generic.List<PlanetSO> planetDeck)
    {
        Vector2 baseCenter = new Vector2(_launchOriginX + celestialCenterXOffset, celestialYCenter);

        int total = 0;
        foreach (var o in orbits) if (o != null) total++;

        int idx = 0;
        foreach (var orbitSo in orbits)
        {
            if (orbitSo == null) continue;
            float xOff = total > 1 ? (idx - (total - 1) * 0.5f) * celestialSpreadX : 0f;
            float yOff = (idx % 2 == 0 ? 1f : -1f) * celestialSpreadY * 0.5f;
            var orbitBody = _deck.CreateOrbit(orbitSo, baseCenter + new Vector2(xOff, yOff));

            string planetName = FindAssignedPlanet(assignments, orbitSo.orbitName);
            if (!string.IsNullOrEmpty(planetName))
            {
                var planetSo = FindPlanetByName(planetDeck, planetName);
                if (planetSo != null)
                {
                    var planetBody = _deck.CreatePlanet(planetSo);
                    orbitBody.PlacePlanet(planetBody);
                }
            }
            idx++;
        }
    }

    static string FindAssignedPlanet(System.Collections.Generic.List<OrbitAssignment> assignments, string orbitName)
    {
        if (assignments == null) return null;
        foreach (var a in assignments)
            if (a.orbitName == orbitName) return a.planetName;
        return null;
    }

    static PlanetSO FindPlanetByName(System.Collections.Generic.List<PlanetSO> deck, string planetName)
    {
        if (deck == null) return null;
        foreach (var p in deck)
            if (p != null && p.bodyName == planetName) return p;
        return null;
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
