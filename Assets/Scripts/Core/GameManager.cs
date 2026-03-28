using UnityEngine;

/// <summary>
/// 게임 상태 머신 오케스트레이터.
/// Map → Combat → Reward → Map 루프를 관리한다.
/// 실제 전투/보상/지도 로직은 각 매니저에 위임.
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

    public GamePhase CurrentPhase { get; private set; }
    public System.Action<GamePhase, GamePhase> OnPhaseChanged;

    private CombatManager _combat;
    private RewardManager _reward;
    private MapManager _map;
    private RoomDefinitionSO _currentRoom;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        GetOrAdd<PlanetRegistry>();
        GetOrAdd<EnemyRegistry>();
        GetOrAdd<PlayerState>();
        GetOrAdd<PlayerHPBar>();
        GetOrAdd<PlayerDamageView>();
        GetOrAdd<RunState>();

        _combat = GetOrAdd<CombatManager>();
        _combat.synergies = synergies;
        _combat.cometPool = cometPool;
        _combat.Initialize();
        _reward = GetOrAdd<RewardManager>();
        _map = GetOrAdd<MapManager>();

        _combat.OnCombatComplete += () => EnterPhase(GamePhase.Reward);
        _reward.OnRewardChosen += () => EnterPhase(GamePhase.Map);
        _map.OnRoomSelected += OnRoomSelected;
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

        if (currentAct != null)
            _map.GenerateMap(currentAct);

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
                if (_map.HasNextRoom())
                    _map.ShowMap();
                else
                    EnterPhase(GamePhase.Victory);
                break;

            case GamePhase.Combat:
                if (_currentRoom?.waveDefs != null)
                    _combat.StartCombat(
                        _currentRoom.waveDefs,
                        RunState.Instance.starDeck.ToArray(),
                        RunState.Instance.planetDeck.ToArray()
                    );
                break;

            case GamePhase.Reward:
                _reward.ShowRewards(_currentRoom?.rewardPool);
                break;

            case GamePhase.Rest:
                PlayerState.Instance?.Heal(PlayerState.Instance.maxHP * 0.3f);
                Debug.Log("[휴식] HP 30% 회복");
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

    void OnRoomSelected(RoomDefinitionSO room)
    {
        _currentRoom = room;
        switch (room.roomType)
        {
            case RoomType.Combat:
            case RoomType.Elite:
            case RoomType.Boss:
                EnterPhase(GamePhase.Combat);
                break;
            case RoomType.Rest:
                EnterPhase(GamePhase.Rest);
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
