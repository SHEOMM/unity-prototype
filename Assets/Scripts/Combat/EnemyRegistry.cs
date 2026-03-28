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

    public Enemy GetRandom()
    {
        if (_alive.Count == 0) return null;
        int idx = Random.Range(0, _alive.Count);
        foreach (var e in _alive)
            if (idx-- == 0) return e;
        return null;
    }
}
