using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 게임 상태 머신 오케스트레이터.
/// IRoomHandler를 통해 방 타입별로 씬/로직을 위임한다.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("초기 덱")]
    [SerializeField] private StarSO[] startingStars;
    [SerializeField] private PlanetSO[] startingPlanets;

    [Header("시저지")]
    [SerializeField] private SynergyDefinitionSO[] synergies;

    [Header("혜성")]
    [SerializeField] private CometSO[] cometPool;

    [Header("막 정의")]
    [SerializeField] private ActDefinitionSO currentAct;

    [Header("맵 설정")]
    [SerializeField] private int mapFloors = 10;
    [SerializeField] private int mapColumns = 5;
    [SerializeField] private int mapPaths = 4;

    public GamePhase CurrentPhase { get; private set; }
    public System.Action<GamePhase, GamePhase> OnPhaseChanged;

    private CombatManager _combat;
    private RewardManager _reward;
    private MapManager _map;
    private MapNode _currentNode;

    private Dictionary<RoomType, IRoomHandler> _roomHandlers;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        GetOrAdd<PlanetRegistry>();
        GetOrAdd<EnemyRegistry>();
        GetOrAdd<PlayerState>();
        GetOrAdd<PlayerHPBar>();
        GetOrAdd<PlayerDamageView>();
        GetOrAdd<RunState>();
        GetOrAdd<SceneLoader>();

        _combat = GetOrAdd<CombatManager>();
        _combat.synergies = synergies;
        _combat.cometPool = cometPool;
        _combat.Initialize();
        _reward = GetOrAdd<RewardManager>();
        _map = GetOrAdd<MapManager>();

        _combat.OnCombatComplete += () => EnterPhase(GamePhase.Reward);
        _reward.OnRewardChosen += () => EnterPhase(GamePhase.Map);
        _map.OnNodeSelected += OnNodeSelected;

        RegisterRoomHandlers();
    }

    void RegisterRoomHandlers()
    {
        _roomHandlers = new Dictionary<RoomType, IRoomHandler>();
        var handlers = new IRoomHandler[]
        {
            new CombatRoomHandler(),
            new BossRoomHandler(),
            new ShopRoomHandler(),
            new RestRoomHandler()
        };
        foreach (var h in handlers)
            _roomHandlers[h.HandledType] = h;
    }

    void Start()
    {
        StartRun();
    }

    public void StartRun()
    {
        RunState.Instance.InitializeRun(
            Random.Range(0, 99999),
            startingStars ?? new StarSO[0],
            startingPlanets ?? new PlanetSO[0]
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

        switch (phase)
        {
            case GamePhase.Map:
                if (_map.HasNextNodes())
                    Debug.Log("[GameManager] 맵 화면 — 노드를 선택하세요");
                else
                    EnterPhase(GamePhase.Victory);
                break;

            case GamePhase.Combat:
                _combat.StartCombat(
                    null, // TODO: 노드에 따른 웨이브 매핑
                    RunState.Instance.starDeck.ToArray(),
                    RunState.Instance.planetDeck.ToArray()
                );
                break;

            case GamePhase.Reward:
                _reward.ShowRewards(null); // TODO: 노드에 따른 보상 풀
                break;

            case GamePhase.Rest:
                PlayerState.Instance?.Heal(PlayerState.Instance.maxHP * 0.3f);
                Debug.Log("[휴식] HP 30% 회복");
                EnterPhase(GamePhase.Map);
                break;

            case GamePhase.Shop:
                Debug.Log("[상점] TODO: 상점 UI");
                EnterPhase(GamePhase.Map);
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

        // IRoomHandler로 위임
        if (_roomHandlers.TryGetValue(node.roomType, out var handler))
        {
            handler.Enter(node.roomType);
            return;
        }

        // 핸들러 없는 타입은 기본 전투로 처리
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
                EnterPhase(GamePhase.Map);
                break;
        }
    }

    T GetOrAdd<T>() where T : Component
    {
        var c = GetComponent<T>();
        return c != null ? c : gameObject.AddComponent<T>();
    }
}
