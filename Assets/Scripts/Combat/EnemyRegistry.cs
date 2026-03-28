using UnityEngine;
using System.Collections.Generic;

public class EnemyRegistry : MonoBehaviour
{
    public static EnemyRegistry Instance { get; private set; }

    private readonly HashSet<Enemy> _alive = new HashSet<Enemy>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(Enemy e) => _alive.Add(e);
    public void Unregister(Enemy e) => _alive.Remove(e);

    public List<Enemy> GetAll() => new List<Enemy>(_alive);

    public List<Enemy> GetNearby(Vector3 position, float radius)
    {
        var result = new List<Enemy>();
        foreach (var e in _alive)
            if (e != null && Vector3.Distance(e.transform.position, position) <= radius)
                result.Add(e);
        return result;
    }

    public Enemy GetRandom()
    {
        if (_alive.Count == 0) return null;
        int idx = Random.Range(0, _alive.Count);
        foreach (var e in _alive)
            if (idx-- == 0) return e;
        return null;
    }
}
