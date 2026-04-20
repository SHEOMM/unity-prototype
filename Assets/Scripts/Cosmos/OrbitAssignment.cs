using System;

/// <summary>
/// 행성↔궤도 배치 1건. RunState.orbitAssignments에 직렬화되어 저장됨.
/// Phase 9 리팩터링(PR2): RunState.cs 말미에 있던 struct를 Cosmos 폴더로 이전.
/// 필드명(orbitName/planetName)·struct 이름·Serializable 속성은 직렬화 키라 변경 금지.
/// </summary>
[Serializable]
public struct OrbitAssignment
{
    public string orbitName;
    public string planetName;
}
