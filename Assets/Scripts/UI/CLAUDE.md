# UI 레이어 가이드

**Phase 6 완성 — Observer 기반 View 계층.** 루트 `CLAUDE.md`의 Model-View 규칙 상속.

---

## 부착 규약

| 대상 | Spawner | 자동 부착 View |
|---|---|---|
| Enemy | `EnemySpawner`, `SplitterBehavior` | `EnemyView` + `StatusIconView` |
| AllyUnit | `AllySpawner.SpawnAt` | `AllyView` + `StatusIconView` |
| Structure | `StructureSpawner.SpawnAt` | `StructureView` + `StatusIconView` |
| Player (전역) | `CombatSceneBoot` | `PlayerHPBar`, `PlayerDamageView`, `SynergyToastView`, (VFX) `SynergyVisualHost` |

**규칙**: 모든 전투 개체 View는 Spawner에서 AddComponent로 부착. 개체 스크립트 자체가 View를 생성하지 않음.

---

## UIFactory API (재사용 핵심)

`Assets/Scripts/UI/UIFactory.cs` — World Space TextMesh + SpriteRenderer 기반.

```csharp
TextMesh CreateLabel(Transform parent, string text, float yOffset,
                     float scaleMultiplier, Color color, int sortingOrder);

struct HPBarHandle { SpriteRenderer background, fill; TextMesh text; }
HPBarHandle CreateHPBar(Transform parent, float yOffset, float barWidth);
void UpdateHPBar(HPBarHandle handle, float currentHP, float maxHP);

Sprite MakePixel();  // 1x1 흰 스프라이트. 색상 적용해 색점·오버레이에 재사용
```

HP 바 로직 중복 금지 — `UIFactory`만 수정.

---

## 전투 개체 View 패턴 (EnemyView/AllyView/StructureView 공통)

```csharp
void Start() {
    _model = GetComponent<ModelType>();
    UIFactory.CreateLabel(transform, _model.Data.name, ...);
    _hpBar = UIFactory.CreateHPBar(transform, offset);
    _model.OnDamaged += HandleDamaged;
}
void Update() { UIFactory.UpdateHPBar(_hpBar, _model.currentHP, _model.maxHP); }
void HandleDamaged(float dmg, Element e) { DamagePopup.Spawn(transform.position, dmg, e); }
void OnDestroy() { _model.OnDamaged -= HandleDamaged; }
```

Ally/Structure View는 EnemyView를 그대로 미러링. 공통 추상 베이스 만들지 않음 (3 유사 클래스 < 과도한 generic).

---

## SynergyToastView (Phase 6)

`SynergyDispatcher.OnSynergyFired` 구독 → 큐 기반 순차 토스트.
- 앞 토스트 종료 전까진 다음 펜딩. 여러 시너지 동시 발동해도 겹치지 않음.
- 표시: `SynergyRuleSO.displayName` — 황금색, 1.5초 페이드.
- CombatSceneBoot가 `AddComponent<SynergyToastView>().Bind(combat.SynergyDispatcher)`.

레거시 `SynergyPopup.Show(string)` 정적 API는 그대로 유지 — SlashResolver 경로에서 여전히 사용.

---

## StatusIcon 시스템

| 파일 | 역할 |
|---|---|
| `IStatusIconMeta` | Color + Label (ISP 최소) |
| `[StatusIconId("x")]` + `StatusIconRegistry` | 리플렉션 자동 수집 |
| `StatusIconMeta/*IconMeta.cs` | Stun(ST 노랑) / Weakness(WK 보라) / DoT(빨강) / Slow(SL 시안) |
| `StatusIconView` | 부모의 `IStatusHost.ActiveStatuses` 관찰 → 활성 id별 색점 1개씩 머리 위 렌더 |

**확장**: 새 상태이상 아이콘 = `[StatusIconId("x")] class XIconMeta : IStatusIconMeta` 파일 1개 + `IStatusEffect.IconId => "x"` 1줄. 기존 코드 수정 0.

`IStatusEffect.IconId`는 기본 `=> null` (아이콘 표시 안 함). 시각 피드백이 필요한 상태이상만 override.

---

## 기타 View 파일

| 파일 | 역할 |
|---|---|
| `PlanetLabelView` | PlanetBody에 자동 부착, 이름 라벨 |
| `StarLabelView` | StarSystem에 자동 부착 |
| `AquariusHUD` | 물병별 수위 게이지 (IPlanetHUD 구현체 — DeckManager가 부착) |
| `MapView` | 분기형 맵 시각화 (MapManager.OnMapGenerated/OnNodeSelected 구독) |
| `GravityRangeView` | IGravitySource 반경 원 표시 |
| `CombatDividerView` | 천상/지상 구분선 (CombatManager.Initialize가 호출) |
| `DamagePopup` | 정적 Spawn 메소드로 데미지 팝업 스폰 + 애니메이션 |
| `SlashFeedbackView / ShipFeedbackView` | (레거시) SlashResult.activatedSynergies 순회 후 SynergyPopup 호출 |

---

## 관례

- View는 코루틴·Update 두 가지로 애니메이션. MonoBehaviour 수명은 대상 개체와 동일.
- Canvas UI 미사용 — World Space TextMesh + SpriteRenderer만. `GUI.*` 사용은 `PlayerHPBar`만 예외 (좌상단 HUD).
- DamagePopup은 타입 무관 (`IDamageable` 이벤트만 있으면 재사용).
- 새 Toast 유형이 필요하면 Queue 구조를 별도 뷰로 복제 (중첩 방지 책임 View 본인).
