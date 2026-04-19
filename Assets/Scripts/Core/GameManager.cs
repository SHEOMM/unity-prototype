using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// 게임 상태 머신 오케스트레이터.
/// PersistentScene에 존재하며 DontDestroyOnLoad로 영속.
/// SceneLoader로 MapScene/CombatScene/ShopScene 등을 Additive 로드/언로드.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("초기 덱")]
    [SerializeField] private PlanetSO[] startingPlanets;
    [SerializeField] private OrbitSO[] startingOrbits;
    [Tooltip("시작 시 궤도↔행성 배치. 길이·순서는 startingOrbits와 독립적.")]
    [SerializeField] private OrbitAssignment[] defaultAssignments;

    [Header("시너지")]
    [SerializeField] private SynergyDefinitionSO[] synergies;
    [Tooltip("Phase 3+ 신규 시너지 규칙. SynergyDispatcher가 구독.")]
    [SerializeField] private SynergyRuleSO[] synergyRules;

    [Header("혜성")]
    [SerializeField] private CometSO[] cometPool;

    [Header("맵 설정")]
    [SerializeField] private int mapFloors = 10;
    [SerializeField] private int mapColumns = 5;
    [SerializeField] private int mapPaths = 4;

    public GamePhase CurrentPhase { get; private set; }
    public System.Action<GamePhase, GamePhase> OnPhaseChanged;

    // 영속 매니저 (DontDestroyOnLoad)
    private MapManager _map;
    private SceneLoader _sceneLoader;

    private MapNode _currentNode;

    // 외부에서 접근 가능한 설정/상태
    public SynergyDefinitionSO[] Synergies => synergies;
    public SynergyRuleSO[] SynergyRules => synergyRules;
    public CometSO[] CometPool => cometPool;
    public MapNode CurrentNode => _currentNode;
    public WaveDefinitionSO[] DefaultWaves => defaultWaves;
    public PlanetSO[] StartingPlanets => startingPlanets;
    public OrbitSO[] StartingOrbits => startingOrbits;
    public OrbitAssignment[] DefaultAssignments => defaultAssignments;

    [Header("기본 웨이브")]
    [SerializeField] private WaveDefinitionSO[] defaultWaves;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 영속 싱글턴들
        GetOrAdd<PlayerState>();
        GetOrAdd<RunState>();
        _sceneLoader = GetOrAdd<SceneLoader>();
        _map = GetOrAdd<MapManager>();

        _map.OnNodeSelected += OnNodeSelected;
    }

    void Start()
    {
        StartRun();
    }

    public void StartRun()
    {
        RunState.Instance.InitializeRun(
            Random.Range(0, 99999),
            startingPlanets ?? new PlanetSO[0],
            startingOrbits ?? new OrbitSO[0],
            defaultAssignments ?? new OrbitAssignment[0]
        );
        PlayerState.Instance.ResetForNewRun();

        _map.GenerateMap(mapFloors, mapColumns, mapPaths, RunState.Instance.runSeed);

        EnterPhase(GamePhase.Map);
    }

    public void EnterPhase(GamePhase phase)
    {
        var old = CurrentPhase;
        CurrentPhase = phase;
        OnPhaseChanged?.Invoke(old, phase);

        Debug.Log($"[GameManager] Phase: {old} → {phase}");

        switch (phase)
        {
            case GamePhase.Map:
                if (_map.HasNextNodes())
                    _sceneLoader.LoadScene("MapScene");
                else
                    EnterPhase(GamePhase.Victory);
                break;

            case GamePhase.Combat:
                _sceneLoader.LoadScene("CombatScene");
                break;

            case GamePhase.Reward:
                _sceneLoader.LoadScene("RewardScene");
                break;

            case GamePhase.Rest:
                _sceneLoader.LoadScene("RestScene");
                break;

            case GamePhase.Shop:
                _sceneLoader.LoadScene("ShopScene");
                break;

            case GamePhase.GameOver:
                Debug.Log("[게임 오버]");
                Time.timeScale = 0f;
                break;

            case GamePhase.Victory:
                Debug.Log("[승리!] 모든 방을 클리어했습니다.");
                break;
        }
    }

    void OnNodeSelected(MapNode node)
    {
        _currentNode = node;
        Debug.Log($"[GameManager] 노드 선택: {node.roomType} (층 {node.floor})");

        switch (node.roomType)
        {
            case RoomType.Combat:
            case RoomType.Elite:
            case RoomType.Boss:
                EnterPhase(GamePhase.Combat);
                break;
            case RoomType.Rest:
                EnterPhase(GamePhase.Rest);
                break;
            case RoomType.Shop:
                EnterPhase(GamePhase.Shop);
                break;
            default:
                EnterPhase(GamePhase.Combat);
                break;
        }
    }

    T GetOrAdd<T>() where T : Component
    {
        var c = GetComponent<T>();
        return c != null ? c : gameObject.AddComponent<T>();
    }
}
