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
}
