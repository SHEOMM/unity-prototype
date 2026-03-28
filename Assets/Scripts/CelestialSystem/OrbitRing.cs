using UnityEngine;

public class OrbitRing : MonoBehaviour
{
    public OrbitDefinition definition;
    public PlanetBody occupant;
    private float _angle;

    public void Initialize(OrbitDefinition def)
    {
        definition = def;
        _angle = def.startAngle;
        DrawOrbitPath(def);
    }

    public void AssignPlanet(PlanetBody planet)
    {
        occupant = planet;
        occupant.transform.SetParent(transform);
    }

    void Update()
    {
        if (occupant == null || definition == null) return;
        float dir = definition.clockwise ? -1f : 1f;
        _angle += definition.angularSpeed * dir * Time.deltaTime;
        float rad = _angle * Mathf.Deg2Rad;
        occupant.transform.localPosition = new Vector3(
            Mathf.Cos(rad) * definition.radius,
            Mathf.Sin(rad) * definition.radius * definition.eccentricity,
            0
        );
    }

    void DrawOrbitPath(OrbitDefinition def)
    {
        var pathGo = new GameObject("OrbitPath");
        pathGo.transform.SetParent(transform);
        pathGo.transform.localPosition = Vector3.zero;

        // 부모(StarSystem) 스케일 상쇄
        float parentScale = transform.lossyScale.x;
        float invScale = parentScale > 0.01f ? 1f / parentScale : 1f;
        pathGo.transform.localScale = Vector3.one * invScale;

        var lr = pathGo.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.startWidth = GameConstants.Orbit.PathWidth;
        lr.endWidth = GameConstants.Orbit.PathWidth;
        lr.material = GameConstants.SpriteMaterial;
        lr.startColor = GameConstants.Colors.OrbitPath;
        lr.endColor = GameConstants.Colors.OrbitPath;
        lr.sortingOrder = GameConstants.SortingOrder.OrbitPath;

        int segments = GameConstants.Orbit.PathSegments;
        lr.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Cos(t) * def.radius * parentScale;
            float y = Mathf.Sin(t) * def.radius * def.eccentricity * parentScale;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
