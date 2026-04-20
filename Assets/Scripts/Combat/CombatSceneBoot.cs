using UnityEngine;

/// <summary>
/// CombatScene 진입 시 자동 초기화. 전투 시스템을 설정하고 전투를 시작.
/// Phase 9b: floor × roomType 기반 난이도 승수 계산 + EnemySpawner에 주입.
/// </summary>
public class CombatSceneBoot : SceneBootBase
{
    const float FloorGrowth = 0.15f;      // 층당 +15% 적 수
    const float EliteMultiplier = 1.5f;
    const float BossMultiplier = 2.0f;

    protected override void OnBoot()
    {
        var gm = GameManager.Instance;
        if (gm == null) { Debug.LogError("[CombatScene] GameManager not found"); return; }

        var combatGo = new GameObject("CombatManager");
        var combat = combatGo.AddComponent<CombatManager>();
        combat.synergyRules = gm.SynergyRules;
        combat.cometPool = gm.CometPool;
        combat.Initialize();

        combat.OnCombatComplete += () => gm.EnterPhase(GamePhase.Reward);

        int floor = RunState.Instance?.currentFloor ?? 0;
        var roomType = gm.CurrentNode?.roomType;
        float difficulty = ComputeDifficulty(floor, roomType);
        WaveDefinitionSO[] waves = BuildWaveSet(gm.DefaultWaves, floor, roomType);

        // 배경: 천상(space) + 지상(ground). 씬 divider y 기준으로 영역 분할.
        SpawnBackgroundView(combat, roomType);

        combat.StartCombat(
            waves,
            RunState.Instance?.unlockedOrbits,
            RunState.Instance?.orbitAssignments,
            RunState.Instance?.planetDeck,
            difficulty
        );

        Debug.Log($"[CombatScene] 전투 시작 — 웨이브 {(waves != null ? waves.Length : 0)}개, 난이도 ×{difficulty:F2}");

        combatGo.AddComponent<PlayerHPBar>();
        combatGo.AddComponent<PlayerDamageView>();
        combatGo.AddComponent<SynergyToastView>().Bind(combat.SynergyDispatcher);
        combatGo.AddComponent<SynergyVisualHost>().Bind(combat.SynergyDispatcher);

        SpawnPlayerCharacter(gm.PlayerCharacter);
    }

    static void SpawnPlayerCharacter(CharacterAnimationSet set)
    {
        if (set == null) return;
        var go = new GameObject("PlayerCharacter");
        var view = go.AddComponent<PlayerCharacterView>();
        view.animations = set;
    }

    static void SpawnBackgroundView(CombatManager combat, RoomType? roomType)
    {
        var cam = CameraService.Instance?.Camera;
        if (cam == null) return;

        var key = MapRoomTypeToKey(roomType);
        var spaceSet = BackgroundResolver.ResolveSpace(key);
        var groundSet = BackgroundResolver.ResolveGround(key);
        if (spaceSet == null && groundSet == null) return;

        float dividerY = combat.celestialYCenter - combat.celestialRadius;
        float camTop = cam.transform.position.y + cam.orthographicSize;
        float camBottom = cam.transform.position.y - cam.orthographicSize;

        var bgGo = new GameObject("BackgroundView");
        var bgView = bgGo.AddComponent<BackgroundView>();
        if (groundSet != null)
            bgView.ApplyGround(groundSet, cam, camBottom, dividerY, GameConstants.SortingOrder.BackgroundGroundBase);
        if (spaceSet != null)
            bgView.ApplySpace(spaceSet, cam, dividerY, camTop, GameConstants.SortingOrder.BackgroundSpace);
    }

    static BackgroundKey MapRoomTypeToKey(RoomType? roomType)
    {
        if (roomType == RoomType.Elite) return BackgroundKey.CombatElite;
        if (roomType == RoomType.Boss) return BackgroundKey.CombatBoss;
        return BackgroundKey.Combat;
    }

    static float ComputeDifficulty(int floor, RoomType? roomType)
    {
        float mult = 1f + floor * FloorGrowth;
        if (roomType == RoomType.Elite) mult *= EliteMultiplier;
        else if (roomType == RoomType.Boss) mult *= BossMultiplier;
        return mult;
    }

    /// <summary>
    /// 층·roomType에 따라 DefaultWaves에서 앞 N개만 사용.
    /// 초반 전투는 1 웨이브로 짧게, Elite/Boss는 전체.
    /// </summary>
    static WaveDefinitionSO[] BuildWaveSet(WaveDefinitionSO[] source, int floor, RoomType? roomType)
    {
        if (source == null || source.Length == 0) return source;

        int target;
        if (roomType == RoomType.Boss) target = source.Length;
        else if (roomType == RoomType.Elite) target = Mathf.Min(source.Length, 3);
        else if (floor <= 1) target = 1;
        else if (floor <= 4) target = Mathf.Min(source.Length, 2);
        else target = source.Length;

        if (target >= source.Length) return source;
        var result = new WaveDefinitionSO[target];
        System.Array.Copy(source, result, target);
        return result;
    }
}
