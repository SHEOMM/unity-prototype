# 천체 마법 전투 게임 — 프로젝트 가이드

Slay the Spire 스타일 로그라이크 + 천체 주문 비행 게임. 슬링샷 발사체를 하늘로 쏘아 **궤도를 공전하는 천체**를 거치며 만든 시퀀스가 주문이 되어 지상의 적을 공격한다. Phase 9부터 **항성(Star) 개념이 제거**되었고 행성(천체)이 각자 고유 궤도를 가진다.

> 📁 도메인별 상세 문서 (해당 폴더 작업 시 자동 로드):
> - `Assets/Scripts/Synergy/CLAUDE.md` — 시너지 시스템 (37개 + 4 trigger + primitive)
> - `Assets/Scripts/UI/CLAUDE.md` — View 부착 규약 + UIFactory + StatusIcon
> - `Assets/Scripts/VFX/CLAUDE.md` — 행성 VFX (ISpellVisual) + 시너지 VFX (ISynergyVisual) + 공용 visual 6개

---

## 핵심 설계 원칙

1. **Model-View 분리** — 모델(Enemy, PlayerState, PlanetBody, AllyUnit, Structure …)은 로직 + 이벤트 발행만. View는 이벤트 구독. View → Model 호출 금지.
2. **Strategy + `[Attribute]` + Registry** — 전략 클래스를 만들고 `[*Id("x")]` 붙이면 리플렉션 자동 등록. 새 기능 = 파일 1개.
3. **ScriptableObject 데이터** — 모든 엔티티 데이터는 SO. Inspector 편집.
4. **타입 안전 서비스** — `Camera.main` 금지 → `CameraService.Instance.Camera`. `LightingService.Instance.Light` 동일 패턴.
5. **인터페이스 기반 primitive** — `IDamageable`/`IStatusHost`/`IMoveable`에 의존, 구체 클래스 무관.

---

## 패턴 적용 맵

| 영역 | 데이터 SO | 전략 인터페이스 | 어트리뷰트 | 레지스트리 |
|---|---|---|---|---|
| 행성 효과 | `PlanetSO` | `IStarEffect` | `[EffectId]` | `EffectRegistry` |
| 궤도 | `OrbitSO` | — | — | — |
| 행성 비주얼 | — | `ISpellVisual` | `[VisualId]` | `VisualRegistry` |
| 행성 UI | — | `IPlanetHUD` | `[PlanetHUD]` | (DeckManager 부착) |
| 적 행동 | `EnemySO` | `IEnemyBehavior` | `[EnemyBehaviorId]` | `EnemyBehaviorRegistry` |
| 아군 행동 | `AllySO` | `IAllyBehavior` | `[AllyBehaviorId]` | `AllyBehaviorRegistry` |
| 구조물 행동 | `StructureSO` | `IStructureBehavior` | `[StructureBehaviorId]` | `StructureBehaviorRegistry` |
| 유물 | `RelicSO` | `IRelicEffect` | `[RelicEffectId]` | `RelicEffectRegistry` |
| 상태이상 로직 | — | `IStatusEffect` | `[StatusEffectId]` | `StatusEffectRegistry` |
| 상태이상 아이콘 | — | `IStatusIconMeta` | `[StatusIconId]` | `StatusIconRegistry` |
| 시너지 효과 | `SynergyRuleSO` | `ISynergyEffect` | `[SynergyId]` | `SynergyRegistry` |
| 시너지 VFX | — | `ISynergyVisual` | `[SynergyVisualId]` | `SynergyVisualRegistry` |
| 중력 타입 | — | `IGravityType` | `[GravityTypeId]` | `GravityTypeRegistry` |

**상태 객체**(`IPlanetState` + `[PlanetState]`, `IEnemyState` + `[EnemyState]`)는 해당 Effect/Behavior가 필요 시 자동 부착.

---

## 전투 개체 공통 계약 (Ally/Structure/Enemy)

```csharp
IDamageable : CurrentHP/MaxHP/Position/IsAlive/TakeDamage + OnDamaged/OnDeath 이벤트
IStatusHost : IDamageable   // + ApplyStatus + ActiveStatuses (IReadOnlyList)
IMoveable   : BaseSpeed + ApplyKnockback
```

| | IDamageable | IStatusHost | IMoveable |
|---|:-:|:-:|:-:|
| Enemy | ✓ | ✓ | ✓ |
| AllyUnit | ✓ | ✓ | ✓ |
| Structure | ✓ | ✓ | — |

**Primitive는 항상 인터페이스를 타겟팅** (Enemy 직접 참조 금지). Phase 1 primitive 라이브러리가 Ally/Structure도 동일하게 다룰 수 있는 이유.

---

## 확장 가이드 (한 줄 요약)

| 무엇을 추가? | 방법 |
|---|---|
| 행성 효과 | `[EffectId("x")] class X : IStarEffect` + PlanetSO에 `effectId="x"` |
| 행성 비주얼 | `[VisualId("x")] class X : ISpellVisual` + PlanetSO에 `visualId="x"` |
| 적 행동 | `[EnemyBehaviorId("x")] class X : IEnemyBehavior` + EnemySO에 `behaviorId="x"` |
| 아군 유닛 | `[AllyBehaviorId("x")] class X : IAllyBehavior` + AllySO + `AllySpawner.Spawn(...)` |
| 구조물 | `[StructureBehaviorId("x")] class X : IStructureBehavior` + StructureSO + `StructureSpawner.Spawn(...)` |
| 유물 | `[RelicEffectId("x")] class X : RelicEffectBase` + RelicSO |
| 상태이상 | `[StatusEffectId("x")] class X : IStatusEffect` + (옵셔널) `[StatusIconId("x")]` meta 1개 |
| 시너지 (상세는 Synergy/CLAUDE.md) | `[SynergyId("x")] class X : SynergyEffectBase` + `SynergyRuleSO` 에셋 |
| 시너지 비주얼 | `[SynergyVisualId("x")] class X : ISynergyVisual` + rule.visualId="x" |
| 중력 타입 | `[GravityTypeId("x")] class X : IGravityType` + `CelestialBodySO.gravityTypeId="x"` |
| 새 씬 | `class XSceneBoot : SceneBootBase` + `Assets/Data/SceneEnvironments/X.asset` (Main Camera/Light 추가 금지) |

---

## Scene & Camera 아키텍처

- **PersistentScene이 유일한 Main Camera + Global Light 2D 소유자**. 전환 씬에 추가 금지.
- 게임 코드는 `Camera.main` 금지 → `CameraService.Instance.Camera` / `.ScreenToWorld2D(screenPos)` / `.PushTemporaryView(pos, size)` / `.Shake(strength, duration)`.
- 씬 진입 시 `SceneBootBase.Start()` 템플릿이 (1) `SetActiveScene` (2) `SceneEnvironment` 적용 (3) `OnBoot()` 호출.
- `Editor/SceneValidator.cs`가 중복 Main Camera / Global Light / SceneBootBase 누락을 저장 시 자동 검출.

---

## Ship (주문 비행) 파이프라인

```
ShipInput (슬링샷 드래그, 마우스 반대방향 발사)
  → ShipController (이벤트 3종: OnFlightStarted/OnPlanetHit/OnFlightEnded)
    → ShipModel (FixedTimestepSimulator + GravityAccumulator + ShipIntegrator)
    → ShipVisual (밴드 + 궤적 프리뷰 + 트레일)
    → SlashResolver (히트 시퀀스 → SpellResult)            [레거시]
    → SpellEffectManager → VisualRegistry → ISpellVisual    [레거시]
  → SynergyDispatcher 이벤트 구독 → 시너지 발동 → OnSynergyFired → Toast/Visual
```

---

## Model-View 규칙

- 모델에 UI 참조 금지. 이벤트만 발행 (`OnDamaged`, `OnDeath`, `OnPlanetHit`, 시너지의 `OnSynergyFired` 등).
- View는 이벤트 구독만. View → Model 호출 금지.
- View 부착은 팩토리/Spawner에서 (`EnemySpawner` → `EnemyView` + `StatusIconView`, `AllySpawner` → `AllyView` + `StatusIconView`, `StructureSpawner` → `StructureView` + `StatusIconView`).
- 상세 UI 규약은 `Assets/Scripts/UI/CLAUDE.md` 참조.

---

## 폴더 구조

```
Assets/Scripts/
├── Core/               게임 루프, 상태 머신
│   ├── GameManager, CombatManager, MapManager, RewardManager
│   ├── RunState, PlayerState, DeckManager, SceneLoader
│   ├── GameConstants, GamePhase
│   └── Services/       CameraService(+Shake), LightingService, SceneEnvironment, SceneBootBase
├── Data/               ScriptableObject 정의 (Planet/Orbit/Enemy/Ally/Structure/Relic/Wave/Synergy/Comet/Satellite)
├── CelestialSystem/    천체 런타임 (OrbitBody, PlanetBody, CometBody, SatelliteBody, GravitySourceRegistry, PlanetRegistry)
├── Ship/               슬링샷 발사체 (ShipController/Model/Input/Visual/Integrator, GravityTypes/)
├── Slash/              (레거시) 슬래시 시스템
├── Effects/            행성 효과 전략 (IStarEffect + 각 행성), IRelicEffect, ISpellModifier
├── Combat/             Enemy/AllyUnit/Structure + Behaviors/ + StatusEffects/ + 인터페이스 (IDamageable/IStatusHost/IMoveable)
├── Synergy/            시너지 시스템 — 상세는 Synergy/CLAUDE.md
│   ├── ISynergyEffect, SynergyRuleSO, SynergyDispatcher, SynergyRegistry, Matcher, TriggerType
│   ├── Effects/        Fire/Water/Wind/Earth/Lightning/War/Dark/Civilization + Position/Combo/PerHit
│   └── Primitives/     Dot/Slow/Knockback/Stun/Weakness/Execute/Chain/Aoe/Sweep/AllySpawner/StructureSpawner/PlayerBuff/AllyBuff/AllyHealApplicator/EnemyFiltering
├── VFX/                행성 VFX (ISpellVisual + VisualRegistry) + 시너지 VFX (ISynergyVisual + SynergyVisualRegistry + Synergy/ 6 공용) — 상세는 VFX/CLAUDE.md
├── UI/                 View 계층 — 상세는 UI/CLAUDE.md
│   ├── EnemyView/AllyView/StructureView, PlanetLabelView, PlayerHPBar, PlayerDamageView
│   ├── SynergyPopup (레거시), SynergyToastView, MapView, CombatDividerView
│   ├── StatusIconView + StatusIconRegistry + StatusIconMeta/
│   └── UIFactory (CreateLabel / CreateHPBar / UpdateHPBar / MakePixel)
├── Map/                MapManager, MapSceneBoot, MapGenerator, MapData/Node
└── Editor/             SceneValidator, SynergySmokeTest
```

---

## 게임 상태 흐름

```
StartRun → [Map] → [Combat] → [Reward] → [Map] → … → [Boss] → [Victory]
                                       ↑
                        HP ≤ 0 → [GameOver]
```

---

## 주요 관례

- `Camera.main` 직접 접근 금지 — `CameraService` 경유
- Primitive에 Enemy/Ally/Structure 구체 타입 넘기지 않기 — `IDamageable`/`IStatusHost`/`IMoveable` 사용
- `SynergyDefinitionSO`는 `[Obsolete]` — 레거시 SlashResolver 경유만. 신규 기능은 `SynergyRuleSO` + `[SynergyId]`
- Boot 스크립트는 항상 `SceneBootBase` 상속 (씬마다 `SceneEnvironment` 에셋 바인딩 필수)
- 전투 종료 시 `AllyRegistry.DestroyAll()` + `StructureRegistry.DestroyAll()` (combat-scoped)
- 새 `IStatusEffect` 구현 시 `IconId` 오버라이드 고려 (기본 null = 아이콘 표시 안 함)

---

## 전투 루프 (Phase 9b)

**Map → Combat → Reward → Map** 루프가 실제 동작.

| 단계 | 메커닉 |
|---|---|
| **난이도 스케일** | `CombatSceneBoot.ComputeDifficulty(floor, roomType)` → `EnemySpawner.spawnCountMultiplier`. 층당 +15%, Elite ×1.5, Boss ×2.0 |
| **전투 클리어** | 모든 웨이브 전멸 → `EnemySpawner.OnAllWavesComplete` → `CombatManager.EndCombat` → `OnCombatComplete` → `GameManager.EnterPhase(Reward)` |
| **보상 선택** | `RewardSceneBoot`가 `RewardManager.BuildChoices`로 3 선택지 구성 (궤도·행성·유물 혼합, 보유 중인 것 자동 제외) → World Space 카드 3장 → 클릭 시 `payload.ApplyAsReward(player, run)` |
| **자동 배치** | Cosmos 씬 대체. `OrbitSO.ApplyAsReward` → `RunState.TryAutoAssignPlanetToOrbit` / `PlanetSO` 획득 시 `TryAutoAssignOrbitToPlanet`. 빈 궤도/미배치 행성이 있으면 즉시 연결 |
| **맵 복귀** | `RewardManager.OnRewardChosen` → `RunState.AdvanceFloor` + `EnterPhase(Map)` |

**보상 풀**: `GameManager.rewardOrbitPool` / `rewardPlanetPool` / `rewardRelicPool` — Inspector에서 편집.
**보상 확장**: 새 보상 타입 = `IRewardApplicable` 구현 클래스 추가. RewardManager.BuildChoices 카테고리에 따른 `MakeXChoice` 팩토리 하나 추가.

---

## 천체·궤도 시스템 (Phase 9)

항성(Star) 개념 제거. 모든 것은 **천체(Planet)**이고 각자 **고유 궤도(Orbit)**를 가진다.

| 레이어 | 역할 |
|---|---|
| `Data/OrbitSO` | 궤도 데이터 — `radius`, `angularSpeed`(deg/s, 음수=역방향), `startAngle`, `eccentricity`, `orbitLineColor` |
| `CelestialSystem/OrbitBody` | 궤도 런타임 — LineRenderer 궤도선 + 1 행성 점유 + 공전 업데이트 |
| `Data/PlanetSO` (최상위 천체) | 행성 데이터. 독립적으로 덱에 들어감 |
| `CelestialSystem/PlanetBody` | 행성 런타임. OrbitBody의 child로 배치되어 공전 |
| `RunState.unlockedOrbits` + `orbitAssignments` | 보유 궤도 목록 + 행성↔궤도 매핑 (`OrbitAssignment struct`) |
| `GameManager.startingOrbits` + `defaultAssignments` | 런 시작 시 주어지는 초기 구성 |
| `CombatManager.SetupCosmos` | 전투 시작 시 궤도들을 월드 원점 y=celestialYCenter에 동심원으로 배치 + 매핑된 행성 부착 |

**규칙**: 1 궤도 = 1 천체. 천체 수 > 궤도 수면 일부 미배치 (RunState에 매핑 없으면 생성 안 함). 궤도·천체는 스테이지 클리어 보상으로 획득 (Phase 9b 예정).

### 확장
- 새 궤도 = `OrbitSO` 에셋 추가 → `GameManager.startingOrbits` 또는 보상 풀에 배치
- 새 천체 = 기존 `PlanetSO` 추가 가이드 그대로

---

## 행성 스프라이트 파이프라인 (Phase 8)

`Assets/art/planet/*.png` 4개 시트를 Grid 슬라이싱해 픽셀 아트 아이콘 사용. 절차 생성기(`CelestialSpriteGenerator`)는 폴백으로 유지.

| 레이어 | 역할 |
|---|---|
| `Editor/PlanetSpriteSheetSlicer` | `Tools/Art/Slice Planet Sheets` — 4 시트 Grid 슬라이싱 (셀 크기 상수, 바꾸면 재실행) |
| `Data/PlanetSpriteLibrary` + `PlanetSpriteLibraryPopulator` | `Tools/Art/Populate Planet Sprite Library` — 서브 Sprite를 티어(big/medium/small/verySmall) 배열로 집계 |
| `Data/PlanetIconBindingTable` + `PlanetIconBinder` | Inspector 편집 가능한 행성↔스프라이트 SO. `Tools/Art/Seed Planet Icon Bindings` (기본값 채움) + `Tools/Art/Apply Planet Icon Bindings` (PlanetSO.icon 주입) |
| `Data/PlanetSpriteResolver.Resolve(PlanetSO)` | 렌더 시 `icon` 있으면 사용, 없으면 절차 폴백. `DeckManager.CreatePlanet` 유일 호출자 |

**매핑 변경**: BindingTable SO를 Inspector에서 편집 → `Apply Planet Icon Bindings`. 코드 변경 불필요.
**스프라이트 교체**: 시트 재슬라이싱 → Populator 재실행 → BindingTable 갱신 → Apply.
