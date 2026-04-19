# 천체 마법 전투 게임 — 프로젝트 가이드

## 아키텍처 개요

Slay the Spire 스타일 로그라이크 + 천체 슬래시 전투 게임. 하늘에서 행성을 슬래시하여 지상의 적을 공격한다.

### 핵심 설계 원칙

**1. Model-View 분리**
- **모델** (Enemy, PlayerState, PlanetBody): 게임 로직만 담당. UI 코드 0줄. 상태 변경 시 C# event 발행.
- **뷰** (EnemyView, PlayerHPBar, PlanetLabelView 등): 이벤트를 구독하여 표시만 담당. 게임 상태를 수정하지 않음.
- **팩토리** (UIFactory): 라벨, HP바 등 공통 UI 요소 생성 유틸리티.

**2. Strategy + [Attribute] + Registry 패턴**
- 로직을 인터페이스로 분리, 어트리뷰트로 ID 선언, 리플렉션으로 자동 등록.
- 새 기능 추가 시 파일 1개 생성만으로 확장 가능. 기존 코드 수정 불필요.

**3. ScriptableObject 데이터**
- 모든 엔티티 데이터를 SO로 정의. Inspector에서 편집 가능.

**4. 공유 유틸리티**
- `GameConstants`: 모든 매직 넘버를 이름 있는 상수로 관리
- `SlashGeometry`: 충돌 판정 공통 로직
- `UIFactory`: UI 요소 생성 공통 로직

### 패턴 적용 맵

| 영역 | SO (데이터) | 전략 인터페이스 | 어트리뷰트 | 레지스트리 | 상태 |
|---|---|---|---|---|---|
| 행성 효과 | `PlanetSO` | `IStarEffect` | `[EffectId]` | `EffectRegistry` | `IPlanetState` + `[PlanetState]` |
| 행성 비주얼 | — | `ISpellVisual` | `[VisualId]` | `VisualRegistry` | — |
| 행성 UI | — | `IPlanetHUD` | `[PlanetHUD]` | — (DeckManager에서 부착) | — |
| 적 행동 | `EnemySO` | `IEnemyBehavior` | `[EnemyBehaviorId]` | `EnemyBehaviorRegistry` | `IEnemyState` + `[EnemyState]` |
| 유물 | `RelicSO` | `IRelicEffect` | `[RelicEffectId]` | `RelicEffectRegistry` | — |
| 상태이상 | — | `IStatusEffect` | `[StatusEffectId]` | `StatusEffectRegistry` | — |
| 보상 | — | `IRewardApplicable` | — | — | — |

---

## 새 행성 효과 추가하기

### 1단계: 효과 클래스 (`Effects/`)

```csharp
[EffectId("my_planet")]
public class MyPlanetEffect : IStarEffect
{
    public void Execute(EffectContext ctx)
    {
        float dmg = ctx.source.Planet.baseDamage * ctx.damageMultiplier;
        ctx.result.commands.Add(new SpellCommand
        {
            element = ctx.source.Planet.element,
            damage = dmg,
            hitCount = 1 + ctx.extraHits,
            sourceName = ctx.source.Planet.bodyName,
            visualType = SpellVisualType.Strike,
            visualId = "my_planet"  // 고유 비주얼이 있으면
        });
    }
}
```

### 2단계: PlanetSO 에셋 (`Assets/Data/Planets/`)
- Create → Celestial → Planet, `effectId`에 "my_planet"

### (선택) 지속 상태: `[PlanetState(typeof(MyState))]` 추가
### (선택) 고유 비주얼: `[VisualId("my_planet")]` 클래스 생성 (`VFX/`)
### (선택) 상태 UI: `[PlanetHUD(typeof(MyHUD))]` 추가 — DeckManager가 자동 부착
### (선택) 히트리스트 전처리: `ISlashModifier` 함께 구현

---

## 새 적 추가하기

### 1단계: 행동 클래스 (`Combat/Behaviors/`)

```csharp
[EnemyBehaviorId("my_enemy")]
public class MyEnemyBehavior : IEnemyBehavior
{
    public bool Tick(Enemy enemy, float dt) => true;
    public float ModifyIncomingDamage(Enemy enemy, float dmg, Element el) => dmg;
    public bool OnDeath(Enemy enemy) => true;
}
```

### 2단계: EnemySO 에셋 (`Assets/Data/Enemies/`)
- Create → Combat → Enemy, `behaviorId`에 "my_enemy"

EnemyView가 자동으로 라벨 + HP바 + 데미지 팝업을 표시함 (EnemySpawner가 부착).

### (선택) 상태: `[EnemyState(typeof(MyState))]` 추가

---

## 새 유물 추가하기

### 1단계: 효과 클래스 (`Effects/Relics/`)

```csharp
[RelicEffectId("my_relic")]
public class MyRelicEffect : RelicEffectBase
{
    public override void OnEnemyKilled(Enemy enemy, PlayerState player)
    {
        player.Heal(2f);
    }
}
```

사용 가능한 훅: `OnAcquired`, `OnSlashPerformed`, `OnEnemyKilled`, `OnWaveStart`, `OnWaveComplete`, `OnBeforeDamage(ref)`, `OnAfterDamage`

### 2단계: RelicSO 에셋
- Create → Data → Relic, `effectId`에 "my_relic"

RelicSO는 `IRewardApplicable`을 구현하므로 보상 풀에 넣으면 자동 적용됨.

---

## 새 상태이상 추가하기

### 1단계: 효과 클래스 (`Combat/StatusEffects/`)

```csharp
[StatusEffectId("burn")]
public class BurnEffect : IStatusEffect
{
    public void Tick(Enemy target, float dt) { /* 틱 로직 */ }
}
```

### 2단계: 적용
```csharp
var effect = StatusEffectRegistry.Get("burn");
enemy.ApplyStatus(new StatusEffect(effect, duration: 5f));
```

---

## 폴더 구조

```
Assets/Scripts/
├── Core/               — 게임 루프, 상태 머신
│   ├── GameManager.cs        (상태 머신: Map→Combat→Reward)
│   ├── CombatManager.cs      (전투 조율: 웨이브, 혜성)
│   ├── MapManager.cs         (StS 스타일 지도)
│   ├── RewardManager.cs      (보상 선택, IRewardApplicable)
│   ├── RunState.cs           (런 진행: 층, 덱, 시드)
│   ├── PlayerState.cs        (HP, 유물, 이벤트 발행)
│   ├── DeckManager.cs        (천체 생성 + View 부착)
│   ├── GameConstants.cs      (공유 상수)
│   └── GamePhase.cs          (상태 enum)
├── Data/               — ScriptableObject 정의
│   ├── PlanetSO, StarSO, SatelliteSO, CometSO
│   ├── EnemySO, WaveDefinitionSO
│   ├── RelicSO, RoomDefinitionSO, ActDefinitionSO
│   └── Element, ElementResistance, EnemyShape
├── CelestialSystem/    — 천체 런타임 (순수 모델)
│   ├── StarSystem, OrbitRing, PlanetBody
│   ├── SatelliteBody, CometBody, CometSpawner
│   ├── PlanetRegistry, AquariusState
├── Effects/            — 전략 패턴 로직
│   ├── IStarEffect + 행성별 효과 (Archer, Emperor, Libra...)
│   ├── ISpellVisual + VisualRegistry
│   ├── IRelicEffect + RelicEffectBase + Relics/
│   ├── IRewardApplicable, ISlashModifier
│   ├── IPlanetState, IPlanetHUD (+ 어트리뷰트)
│   └── EffectRegistry, EffectContext, SlashResult
├── Slash/              — 슬래시 시스템
│   ├── SlashController.cs    (입력→판정→해석 통합)
│   ├── SlashInput, SlashDetector, SlashResolver
│   ├── SlashVisual, SlashGeometry
│   └── ISlashTarget
├── Combat/             — 적 시스템 (순수 모델)
│   ├── Enemy.cs              (이벤트 발행, UI 코드 없음)
│   ├── EnemySpawner.cs       (적 생성 + EnemyView 부착)
│   ├── IEnemyBehavior + Behaviors/ (Grunt, Tank, Healer...)
│   ├── IStatusEffect + StatusEffects/ (Poison, Slow...)
│   ├── EnemyRegistry, SpellEffectManager
│   └── StatusEffectRegistry + 어트리뷰트
├── VFX/                — 비주얼 전략
│   ├── ISpellVisual + DefaultVisual, ArcherVisual, EmperorVisual...
│   ├── VisualRegistry + VisualIdAttribute
│   ├── CelestialSpriteGenerator, EnemySpriteGenerator
└── UI/                 — 뷰 컴포넌트 (표시 전담)
    ├── EnemyView.cs          (적 라벨 + HP바 + 데미지 팝업)
    ├── PlanetLabelView.cs    (행성 이름 라벨)
    ├── PlayerHPBar.cs        (플레이어 HP 바, 이벤트 구독)
    ├── PlayerDamageView.cs   (플레이어 피격 팝업)
    ├── CombatDividerView.cs  (천상/지상 구분선)
    ├── AquariusHUD.cs        (물병별 수위 게이지)
    ├── DamagePopup.cs, SynergyPopup.cs
    └── UIFactory.cs          (라벨/HP바 생성 유틸리티)
```

## Model-View 규칙

| 규칙 | 설명 |
|---|---|
| 모델은 UI를 모른다 | Enemy, PlayerState, PlanetBody에 UIFactory, DamagePopup 등 UI 참조 금지 |
| 모델은 이벤트만 발행 | `OnDamaged?.Invoke()`, `OnHPChanged?.Invoke()` |
| View는 이벤트를 구독 | EnemyView가 Enemy.OnDamaged 구독 → DamagePopup.Spawn() |
| View는 상태를 수정하지 않음 | View → Model 방향 호출 금지 |
| View 부착은 팩토리에서 | EnemySpawner → `AddComponent<EnemyView>()`, DeckManager → `AddComponent<PlanetLabelView>()` |

## SlashController 파이프라인

```
SlashInput (드래그 감지)
  → SlashController (통합 조율)
    → SlashDetector (충돌 판정, SlashGeometry 사용)
    → SlashResolver (효과 해석: PreProcess → Execute → PostProcess)
    → SlashVisual (선 렌더링)
    → SpellEffectManager → VisualRegistry → ISpellVisual
  → OnSlashComplete 이벤트 → CombatManager/PlayerState
```

## Scene & Camera Architecture

### 원칙
- **PersistentScene이 유일한 Main Camera와 Global Light 2D 소유자**. 전환 씬에는 부착 금지.
- 게임 코드는 `Camera.main` 금지 → `CameraService.Instance.Camera` 또는 `CameraService.Instance.ScreenToWorld2D(screenPos)` 사용.
- 씬 진입 로직은 `SceneBootBase`를 상속 → 활성 씬 설정과 환경 적용을 템플릿이 처리.

### 구성 요소

| 경로 | 역할 |
|---|---|
| `Core/Services/CameraService.cs` | Main Camera 타입 안전 래퍼. `Camera`, `ScreenToWorld2D`, `ApplyEnvironment`, `PushTemporaryView` IDisposable |
| `Core/Services/LightingService.cs` | Global Light 2D 래퍼. `Light`, `ApplyEnvironment` |
| `Core/Services/SceneEnvironment.cs` | SO 데이터: `cameraPosition`, `orthographicSize`, `backgroundColor`, `lightIntensity`, `lightColor` |
| `Core/Services/SceneBootBase.cs` | 추상 MonoBehaviour. `Start()` 템플릿: SetActiveScene → ApplyEnvironment → `OnBoot()` |
| `Assets/Data/SceneEnvironments/*.asset` | 씬별 환경 에셋 |
| `Editor/SceneValidator.cs` | 에디터 검증: Main Camera/Global Light/SceneBootBase 불변 조건 감지 |
| `Core/SceneLoader.cs` | Additive 씬 전환. `OnSceneWillUnload`/`OnSceneLoaded` 이벤트 노출 |

### 새 씬 추가 3단계

새 "Boss" 씬이 필요하다면:

1. **씬 파일 생성** `Assets/Scenes/BossScene.unity` — **Main Camera/Light 추가 금지**. `BossBoot` GameObject만 추가.
2. **환경 SO 생성** `Assets/Data/SceneEnvironments/BossEnvironment.asset` — Create → Core → Scene Environment, Inspector에서 값 지정
3. **Boot 클래스 작성**
   ```csharp
   public class BossSceneBoot : SceneBootBase
   {
       protected override void OnBoot() { /* 보스 로직 */ }
   }
   ```
   `BossBoot`에 `BossSceneBoot` 부착 + `environment` 필드에 SO 할당

SceneValidator가 Main Camera 중복 같은 실수를 에디터에서 잡아줌.

### 주문 비행 파이프라인

```
ShipInput (드래그 감지)
  → ShipController (통합 조율)
    → ShipModel (중력 적분 + 충돌 판정)
    → ShipVisual (슬링샷 밴드 + 궤적 프리뷰 + 트레일)
    → SlashResolver (히트 시퀀스 → SpellResult)
    → SpellEffectManager → VisualRegistry → ISpellVisual
  → OnShipComplete 이벤트 → PlayerState.NotifySpellCast
```

## 게임 상태 흐름

```
StartRun → [Map] → [Combat] → [Reward] → [Map] → ... → [Boss] → [Victory]
                                                    ↑
                                    HP ≤ 0 → [GameOver]
```
