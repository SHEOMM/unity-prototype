# 리팩터 큐 — Phase 9 (Orbit/Reward/Cosmos) 작업 결과물

Phase 9a/9b/9c를 단기간에 세 연속 PR로 밀어붙이며 쌓인 기술 부채와 설계 불균형을 정리. 다음 개발 사이클에서 리팩터 대상으로 삼을 항목들.

**범위**: 2026-04-20에 작업된 PR #15 (Phase 9a), #16 (Phase 9b), #17 (Phase 9c)의 산출물만 대상.

---

## 1. 매직 넘버 · 하드코딩된 값

### 1-1. Cosmos 패널 레이아웃 (`Scripts/Cosmos/*`)

동적 레이아웃으로 교체하거나 `CosmosLayoutSO`(ScriptableObject)로 외부화할 후보.

| 파일 | 상수 | 값 | 의미 |
|---|---|---|---|
| `CosmosPanelView.cs` | `PanelWidth` | 14f | 패널 전체 가로 |
| `CosmosPanelView.cs` | `PanelHeight` | 8f | 패널 전체 세로 |
| `CosmosPanelView.cs` | `SlotAreaY` | 1.4f | 상단 슬롯 Y 좌표 |
| `CosmosPanelView.cs` | `SlotSpacing` | 2.2f | 슬롯 간격 |
| `CosmosPanelView.cs` | `InventoryY` | -1.8f | 하단 인벤토리 Y |
| `CosmosPanelView.cs` BuildStaticChildren | 타이틀 Y `+0.4f`, X버튼 `-0.5f/-0.4f/0.6f` | 구조 상수 인라인 |
| `CosmosPanelView.cs` Refresh | 인벤토리 영역 `Vector2(PanelWidth - 1f, 2.2f)` | 매직 패딩 `1f` + 높이 `2.2f` |
| `CosmosOrbitSlot.cs` | `SlotSize` | 1.5f | 슬롯 스프라이트 크기 |
| `CosmosOrbitSlot.cs` Initialize | 이름 라벨 y=`0.55f`, 정규화 반경 `Mathf.Min(radius, 3f) * 0.18f` | 궤도 프리뷰 매직 |
| `CosmosPlanetToken.cs` | `TokenSize` | 0.7f | 토큰 크기 |
| `CosmosPlanetToken.cs` | 이름 라벨 y=`-0.7f`, 아이콘 스케일 `0.85f` | 구성 매직 |
| `CosmosInventoryArea.cs` | `TokenSpacing` | 0.9f | 토큰 간격 |
| `CosmosInventoryArea.cs` Initialize | 라벨 `(-0.43f, 0.4f)`, `font=32` | 라벨 매직 |
| `CosmosMapButton.cs` | `XAnchor=-0.78f`, `YAnchor=0.88f`, `Width=2.4f`, `Height=0.7f` | 앵커 비율 + 치수 |

**권장 리팩터**: `CosmosPanelLayoutSO` (또는 `Data/CosmosStyle.asset`) 도입 — panel/slot/token/inventory 치수·좌표를 Inspector로 편집. 현재는 코드 상수라 아트 튜닝 시 반드시 코드 수정+재컴파일.

### 1-2. 매직 Sorting Order

여러 파일에 분산되어 충돌 위험. `GameConstants.SortingOrder`로 통합해야 함.

| 파일 | Sorting Order | 대상 |
|---|---|---|
| `CosmosPanelView.cs` | 28 (Bg), 44 (Title) | 패널 배경·타이틀 |
| `CosmosPanelView.cs` | 45 (CloseBtn bg), 46 (X text) | 닫기 버튼 |
| `CosmosOrbitSlot.cs` | 30 (Bg), 31 (Preview), 44 (Name) | 슬롯 |
| `CosmosPlanetToken.cs` | 42 (Bg), 43 (Icon), 44 (Name) | 토큰 |
| `CosmosInventoryArea.cs` | 30 (Bg), 44 (Label) | 인벤토리 |
| `CosmosMapButton.cs` | 50 (Bg), 51 (Label) | Cosmos 버튼 |
| `RewardCardView.cs` | 20 (Bg), 21 (Badge/Icon/Preview), 22 (Label) | 보상 카드 |
| `RewardSceneBoot.cs` | 25 (Title) | 보상 씬 타이틀 |

**권장 리팩터**: `GameConstants.SortingOrder`에 `CosmosPanelBg`, `CosmosPanelTitle`, `CosmosSlotBg`, `CosmosTokenBg` 등 상수 추가. 현재는 겹침 확인이 코드 전체 grep 필요.

### 1-3. 전투 난이도 스케일 (`Scripts/Combat/CombatSceneBoot.cs`)

```csharp
const float FloorGrowth = 0.15f;      // 층당 +15%
const float EliteMultiplier = 1.5f;
const float BossMultiplier = 2.0f;
```

그리고 `BuildWaveSet`의 층 구간:
```csharp
if (floor <= 1) target = 1;          // 초반 1 웨이브
else if (floor <= 4) target = 2;      // 중반 2 웨이브
else target = source.Length;          // 후반 전체
```

**문제**: 밸런싱을 데이터로 조정 불가. 웨이브 설계자가 코드 수정해야 함.
**권장 리팩터**: `GameManager.difficultyConfig: DifficultyConfigSO` — SO에 `floorGrowth`, `eliteMult`, `bossMult`, `WaveCountCurve` (층별 웨이브 수 커브) 필드.

### 1-4. 보상 카드 색상 (`Scripts/Core/RewardManager.cs`)

```csharp
static readonly Color OrbitColor  = new Color(0.55f, 0.9f, 1f, 1f);
static readonly Color PlanetColor = new Color(1f, 0.85f, 0.4f, 1f);
static readonly Color RelicColor  = new Color(0.9f, 0.55f, 1f, 1f);
```

**권장**: 타입별 색상은 Theme ScriptableObject로. 시너지 VFX 팔레트(`SynergyVisualElementPalette`)와 통합해 `UIThemePalette.asset` 단일화.

### 1-5. 기타 인라인 값

| 파일 | 값 | 설명 |
|---|---|---|
| `RewardCardView.cs` | `CardWidth=2.6f`, `CardHeight=3.6f`, `IconSize=1.4f` | 카드 치수 |
| `RewardSceneBoot.cs` | `CardSpacing=3f`, `CardsY=0f`, `TitleY=2.2f` | 카드 배치 |
| `RewardSceneBoot.cs` Title 색상 `(1f, 0.9f, 0.5f)` | 카드 타이틀 |
| `OrbitSO` 기본값 | `radius=1.5`, `angularSpeed=60`, `eccentricity=1` | SO 필드 기본 |
| `Assets/Data/Orbits/*.asset` | Inner/Middle/Outer/Swift/Elliptic/Distant의 구체 값 | 데이터는 SO로 되어있음 (OK) |

---

## 2. SOLID 위반 — RunState의 Cosmos 관리

`Scripts/Core/RunState.cs`가 **너무 많은 책임**을 진다. Phase 9 추가분이 대부분 Cosmos 배치 관련.

### 2-1. SRP (Single Responsibility) 위반

**RunState의 본래 책임**: 런의 진행 상태 (floor, act, seed, 덱, 위성).

**Phase 9가 덧붙인 책임** (Cosmos 배치 로직):
```csharp
// 상태 (OK — 데이터 소유)
public List<OrbitSO> unlockedOrbits;
public List<OrbitAssignment> orbitAssignments;

// 순수 조회 (OK — 보조)
public OrbitSO FindOrbitForPlanet(string planetName);
public string FindPlanetForOrbit(string orbitName);
public PlanetSO FindPlanetByName(string planetName);
public List<PlanetSO> GetAssignedPlanets();
public List<PlanetSO> GetUnassignedPlanets();

// 단순 변경 (OK — 데이터 CRUD)
public void UnlockOrbit(OrbitSO orbit);
public void AssignPlanetToOrbit(OrbitSO, PlanetSO);
public void UnassignPlanet(PlanetSO);
public void SwapOrbitAssignments(OrbitSO, OrbitSO);

// ⚠ 의사결정 로직 (문제) — "자동 배치" 규칙은 상태 소유자가 아닌 정책 레이어의 일
public bool TryAutoAssignPlanetToOrbit(OrbitSO orbit);  // 덱에서 "첫 번째 미배치" 선택
public bool TryAutoAssignOrbitToPlanet(PlanetSO planet);
```

**제안 구조** (분리):
- `RunState` → 상태 + 단순 CRUD + 조회만 유지
- `Scripts/Core/Cosmos/CosmosService.cs` (신규) → 자동 배치 정책, 스왑 규칙, 기타 의사결정
- `OrbitAssignment` struct → `Scripts/Data/OrbitAssignment.cs` 파일 분리 (현재 RunState.cs 말미에 있음)

### 2-2. OCP (Open/Closed) 위반 — `TryAutoAssign*`의 "첫 번째 매칭" 정책

```csharp
foreach (var planet in planetDeck)
    if (... && FindOrbitForPlanet(...) == null) { 할당; return true; }
```

현재는 **리스트 순서 첫 번째 미배치 행성**을 맹목적으로 선택. 새 정책(가장 최근 획득한 것 / 강한 것 / 랜덤 등) 추가 시 메소드 본문 수정 필요.

**권장**: `IAutoAssignStrategy` 인터페이스 (`Scripts/Core/Cosmos/`). 구현체 `FirstAvailableStrategy`, `MostRecentStrategy`, `RandomStrategy` 등. `CosmosService`가 전략 주입받아 사용.

### 2-3. OrbitAssignment struct 위치

```csharp
// RunState.cs 말미
[Serializable]
public struct OrbitAssignment {
    public string orbitName;
    public string planetName;
}
```

RunState 파일 안에 숨어있어 검색·재사용이 어려움. `Assets/Scripts/Data/OrbitAssignment.cs`로 분리하고 필요 시 `DefaultAssignmentsAsset` (ScriptableObject) 도입 고려.

### 2-4. DIP — 정적 싱글턴 강결합

`CosmosPanelView.Refresh`, `ResolveDrop` 등이 `RunState.Instance`를 직접 참조. 테스트 가능성 저하.
**권장**: Panel에 `IPlanetAssignmentStore` 같은 인터페이스를 DI. RunState는 구현체.

### 2-5. 책임 중복 — `CelestialBodySO.ApplyAsReward`

```csharp
public virtual void ApplyAsReward(PlayerState player, RunState run) {
    run.AddToDeck(this);
    if (this is PlanetSO planet) run.TryAutoAssignOrbitToPlanet(planet);  // ⚠ 부가 효과
}
```

"덱에 추가"라는 기본 동작에 "자동 배치" 부가효과가 붙어 있어 **AddToDeck = AddToDeck + 자동배치**로 의미가 변함.

**권장**: `RewardManager.Apply`에서 `payload.ApplyAsReward` 호출 후 별도로 `CosmosService.ApplyAutoPlacement(planet)` 명시적 호출. CelestialBodySO는 "덱에만" 추가.

### 2-6. `OrbitSO`가 `IRewardApplicable` 구현

```csharp
public class OrbitSO : ScriptableObject, IRewardApplicable {
    public void ApplyAsReward(PlayerState player, RunState run) {
        run.UnlockOrbit(this);
        run.TryAutoAssignPlanetToOrbit(this);
    }
}
```

데이터 SO가 런타임 상태 변경 메소드를 직접 호출 — 흐름이 거꾸로(SO → RunState)임.
**권장**: `OrbitRewardHandler : IRewardHandler<OrbitSO>` 같은 핸들러 레지스트리. 데이터는 순수 데이터로 유지.

---

## 3. 임시적 구현 / 기술 부채

### 3-1. Reward 풀 — Relic 빈 배열

`PersistentScene`의 `rewardRelicPool: []`. Relic 에셋이 없어 보상 3 선택지에 유물이 등장하지 않음. **Phase 9b 의도된 임시 상태** (plan 문서에 기록됨).

**할 일**: 2~5개의 `RelicSO` 에셋 작성 + 풀 바인딩.

### 3-2. Reward 풀 — 행성 풀에 전체 12개 고정

`PersistentScene`의 `rewardPlanetPool`이 전체 12 PlanetSO를 포함. 덱에 이미 있는 것은 `BuildChoices`에서 필터링되나, Inspector 관리 번거로움.

**할 일**: 풀 관리 자동화 — `PlanetSpriteLibrary` 같은 카탈로그를 확장하거나, `RewardPoolSO` 도입해 티어·등장률 표현.

### 3-3. Reward UI — 키보드/재추첨 없음

- 현재 3 중 1 선택만 가능. "스킵" 버튼 없음 (풀 비었을 때만 자동 스킵).
- 마우스 클릭 전용 — 키보드(1/2/3)로 선택 불가.
- 재추첨 기능 없음.

**할 일**: "건너뛰기" 버튼 추가. 키보드 단축키. 추첨 재시도(코인 소비 등) 고려.

### 3-4. Cosmos Refresh 비효율

`CosmosPanelView.Refresh()`는 모든 슬롯·토큰·인벤토리 영역을 파괴 후 재생성. Hide/Show 때마다 전체 재구성. 작은 N이라 문제 없으나, 행성 100개 이상 확장 시 부담.

**할 일**: 증분 업데이트 — 변경된 토큰/슬롯만 수정. 단 선택적 — 현재 규모로는 필요 없음.

### 3-5. Cosmos 툴팁 · 상세 정보 없음

- 궤도의 radius/angularSpeed를 시각적으로 알려주는 텍스트·아이콘 없음
- 행성 시너지 family/element도 카드에 표시 안 됨
- 호버 시 tooltip 없음

**할 일**: 호버 이벤트 + Tooltip 패널 + 상세 데이터 표시.

### 3-6. Cosmos 드래그 중 시각 피드백 부족

- 드래그 중 토큰이 어디에 드롭 가능한지 하이라이트 없음
- 드롭 불가능 영역 무표시
- 실패 시 단순 스냅백 (애니메이션 X)

**할 일**: Slot/InventoryArea가 드래그 중 인식하면 테두리 glow. 실패 시 shake 애니.

### 3-7. `CosmosDragController.TryStartDrag` 선형 탐색

```csharp
for (int i = tokens.Count - 1; i >= 0; i--)
    if (t.ContainsWorldPoint(world)) { ... }
```

토큰 수가 많아지면 O(n). 토큰 수가 ≤30 정도면 문제 없음.

**할 일**: 공간 분할 자료구조 필요 없음 (스코프 밖). 단, 렌더 순서 = 드래그 우선순위 가정이 암묵적 — 주석만 추가.

### 3-8. Cosmos 버튼 · 패널이 다른 씬(RestScene, ShopScene, RewardScene)에 없음

사용자가 Rest/Shop/Reward 씬에서도 궤도 재배치를 하고 싶을 수 있음. 현재는 Map에서만 접근 가능.

**할 일**: 공통 오버레이 패턴으로 분리 (CosmosPanelPrefab) 후 각 씬에서 재사용. 또는 PersistentScene에 상시 탑재.

### 3-9. MapScene에 PlayerHPBar / 리소스 표시 없음

전투 씬에만 Player HP UI가 붙음. Map에서는 현재 HP/골드 등을 볼 수 없음.

**할 일**: MapSceneBoot에도 PlayerHPBar 등 HUD 부착. 또는 PersistentScene의 상시 HUD 레이어.

### 3-10. `RewardManager.ShowRewards(ScriptableObject[])` 오래된 시그니처

```csharp
public void ShowRewards(ScriptableObject[] pool);  // 사실상 미사용
public List<RewardChoice> BuildChoices(...);        // 신규 사용 경로
public void Apply(RewardChoice choice);
```

구 `ShowRewards`는 BuildChoices + Apply로 대체됐지만 남아있음. 혜성 `ApplyReward(RewardOption)` 경로는 별개로 유지.

**할 일**: 레거시 `ShowRewards(ScriptableObject[])` 제거. 코드 전체 grep 후 미사용 확인.

### 3-11. `OrbitSO.description`, `PlanetSO.description` 미사용

Cosmos 카드/툴팁에 설명 텍스트 표시 기회가 있으나 현재 미활용. 플레이어는 이름만 보고 판단해야 함.

**할 일**: 툴팁(3-5) 구현 시 활용.

### 3-12. 웨이브 1개 정책 — 초반 너무 쉽고 2층부터 급변

```csharp
if (floor <= 1) target = 1;
else if (floor <= 4) target = 2;
```

1-wave → 2-waves 전환이 2층에서 급격. 곡선 튜닝 필요.

**할 일**: `AnimationCurve` 또는 데이터 테이블로 부드러운 증가.

### 3-13. Cosmos 패널 열기 / 닫기 애니메이션 없음

현재는 즉시 SetActive(true/false). Fade/Slide 애니 없음.

**할 일**: 간단한 Scale/Alpha 트윈 (코루틴).

### 3-14. `CosmosPanelView.Hide()`가 모든 자식 파괴

```csharp
foreach (Transform c in transform) Destroy(c.gameObject);
```

캐싱된 상태 없음. Show() 때마다 전체 재구성. 동작은 안정적이나 메모리 할당·재수집 반복.

**할 일**: 필드 객체 풀링 — 첫 Show 시 자식 생성, 이후 Show는 SetActive + Refresh.

### 3-15. `CosmosPanelView.ResolveDrop`의 교환 로직

```csharp
string displacedName = run.FindPlanetForOrbit(targetOrbit.orbitName);
run.AssignPlanetToOrbit(targetOrbit, planet);
if (originOrbit != null && !string.IsNullOrEmpty(displacedName) && displacedName != planet.bodyName) {
    var displacedPlanet = run.FindPlanetByName(displacedName);
    if (displacedPlanet != null)
        run.AssignPlanetToOrbit(originOrbit, displacedPlanet);
}
```

`SwapOrbitAssignments` 헬퍼가 있는데도 Panel에서 직접 조합해 swap 수행. 논리 중복.

**할 일**: `SwapOrbitAssignments`만 호출하도록 단순화. 또는 `CosmosService.Swap(a, b)`로 위임.

### 3-16. `CosmosInventoryArea`가 토큰 레이아웃을 직접 수행

`LayoutTokens(List<CosmosPlanetToken>)` — Panel이 정렬 후 Area에 위임. Area가 순수 드롭 타겟이 아닌 레이아웃 매니저 역할도 함.
**할 일**: Area는 영역/Collider만, 별도 `InventoryLayoutStrategy`로 분리.

### 3-17. 스케일 보정 트릭 반복

```csharp
labelGo.transform.localScale = new Vector3(1f / size.x, 1f / size.y, 1f);
```

거의 모든 컴포넌트에서 부모 스케일을 상쇄하려는 역스케일 로직 존재 (Slot/InventoryArea/MapButton/CloseButton). 실수 유발 가능성 높음.

**할 일**: `UIFactory.CreateScaleCompensatedChild(parent, size)` 유틸 추가. 또는 Canvas RectTransform 기반 UI로 점진 전환 검토.

### 3-18. `CosmosMapButton.Bind()` → Start() 순서 의존성

```csharp
public void Bind(CosmosPanelView panel) { _panel = panel; }
void Start() { ... ApplyCameraAnchoredPosition(); }
```

Bind가 Start 전 호출된다는 가정. `MapSceneBoot`에서 AddComponent 후 즉시 Bind하므로 현재는 OK이나 취약.
**할 일**: 생성자 아닌 Init/Awake 시점 명시 + 테스트.

### 3-19. SynergyDefinitionSO 레거시 미정리

Phase 3 이후 `[Obsolete]`이지만 PersistentScene에 여전히 4개 바인딩 (`f7dbafc6`, `d1ea27d8`, `8f806f15`, `2e8b247f`). SlashResolver 레거시 경로에서만 사용.

**할 일**: Slash 레거시를 SynergyDispatcher로 완전 이관 후 SynergyDefinitionSO 삭제.

### 3-20. Cosmos 패널에 저장 명시성 없음

현재는 RunState 직접 변경 + RunState 자체가 영속(DontDestroyOnLoad). 명시적 "저장" 액션 없음.
**할 일**: 진행 상황 localFile 직렬화 시 RunState를 통째로 serialize — Phase 10+ 범위.

---

## 4. 권장 리팩터 순서 (우선순위)

| 우선순위 | 항목 | 이유 |
|---|---|---|
| P0 | 2-1, 2-5, 2-6 (RunState SRP + 자동 배치 분리) | Phase 10+ 기능 확장 시 코드 응집도 확보 |
| P1 | 1-1, 1-2, 1-3 (매직 상수 → SO/GameConstants) | 밸런싱 반복 작업 비용 절감 |
| P1 | 3-15, 3-16, 3-17 (Cosmos 패널 내부 정리) | 드래그 로직 안정성 |
| P2 | 3-1, 3-5, 3-6 (Reward/Cosmos UI 완성도) | 체감 품질 |
| P2 | 3-10, 3-19 (레거시 API/에셋 제거) | 코드베이스 명료성 |
| P3 | 3-14 (Panel 풀링), 3-2 (풀 자동화) | 성능·운영 부담 |
| P3 | 3-13 (애니메이션), 3-8, 3-9 (HUD 확장) | 폴리싱 |

---

## 5. 리팩터 시 유지해야 할 계약

리팩터 전후에 **동작 보존**을 확인해야 할 API:
- `RewardManager.OnRewardChosen` 이벤트 — RewardSceneBoot이 구독
- `CombatManager.OnCombatComplete` 이벤트 — CombatSceneBoot이 구독
- `SynergyDispatcher.OnSynergyFired` — Toast·VisualHost 2 Observer
- `GameManager.EnterPhase` — 씬 전환 통합 경로
- `RunState.orbitAssignments`의 serialize 가능성 — 구조체 필드 변경 시 기존 바인딩 깨짐
- `PlanetSO.bodyName` / `OrbitSO.orbitName`을 **고유 키로 사용** — 이름 변경 시 PersistentScene의 `defaultAssignments` 갱신 필요

---

## 6. 미해결 설계 질문 (Phase 10+ 착수 전 결정 필요)

- Cosmos 자동 배치 정책은 결국 어떤 전략이 표준인가? (최초 순서 / 가장 최근 / 수동만)
- Reward pool을 Run 중 점진 확장할지 (조우한 적·노드 타입에 따라 풀 변경)
- 궤도 수 vs 행성 수 불균형을 어떤 방식으로 안내할지 (빈 궤도 강조? 행성 초과 시 버림?)
- 레거시 Slash 시스템 완전 제거 시점
