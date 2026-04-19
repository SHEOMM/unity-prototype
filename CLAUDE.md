# 천체 마법 전투 게임 — 프로젝트 가이드

Slay the Spire 스타일 로그라이크 + 천체 주문 비행 게임. 발사체(슬링샷)를 하늘로 쏘아 천체를 거치며 만든 시퀀스가 주문이 되어 지상의 적을 공격한다.

---

## 핵심 설계 원칙

1. **Model-View 분리** — 모델(Enemy, PlayerState, PlanetBody, AllyUnit, Structure …)은 로직 + 이벤트 발행만. View는 이벤트 구독으로 표시만. View → Model 호출 금지.
2. **Strategy + `[Attribute]` + Registry** — 전략 클래스를 만들고 `[*Id("x")]` 붙이면 리플렉션으로 자동 등록. 새 기능 = 파일 1개.
3. **ScriptableObject 데이터** — 모든 엔티티 데이터는 SO. Inspector 편집.
4. **타입 안전 서비스** — `Camera.main` 금지 → `CameraService.Instance.Camera`. `LightingService.Instance.Light` 동일 패턴.
5. **인터페이스 기반 primitive** — `IDamageable`/`IStatusHost`/`IMoveable`에 의존, 구체 클래스 무관.

---

## 패턴 적용 맵

| 영역 | 데이터 SO | 전략 인터페이스 | 어트리뷰트 | 레지스트리 | 상태 |
|---|---|---|---|---|---|
| 행성 효과 | `PlanetSO` | `IStarEffect` | `[EffectId]` | `EffectRegistry` | `IPlanetState` + `[PlanetState]` |
| 행성 비주얼 | — | `ISpellVisual` | `[VisualId]` | `VisualRegistry` | — |
| 행성 UI | — | `IPlanetHUD` | `[PlanetHUD]` | (DeckManager에서 부착) | — |
| 적 행동 | `EnemySO` | `IEnemyBehavior` | `[EnemyBehaviorId]` | `EnemyBehaviorRegistry` | `IEnemyState` + `[EnemyState]` |
| 아군 행동 | `AllySO` | `IAllyBehavior` | `[AllyBehaviorId]` | `AllyBehaviorRegistry` | — |
| 구조물 행동 | `StructureSO` | `IStructureBehavior` | `[StructureBehaviorId]` | `StructureBehaviorRegistry` | — |
| 유물 | `RelicSO` | `IRelicEffect` | `[RelicEffectId]` | `RelicEffectRegistry` | — |
| 상태이상 | — | `IStatusEffect` | `[StatusEffectId]` | `StatusEffectRegistry` | — |
| 시너지 | `SynergyRuleSO` | `ISynergyEffect` | `[SynergyId]` | `SynergyRegistry` | — |
| 중력 타입 | — | `IGravityType` | `[GravityTypeId]` | `GravityTypeRegistry` | — |
| 보상 | — | `IRewardApplicable` | — | — | — |

---

## 전투 개체 공통 계약 (Ally/Structure/Enemy)

```csharp
IDamageable : CurrentHP/MaxHP/Position/IsAlive/TakeDamage + 이벤트
IStatusHost : IDamageable   // + ApplyStatus
IMoveable   : BaseSpeed, ApplyKnockback
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
| 행성 상태 | `[PlanetState(typeof(MyState))]`을 `IStarEffect`에 |
| 적 행동 | `[EnemyBehaviorId("x")] class X : IEnemyBehavior` + EnemySO에 `behaviorId="x"` |
| 아군 유닛 | `[AllyBehaviorId("x")] class X : IAllyBehavior` + AllySO + `AllySpawner.Spawn(...)` |
| 구조물 | `[StructureBehaviorId("x")] class X : IStructureBehavior` + StructureSO + `StructureSpawner.Spawn(...)` |
| 유물 | `[RelicEffectId("x")] class X : RelicEffectBase` + RelicSO |
| 상태이상 | `[StatusEffectId("x")] class X : IStatusEffect` → `StatusEffectRegistry.Get(id)`로 꺼내서 `enemy.ApplyStatus(new StatusEffect(effect, duration))` |
| 시너지 효과 | `[SynergyId("x")] class X : SynergyEffectBase` + `SynergyRuleSO`(trigger 조건 + effectId="x") |
| 중력 타입 | `[GravityTypeId("x")] class X : IGravityType` + `CelestialBodySO.gravityTypeId="x"` |
| 새 씬 | `class XSceneBoot : SceneBootBase` + `Assets/Data/SceneEnvironments/X.asset` (Main Camera/Light 추가 금지) |

---

## 시너지 시스템 (Phase 0-4 구현 완료 — 30 시너지)

### 3층 구조
- **`SynergyRuleSO`** (데이터) — 언제 발동할지 + 파라미터: `triggerType` + `family`+`threshold` / `positionKey`+`planetKey` / `sequenceKeys`. `damage`/`radius`/`duration`/`secondary`/`count`/`element` 5+1개 공통 파라미터. `allyToSpawn`/`structureToSpawn` 레퍼런스로 유닛/구조물 SO 바인딩.
- **`ISynergyEffect`** (로직) — `OnFlightEnd` 훅만 사용 (OnHit은 인터페이스에 남아있되 현재 미호출). Stateless. `ctx.CurrentRule`에서 파라미터 읽어 primitive 조합.
- **`SynergyDispatcher`** (조율자) — **모든 trigger는 end-of-flight에서 판정**. FamilyAccumulation은 family별 highest-only (최고 threshold 1개). SequencePosition/PlanetCombo는 매칭되는 모두 발동.

### Primitive 라이브러리 (`Assets/Scripts/Synergy/Primitives/`)
`DotApplicator`, `SlowApplicator`, `KnockbackApplicator`, `StunApplicator`, `WeaknessApplicator`, `ExecuteApplicator`, `ChainLightning`, `AoeApplicator`, `SweepLine`, `AllySpawner`, `StructureSpawner`, `PlayerBuff`, `AllyBuff`, `AllyHealApplicator`, `EnemyFiltering`. 모두 인터페이스 타겟. 새 시너지는 조합만.

### 구현된 24 FamilyAccumulation 시너지 (`Effects/{Family}/`)
| Family | T1 | T2 | T3 |
|---|---|---|---|
| Fire | Fireball | FlameBurst | Inferno (AoE+DoT) |
| Water | Droplet | Rain | TidalWave |
| Wind | Gust | WindAura (Player+Ally buff) | Cyclone |
| Earth | Spike | Meteor | Wall (Barrier ×3) |
| Lightning | Spark | Thunderbolt (+Stun) | Thunderstorm |
| War | Recruit | Squad | Army (+AllyBuff) |
| Dark | PoisonCloud | Curse (Execute) | Plague (DoT+Weakness) |
| Civilization | Watchtower | Farm | Fortress |

### Phase 4 SequencePosition 시너지 6개 (`Effects/Position/`)
| ID | positionKey | planetKey | 효과 |
|---|---|---|---|
| pos_leading_warrior | Leading | valor | 전역 Fire AoE |
| pos_leading_emperor | Leading | authority | 타깃 AoE + Stun |
| pos_trailing_storm | Trailing | storm | ChainLightning 4회 |
| pos_trailing_scorpion | Trailing | venom | 전체 적 DoT |
| pos_trailing_assassin | Trailing | shadow | HP 30% 이하 전체 처형 |
| pos_any_love | Any | heart | 전체 아군 Heal |

`planetKey`는 `PlanetSO.bodyName` 또는 `keywords` 배열 중 하나와 일치하면 매칭.

### SynergyFamily enum
`Fire / Water / Wind / Earth / Lightning / War / Dark / Civilization` — Element(데미지 속성)와 별개 축. `PlanetSO.synergyFamily`로 지정.

---

## Scene & Camera Architecture

- **PersistentScene이 유일한 Main Camera + Global Light 2D 소유자**. 전환 씬에 추가 금지.
- 게임 코드는 `Camera.main` 금지 → `CameraService.Instance.Camera` / `.ScreenToWorld2D(screenPos)` / `.PushTemporaryView(pos, size)` (IDisposable 스코프로 일시 이동).
- 씬 진입 시 `SceneBootBase.Start()` 템플릿이: (1) `SetActiveScene` (2) `SceneEnvironment` 적용 (3) `OnBoot()` 호출.
- `Editor/SceneValidator.cs`가 중복 Main Camera / Global Light / SceneBootBase 누락을 에디터 저장 시 자동 검출.

---

## Ship (주문 비행) 파이프라인

```
ShipInput (슬링샷 드래그, 마우스 반대방향으로 발사)
  → ShipController (조율, 이벤트 3종 발행: OnFlightStarted/OnPlanetHit/OnFlightEnded)
    → ShipModel (FixedTimestepSimulator + GravityAccumulator + ShipIntegrator)
    → ShipVisual (밴드 + 궤적 프리뷰 + 트레일)
    → SlashResolver (히트 시퀀스 → SpellResult)
    → SpellEffectManager → VisualRegistry → ISpellVisual
  → SynergyDispatcher가 ShipController 이벤트 구독 → 시너지 발동
```

---

## Model-View 규칙

- 모델(Enemy, AllyUnit, Structure, PlayerState, PlanetBody, ShipModel, ProjectileBody)에 UI 참조 금지
- 모델은 이벤트만 발행 (`OnDamaged`, `OnDeath`, `OnPlanetHit`, …)
- View는 이벤트 구독만. View → Model 호출 금지.
- View 부착은 팩토리/Spawner에서 (`EnemySpawner`가 `AddComponent<EnemyView>()`)

---

## 폴더 구조

```
Assets/Scripts/
├── Core/               게임 루프, 상태 머신
│   ├── GameManager, CombatManager, MapManager, RewardManager
│   ├── RunState, PlayerState, DeckManager, SceneLoader
│   ├── GameConstants, GamePhase
│   └── Services/       CameraService, LightingService, SceneEnvironment, SceneBootBase
├── Data/               ScriptableObject 정의 (Planet/Star/Enemy/Ally/Structure/Relic/Wave/Synergy/…)
├── CelestialSystem/    천체 런타임 (StarSystem, OrbitRing, PlanetBody, CometBody, GravitySourceRegistry)
├── Ship/               슬링샷 발사체 (ShipController/Model/Input/Visual/Integrator, GravityTypes/)
├── Slash/              (레거시) 슬래시 시스템
├── Effects/            행성 효과 전략 (IStarEffect + 각 행성), IRelicEffect, ISpellModifier
├── Combat/             Enemy/AllyUnit/Structure + 각 Behavior/Registry + StatusEffects/ + 인터페이스
├── Synergy/            ISynergyEffect, SynergyRegistry/Dispatcher/RuleSO/TriggerType/Matcher
│   └── Primitives/     Dot/Slow/Knockback/Execute/Chain/Aoe/Sweep/AllySpawner/StructureSpawner/…
├── VFX/                ISpellVisual + 각 Visual, VisualRegistry, 스프라이트 생성기
├── UI/                 EnemyView, PlanetLabelView, PlayerHPBar, PlayerDamageView, SynergyPopup, MapView, …
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

## 주요 관례 한 줄씩
- `Camera.main` 직접 접근 금지 (`CameraService` 경유)
- Enemy/Ally/Structure는 `IDamageable` 대신 구체 타입을 primitive에 넘기지 않기
- `SynergyDefinitionSO`는 `[Obsolete]` (레거시 SlashResolver 경유만 사용, 신규 기능은 `SynergyRuleSO` + `[SynergyId]`)
- Boot 스크립트는 항상 `SceneBootBase` 상속 (씬마다 `SceneEnvironment` 에셋 바인딩 필수)
- 전투 종료 시 `AllyRegistry.DestroyAll()` + `StructureRegistry.DestroyAll()` (combat-scoped, run-scoped 아님)
