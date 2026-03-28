using UnityEngine;
using System.Collections;

/// <summary>
/// 웨이브 기반 적 스포너. WaveDefinitionSO에 따라 순차적으로 적을 스폰한다.
/// 웨이브 내 모든 적 전멸 후 다음 웨이브로 진행.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public WaveDefinitionSO[] waves;
    public float spawnY = -3f;
    public float spawnXMin = 7f;
    public float spawnXMax = 10f;
    public float delayBetweenWaves = 5f;

    public System.Action<int> OnWaveStart;
    public System.Action<int> OnWaveComplete;
    public System.Action OnAllWavesComplete;

    public void StartWaves()
    {
        if (waves != null && waves.Length > 0)
            StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        for (int w = 0; w < waves.Length; w++)
        {
            if (waves[w] == null) continue;
            OnWaveStart?.Invoke(w);
            yield return StartCoroutine(SpawnWave(waves[w]));

            // 전멸 대기
            yield return new WaitUntil(() =>
                EnemyRegistry.Instance == null || EnemyRegistry.Instance.GetAll().Count == 0);
            OnWaveComplete?.Invoke(w);
            yield return new WaitForSeconds(delayBetweenWaves);
        }
        OnAllWavesComplete?.Invoke();
    }

    IEnumerator SpawnWave(WaveDefinitionSO wave)
    {
        foreach (var entry in wave.entries)
        {
            if (entry.enemy == null) continue;
            yield return new WaitForSeconds(entry.preDelay);
            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.enemy);
                yield return new WaitForSeconds(entry.spawnInterval);
            }
            yield return new WaitForSeconds(wave.delayBetweenEntries);
        }
    }

    void SpawnEnemy(EnemySO data)
    {
        var go = new GameObject($"Enemy_{data.enemyName}");
        go.transform.position = new Vector3(
            Random.Range(spawnXMin, spawnXMax), spawnY, 0);
        var enemy = go.AddComponent<Enemy>();
        var sprite = EnemySpriteGenerator.GenerateEnemySprite(data);
        enemy.Initialize(data, sprite);
    }
}
