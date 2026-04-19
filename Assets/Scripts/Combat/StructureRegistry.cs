using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 살아있는 구조물 레지스트리. Structure가 OnEnable/OnDisable에서 등록/해제.
/// </summary>
public class StructureRegistry : MonoBehaviour
{
    public static StructureRegistry Instance { get; private set; }

    private readonly HashSet<Structure> _alive = new HashSet<Structure>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(Structure s) => _alive.Add(s);
    public void Unregister(Structure s) => _alive.Remove(s);

    public List<Structure> GetAll() => new List<Structure>(_alive);

    public List<Structure> GetNearby(Vector3 position, float radius)
    {
        var result = new List<Structure>();
        foreach (var s in _alive)
            if (s != null && Vector3.Distance(s.transform.position, position) <= radius)
                result.Add(s);
        return result;
    }

    public void DestroyAll()
    {
        var snapshot = new List<Structure>(_alive);
        foreach (var s in snapshot)
            if (s != null) Destroy(s.gameObject);
        _alive.Clear();
    }
}
