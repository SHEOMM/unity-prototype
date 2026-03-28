using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnInterval = 2.5f;
    public float spawnY = -3f;
    public float spawnXMin = 7f;
    public float spawnXMax = 10f;
    private float _timer;
    private Sprite _sprite;

    void Start() { _sprite = Resources.Load<Sprite>("EnemySprite"); }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < spawnInterval) return;
        _timer = 0f;
        var go = new GameObject("Enemy");
        go.transform.position = new Vector3(Random.Range(spawnXMin, spawnXMax), spawnY, 0);
        go.transform.localScale = Vector3.one * 0.6f;
        var enemy = go.AddComponent<Enemy>();
        enemy.Initialize(Random.Range(60f, 160f), Random.Range(0.5f, 1.5f), _sprite);
    }
}
