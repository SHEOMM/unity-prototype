# 시너지 시스템 가이드

**Phase 0-7 구현 완료 — 37 시너지 × 4 trigger type.** 루트 `CLAUDE.md`의 핵심 원칙 상속.

---

## 3층 구조

### 데이터: `SynergyRuleSO` (ScriptableObject)
"언제 발동할지" + 파라미터를 담은 에셋. `Assets/Data/Synergies/` 배치.

**발동 조건** (triggerType별 다른 필드 사용):
- `FamilyAccumulation` → `family` + `threshold`
- `SequencePosition` → `positionKey` (Leading/Trailing/Any) + `planetKey`
- `PlanetCombo` → `sequenceKeys[]` (모두 포함 매칭)
- `PerHitPlanet` → `planetKey` (현재 터치 행성 매칭)

**공통 파라미터** (모든 effect가 `ctx.CurrentRule`에서 해석):
- `damage`, `radius`, `duration`, `secondary`, `count`, `element`
- `spawnArea` (Rect), `spawnCount`
- `allyToSpawn` (AllySO 레퍼런스), `structureToSpawn` (StructureSO)
- `visualId` (Phase 7 VFX 키), `icon` (Phase 6 Toast 아이콘 옵셔널)

`planetKey` 매칭 규칙: `PlanetSO.bodyName` 또는 `keywords[]` 중 하나와 문자열 일치.

### 로직: `ISynergyEffect` (stateless)
```csharp
void OnHit(SynergyContext ctx);       // PerHitPlanet 전용
void OnFlightEnd(SynergyContext ctx); // 그 외
```
`SynergyEffectBase` 상속 + 필요한 훅만 override. `[SynergyId("x")]` 부착 → `SynergyRegistry`가 자동 수집. 매 호출마다 `Activator.CreateInstance`로 새 인스턴스 생성되므로 필드 상태 금지.

### 조율자: `SynergyDispatcher` (MonoBehaviour)
`ShipController.OnPlanetHit / OnFlightEnded` 구독 후 규칙 순회:

| Trigger | 발동 시점 | 특이 규칙 |
|---|---|---|
| `FamilyAccumulation` | end-of-flight | **Highest-only** — family별 임계 충족한 rule 중 최고 threshold 1개만 발동 |
| `SequencePosition` | end-of-flight | 매칭되는 모두 발동 |
| `PlanetCombo` | end-of-flight | 매칭되는 모두 발동 |
| `PerHitPlanet` | 매 hit 즉시 | `OnHit` 훅 발화 |

effect 호출 직후 **`OnSynergyFired(SynergyRuleSO, SynergyContext)` 이벤트 발화** — UI/VFX/로깅 Observer 구독점.

---

## Primitive 라이브러리 (`Primitives/`)

모두 인터페이스 타겟 (IDamageable/IStatusHost/IMoveable). 새 시너지는 조합만.

| Primitive | 시그니처 요약 |
|---|---|
| `AoeApplicator.Damage` | (center, radius, damage, element) → int hits |
| `ChainLightning.Chain` | (origin, jumps, damage, jumpRadius, element) → int hits |
| `SweepLine.Sweep` | (start, end, width, damage, element, piercing, knockbackStrength) |
| `DotApplicator.Apply` | (IStatusHost, damagePerTick, tickInterval, duration, element) |
| `SlowApplicator.Apply` | (IStatusHost, slowFactor, duration) |
| `StunApplicator.Apply` | (IStatusHost, duration) |
| `WeaknessApplicator.Apply` | (IStatusHost, amplifier, duration) |
| `KnockbackApplicator.Apply` | (IMoveable, direction, strength) |
| `ExecuteApplicator.TryExecute` | (IDamageable, hpRatio) → bool |
| `AllySpawner.Spawn / SpawnAt` | AllySO 기반 AllyUnit 생성 + View 부착 |
| `StructureSpawner.Spawn / SpawnAt` | StructureSO 기반 Structure 생성 + View 부착 |
| `PlayerBuff.Apply` | (Element, bonus, duration) → IDisposable |
| `AllyBuff.Apply / ApplyToAll` | (AllyUnit, dmgMult, spdMult, duration) → IDisposable |
| `AllyHealApplicator.HealAll` | (amount) → int healed |
| `EnemyFiltering.GetFlying / GetGround` | IEnumerable<Enemy> |

---

## SynergyFamily enum

`Fire / Water / Wind / Earth / Lightning / War / Dark / Civilization` — Element(피해 속성)와 별개 축. `PlanetSO.synergyFamily`로 지정.

---

## 구현된 37 시너지 카탈로그

### FamilyAccumulation 24개 (`Effects/{Family}/`)
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

### SequencePosition 6개 (`Effects/Position/`)
| ID | positionKey | planetKey | 효과 |
|---|---|---|---|
| pos_leading_warrior | Leading | valor | 전역 Fire AoE |
| pos_leading_emperor | Leading | authority | 타깃 AoE + Stun |
| pos_trailing_storm | Trailing | storm | ChainLightning 4회 |
| pos_trailing_scorpion | Trailing | venom | 전체 적 DoT |
| pos_trailing_assassin | Trailing | shadow | HP 30% 이하 전체 처형 |
| pos_any_love | Any | heart | 전체 아군 Heal |

### PlanetCombo 4개 (`Effects/Combo/`)
| ID | sequenceKeys | 효과 |
|---|---|---|
| combo_ocean_blessing | tide + depth + heart | 전체 아군 Heal + 전체 적 Slow |
| combo_warrior_storm | blade + valor + storm | 전역 Fire AoE + ChainLightning 5회 |
| combo_shadow_hunt | shadow + venom | 전체 적 DoT + HP 30% 처형 |
| combo_wind_herald | swift + herald + marksman | 전체 적 넉백 + Player Wind Buff |

### PerHitPlanet 3개 (`Effects/PerHit/`, OnHit)
| ID | planetKey | 효과 |
|---|---|---|
| hit_archer_volley | marksman | 터치 지점 주변 AoE |
| hit_messenger_boost | herald | Player Wind Buff 4초 |
| hit_smith_reinforce | forge | 전체 아군 dmg×1.2 3초 |

---

## 확장 방법

### 새 시너지 효과
1. `Effects/{Category}/MyNewSynergy.cs`:
   ```csharp
   [SynergyId("my_new")]
   public class MyNewSynergy : SynergyEffectBase {
       public override void OnFlightEnd(SynergyContext ctx) {
           var rule = ctx.CurrentRule;
           // primitive 조합
       }
   }
   ```
2. `Assets/Data/Synergies/` 아래 SynergyRuleSO 에셋 생성 (`synergyEffectId="my_new"`).
3. `PersistentScene.unity`의 `GameManager.synergyRules` 배열에 GUID 추가.

### 새 Trigger Type
1. `SynergyTriggerType` enum에 엔트리 추가.
2. `SynergyRuleMatcher`에 `MatchX` 메소드 + switch case.
3. `SynergyDispatcher`의 `HandlePlanetHit` 또는 `HandleFlightEnded`에 발동 분기 추가.

### 새 Primitive
`Primitives/` 아래 정적 클래스 추가. 인터페이스 타겟 (구체 타입 인자 금지).

---

## 연관 레이어

- **UI 피드백**: `OnSynergyFired` 이벤트를 `SynergyToastView`(Phase 6) + `SynergyVisualHost`(Phase 7)가 독립 구독. 상세는 `Scripts/UI/CLAUDE.md` / `Scripts/VFX/CLAUDE.md`.
- **에셋 바인딩**: 새 rule 에셋은 반드시 `PersistentScene.unity`의 `GameManager.synergyRules` 리스트에 추가해야 CombatManager가 Dispatcher에 주입.
- **전투 정리**: 전투 종료 시 Ally/Structure Registry는 `DestroyAll()` 호출됨 (combat-scoped).
