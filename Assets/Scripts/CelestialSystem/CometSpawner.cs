using UnityEngine;

/// <summary>
/// 혜성을 간헐적으로 스폰한다.
/// </summary>
public class CometSpawner : MonoBehaviour
{
    public CometSO[] possibleComets;
    public float minInterval = 15f;
    public float maxInterval = 30f;
    public float skyYMin = 1f;
    public float skyYMax = 5f;

    private float _timer;
    private float _nextSpawn;
    private Sprite _sprite;

    public System.Action<CometBody> OnCometCaptured;

    void Start()
    {
        _sprite = Resources.Load<Sprite>("StarSprite");
        _nextSpawn = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        if (possibleComets == null || possibleComets.Length == 0) return;

        _timer += Time.deltaTime;
        if (_timer < _nextSpawn) return;

        _timer = 0f;
        _nextSpawn = Random.Range(minInterval, maxInterval);
        SpawnComet();
    }

    void SpawnComet()
    {
        var data = possibleComets[Random.Range(0, possibleComets.Length)];
        float y1 = Random.Range(skyYMin, skyYMax);
        float y2 = Random.Range(skyYMin, skyYMax);

        // 왼→오 또는 오→왼 랜덤
        bool leftToRight = Random.value > 0.5f;
        Vector2 start = new Vector2(leftToRight ? -8f : 8f, y1);
        Vector2 end = new Vector2(leftToRight ? 8f : -8f, y2);

        var go = new GameObject("Comet_" + data.bodyName);
        var comet = go.AddComponent<CometBody>();
        comet.Initialize(data, _sprite, start, end);
        comet.OnCaptured += c => OnCometCaptured?.Invoke(c);
    }
}
