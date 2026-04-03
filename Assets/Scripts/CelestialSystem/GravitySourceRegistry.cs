using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 모든 중력원(행성+항성) 추적. Zero-alloc 설계.
/// IReadOnlyList로 반환하여 호출마다 리스트 복사를 피한다.
/// </summary>
public class GravitySourceRegistry : MonoBehaviour
{
    public static GravitySourceRegistry Instance { get; private set; }

    private readonly List<IGravitySource> _sources = new List<IGravitySource>(32);

    public IReadOnlyList<IGravitySource> Sources => _sources;
    public int Count => _sources.Count;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void Register(IGravitySource s) => _sources.Add(s);
    public void Unregister(IGravitySource s) => _sources.Remove(s);
}
