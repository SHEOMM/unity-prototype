using UnityEngine;

/// <summary>
/// 궤도 런타임. Phase 9에서 StarSystem+OrbitRing 2단 구조를 대체한 단일 궤도 엔티티.
///
/// 책임:
/// - OrbitSO 데이터를 받아 자기 transform 주변을 공전하는 1개 행성을 관리
/// - LineRenderer로 궤도선 시각화 (기존 OrbitRing.DrawOrbitPath 로직 계승)
/// - PlacePlanet/RemovePlanet으로 점유 상태 제어
///
/// 부모는 StarSystem이 아닌 씬 루트. center는 월드 좌표로 Initialize 시 주입.
/// </summary>
public class OrbitBody : MonoBehaviour
{
    public OrbitSO Data { get; private set; }
    public PlanetBody Occupant { get; private set; }

    private float _angle;

    public bool IsEmpty => Occupant == null;

    public void Initialize(OrbitSO data, Vector2 center)
    {
        Data = data;
        _angle = data.startAngle;
        transform.position = new Vector3(center.x, center.y, 0);
        gameObject.name = "Orbit_" + data.orbitName;
        DrawOrbitPath();
    }

    public void PlacePlanet(PlanetBody planet)
    {
        if (planet == null || !IsEmpty) return;
        Occupant = planet;
        Occupant.transform.SetParent(transform);
        // 시작 위치 즉시 계산 (프레임 대기 없이)
        UpdateOccupantPosition();
    }

    public void RemovePlanet()
    {
        if (Occupant == null) return;
        Occupant.transform.SetParent(null);
        Occupant = null;
    }

    void Update()
    {
        if (Occupant == null || Data == null) return;
        _angle += Data.angularSpeed * Time.deltaTime;
        UpdateOccupantPosition();
    }

    void UpdateOccupantPosition()
    {
        float rad = _angle * Mathf.Deg2Rad;
        Occupant.transform.localPosition = new Vector3(
            Mathf.Cos(rad) * Data.radius,
            Mathf.Sin(rad) * Data.radius * Data.eccentricity,
            0);
    }

    void DrawOrbitPath()
    {
        var pathGo = new GameObject("OrbitPath");
        pathGo.transform.SetParent(transform);
        pathGo.transform.localPosition = Vector3.zero;
        pathGo.transform.localScale = Vector3.one;

        var lr = pathGo.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.startWidth = GameConstants.Orbit.PathWidth;
        lr.endWidth = GameConstants.Orbit.PathWidth;
        lr.material = GameConstants.SpriteMaterial;
        lr.startColor = Data.orbitLineColor;
        lr.endColor = Data.orbitLineColor;
        lr.sortingOrder = GameConstants.SortingOrder.OrbitPath;

        int segments = GameConstants.Orbit.PathSegments;
        lr.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Cos(t) * Data.radius;
            float y = Mathf.Sin(t) * Data.radius * Data.eccentricity;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
