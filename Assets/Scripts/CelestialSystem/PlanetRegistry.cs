using UnityEngine;
using System.Collections.Generic;

public class PlanetRegistry : MonoBehaviour
{
    public static PlanetRegistry Instance { get; private set; }

    private readonly HashSet<PlanetBody> _all = new HashSet<PlanetBody>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(PlanetBody p) => _all.Add(p);
    public void Unregister(PlanetBody p) => _all.Remove(p);

    public List<PlanetBody> GetAll() => new List<PlanetBody>(_all);
}
