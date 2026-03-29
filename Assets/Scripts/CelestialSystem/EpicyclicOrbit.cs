using UnityEngine;

/// <summary>
/// 에피사이클(주전원) 궤도. 여러 원운동을 합성하여 만다라 같은 복잡한 궤적을 생성한다.
/// position(t) = center + Σ rᵢ · (cos(ωᵢt + φᵢ), sin(ωᵢt + φᵢ))
/// </summary>
public class EpicyclicOrbit : MonoBehaviour
{
    private EpicycleTerm[] _terms;
    private Vector2 _center;
    private float _time;

    public void Initialize(EpicycleTerm[] terms, Vector2 center)
    {
        _terms = terms;
        _center = center;

        if (_terms != null && _terms.Length > 0)
            DrawOrbitPath();
    }

    void Update()
    {
        if (_terms == null || _terms.Length == 0) return;

        _time += Time.deltaTime;
        Vector2 pos = CalculatePosition(_time);
        transform.position = new Vector3(pos.x, pos.y, 0);
    }

    Vector2 CalculatePosition(float t)
    {
        Vector2 pos = _center;
        for (int i = 0; i < _terms.Length; i++)
        {
            float rad = (_terms[i].speed * t + _terms[i].phase) * Mathf.Deg2Rad;
            pos.x += _terms[i].radius * Mathf.Cos(rad);
            pos.y += _terms[i].radius * Mathf.Sin(rad);
        }
        return pos;
    }

    void DrawOrbitPath()
    {
        var pathGo = new GameObject("EpicyclicPath");
        pathGo.transform.SetParent(null); // 월드 공간에 독립
        pathGo.transform.position = Vector3.zero;

        var lr = pathGo.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.startWidth = GameConstants.Orbit.PathWidth;
        lr.endWidth = GameConstants.Orbit.PathWidth;
        lr.material = GameConstants.SpriteMaterial;
        lr.startColor = GameConstants.Colors.OrbitPath;
        lr.endColor = GameConstants.Colors.OrbitPath;
        lr.sortingOrder = GameConstants.SortingOrder.OrbitPath;

        // 궤적 한 주기를 계산하여 그림
        float period = CalculateFullPeriod();
        int segments = Mathf.Max(GameConstants.Orbit.PathSegments * 4, 192);
        lr.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments * period;
            Vector2 pos = CalculatePosition(t);
            lr.SetPosition(i, new Vector3(pos.x, pos.y, 0));
        }
    }

    /// <summary>
    /// 모든 항의 속도의 최소공배수에 해당하는 시간(초)을 계산하여 궤적이 닫히는 주기를 구한다.
    /// 정확한 LCM이 아닌 근사: 가장 느린 항의 한 바퀴 시간을 기준으로.
    /// </summary>
    float CalculateFullPeriod()
    {
        float minSpeed = float.MaxValue;
        for (int i = 0; i < _terms.Length; i++)
        {
            float absSpeed = Mathf.Abs(_terms[i].speed);
            if (absSpeed > 0.01f && absSpeed < minSpeed)
                minSpeed = absSpeed;
        }
        // 가장 느린 원이 한 바퀴(360도) 도는 시간
        return minSpeed > 0.01f ? 360f / minSpeed : 10f;
    }
}
