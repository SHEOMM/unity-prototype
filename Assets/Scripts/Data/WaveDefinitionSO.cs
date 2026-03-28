using UnityEngine;

/// <summary>
/// 웨이브 정의. 적 종류, 수, 스폰 간격을 설정한다.
/// </summary>
[CreateAssetMenu(fileName = "NewWave", menuName = "Combat/Wave")]
public class WaveDefinitionSO : ScriptableObject
{
    public string waveName;
    public WaveEntry[] entries;
    public float delayBetweenEntries = 0.5f;
}

[System.Serializable]
public class WaveEntry
{
    public EnemySO enemy;
    public int count = 1;
    [Tooltip("이 엔트리 스폰 전 대기 시간")]
    public float preDelay = 0f;
    [Tooltip("개체 간 스폰 간격")]
    public float spawnInterval = 0.5f;
}
