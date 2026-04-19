using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 살아있는 아군 유닛 레지스트리. AllyUnit이 OnEnable/OnDisable에서 등록/해제.
/// Phase 2에서는 EnemyRegistry와 동일한 구조로 최소 API만 제공. 패턴 중복은 SRP를 위한 의도적 선택.
/// </summary>
public class AllyRegistry : MonoBehaviour
{
    public static AllyRegistry Instance { get; private set; }

    private readonly HashSet<AllyUnit> _alive = new HashSet<AllyUnit>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(AllyUnit a) => _alive.Add(a);
    public void Unregister(AllyUnit a) => _alive.Remove(a);

    public List<AllyUnit> GetAll() => new List<AllyUnit>(_alive);

    public List<AllyUnit> GetNearby(Vector3 position, float radius)
    {
        var result = new List<AllyUnit>();
        foreach (var a in _alive)
            if (a != null && Vector3.Distance(a.transform.position, position) <= radius)
                result.Add(a);
        return result;
    }

    /// <summary>전투 종료 시 CombatManager가 호출해 모든 아군을 파괴.</summary>
    public void DestroyAll()
    {
        var snapshot = new List<AllyUnit>(_alive);
        foreach (var a in snapshot)
            if (a != null) Destroy(a.gameObject);
        _alive.Clear();
    }
}
