using UnityEngine;

/// <summary>
/// 궤도 데이터. Phase 9 천체 시스템에서 행성이 공전하는 고유 궤도를 정의한다.
///
/// 기존 StarSO + OrbitRing 2단 구조를 궤도 하나로 단순화 (별 제거).
/// 플레이어는 스테이지 보상으로 OrbitSO를 수집하고 Cosmos 화면에서 행성에 할당.
///
/// 각 궤도 런타임은 OrbitBody가 담당 — 반경·공전 속도·시작 각도로 행성 1개를 공전시킴.
/// </summary>
[CreateAssetMenu(fileName = "NewOrbit", menuName = "Data/Orbit")]
public class OrbitSO : ScriptableObject, IRewardApplicable
{
    /// <summary>보상 적용 시 궤도를 RunState에 추가하고 미배치 행성이 있으면 자동 부착.</summary>
    public void ApplyAsReward(PlayerState player, RunState run)
    {
        if (run == null) return;
        run.UnlockOrbit(this);
        run.TryAutoAssignPlanetToOrbit(this);
    }

    [Header("기본 정보")]
    public string orbitName;
    [TextArea] public string description;

    [Header("공전 파라미터")]
    [Range(0.5f, 5f)]
    [Tooltip("궤도 반경 (월드 유닛).")]
    public float radius = 1.5f;

    [Range(-180f, 180f)]
    [Tooltip("초당 각속도 (도/초). 음수=시계, 양수=반시계.")]
    public float angularSpeed = 60f;

    [Range(0f, 360f)]
    [Tooltip("초기 배치 각도 (도).")]
    public float startAngle = 0f;

    [Range(0.3f, 1f)]
    [Tooltip("궤도 타원 비율. 1=원, <1=납작.")]
    public float eccentricity = 1f;

    [Header("비주얼")]
    public Color orbitLineColor = new Color(1f, 1f, 1f, 0.25f);
}
