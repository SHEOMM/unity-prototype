# VFX 레이어 가이드

**2계통 병존**: `ISpellVisual`(Ship 발사 결과의 개별 행성 이펙트) + `ISynergyVisual`(시너지 공용 이펙트). Registry와 Attribute가 서로 **독립** — 네임 충돌 없음.

---

## 계통 비교

| 항목 | ISpellVisual (행성) | ISynergyVisual (시너지) |
|---|---|---|
| 계약 | `IEnumerator Play(SpellVisualContext)` | `IEnumerator Play(SynergyVisualContext)` |
| Context | `SpellCommand + Enemy target + targetPosition + elementColor + vfxRoot` | `SynergyRuleSO Rule + SynergyContext + Anchor + ElementColor` |
| Attribute | `[VisualId("x")]` | `[SynergyVisualId("x")]` |
| Registry | `VisualRegistry` (폴백=DefaultVisual) | `SynergyVisualRegistry` (폴백=null, Host가 "default" 해석) |
| 호출자 | `SpellEffectManager` (SpellResolver 경로) | `SynergyVisualHost` (Dispatcher.OnSynergyFired 구독) |
| 용도 | 개별 행성/마법마다 고유 이펙트 | 공용 6종 × rule 데이터 주입으로 37 시너지 표현 |

---

## ISpellVisual (행성용)

`PlanetSO.visualId` → `VisualRegistry.Get(id)` → `StartCoroutine(Play)`. 구현체 예: `DefaultVisual`, `ArcherVisual`, `AquariusVisual`, `EmperorVisual`. 추가 시 `[VisualId("new")]` + 파일 1개.

---

## ISynergyVisual (Phase 7, 시너지용)

### 구조 (`Synergy/` 폴더의 공용 6개)
| visualId | 용도 | rule 파라미터 해석 |
|---|---|---|
| `default` | 폴백 | 작은 원형 펄스 0.3초 |
| `area_pulse` | AoE 계열 | `radius`=최대반경, `element`=색상 |
| `chain` | 체인/번개 | `count`=세그먼트 수, `radius`=세그먼트 길이 |
| `sweep` | 가로 파동 | `radius`×2=폭, anchor.y=중심 |
| `spawn_burst` | Ally/Structure 소환 | `spawnArea`=영역, `spawnCount`=펄스 수 |
| `screen_flash` | 궁극기 | 전체 화면 오버레이 + `CameraService.Shake` |

### Host (`SynergyVisualHost`)
`CombatSceneBoot`가 `AddComponent<SynergyVisualHost>().Bind(combat.SynergyDispatcher)`.
책임:
1. `OnSynergyFired(rule, ctx)` 수신
2. **Anchor Vector3 계산**: PerHitPlanet → `ctx.CurrentPlanet.transform.position`, 그 외 → 첫 살아있는 Enemy 또는 `Vector3.zero`
3. `SynergyVisualElementPalette.Resolve(rule.element)` → Color
4. `SynergyVisualRegistry.Get(rule.visualId ?? "default")` → visual 인스턴스
5. `StartCoroutine(visual.Play(ctx))`

Visual 자체는 anchor만 받아 그림 — "타깃 랜덤 적"인지 여부 무지식.

### Palette (`SynergyVisualElementPalette`)
```csharp
Fire → red     Water → blue      Wind → cyan
Earth → brown  Darkness → purple None → gold
```
6 visual이 모두 참조 → 색상 중복 0.

---

## CameraService.Shake (Phase 7에서 추가)

```csharp
CameraService.Instance?.Shake(strength, duration);
// Perlin 노이즈 기반 오프셋 후 원위치 복원. 코루틴 단일 인스턴스 (중복 호출 시 이전 취소).
```
주 사용처: `ScreenFlashVisual`. 다른 visual도 자유롭게 호출 가능.

---

## 확장 방법

### 새 공용 Visual 템플릿 (공용으로 재사용될 것)
1. `Synergy/MyVisual.cs`:
   ```csharp
   [SynergyVisualId("my_visual")]
   public class MyVisual : ISynergyVisual {
       public IEnumerator Play(SynergyVisualContext ctx) {
           // ctx.Anchor 에서 ctx.Rule.radius/count 기반 렌더
           // ctx.ElementColor 사용
           // LineRenderer/SpriteRenderer 생성 → 애니 → Destroy
       }
   }
   ```
2. 해당 visualId를 사용할 rule들 `SynergyRuleSO.visualId` 에 `"my_visual"` 주입.

### 시너지마다 고유 Visual (드물게)
`[SynergyVisualId("unique_for_this")]` + rule 하나에만 주입. 공용 6개로 충분하지 않을 때만.

### 새 행성 Visual
`[VisualId("x")]` + `ISpellVisual` 구현. `PlanetSO.visualId = "x"`.

---

## 레이어 독립성

- Toast (Scripts/UI/SynergyToastView) 와 VisualHost 모두 `Dispatcher.OnSynergyFired`를 **독립 구독**. Observer 2명이 서로 무지.
- SpellEffectManager(Ship 발사 결과의 행성별 이펙트) 와 SynergyVisualHost(시너지 이펙트)는 서로 다른 호출 경로. 공통 VFX 스케줄러는 후속 Phase 후보.

---

## 기타 VFX 파일

| 파일 | 역할 |
|---|---|
| `CelestialSpriteGenerator` | 행성/항성 절차적 스프라이트 생성 |
| `EnemySpriteGenerator` | 적 절차적 스프라이트 |
| `DefaultVisual` | ISpellVisual 폴백 (VisualRegistry 내장) |
| `ArcherVisual`, `AquariusVisual`, `EmperorVisual` | 행성 고유 visual 예 |
