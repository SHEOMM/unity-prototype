using UnityEngine;

/// <summary>
/// 우주선 비주얼. 스프라이트 + TrailRenderer + 슬링샷 고무줄 + 궤적 프리뷰 점 풀 + 원점 지속 표시.
/// ShipModel(모델)의 상태를 읽어 표시만 한다.
/// </summary>
public class ShipVisual : MonoBehaviour
{
    private GameObject _shipGo;
    private TrailRenderer _trail;
    private LineRenderer _band;

    // 원점 지속 표시(슬링샷 시트 + 클릭 게이트 링)
    private GameObject _originDot;
    private LineRenderer _gateRing;

    private GameObject[] _trajectoryDots;
    private Vector2[] _trajectoryBuffer;

    public void Initialize()
    {
        CreateBand();
        CreateTrajectoryDots();
        CreateOriginIndicator();
    }

    /// <summary>
    /// 발사 원점과 클릭 게이트 영역을 지속 표시. 전투 시작 / 비행 종료 후 호출.
    /// </summary>
    public void ShowOriginIndicator(Vector2 origin, float gateRadius)
    {
        _originDot.transform.position = (Vector3)origin;
        _originDot.SetActive(true);
        UpdateGateRing(origin, gateRadius);
    }

    public void HideOriginIndicator()
    {
        if (_originDot != null) _originDot.SetActive(false);
        if (_gateRing != null) _gateRing.positionCount = 0;
    }

    /// <summary>
    /// 슬링샷 고무줄을 origin → clampedPullPos로 렌더.
    /// pullRatio(0~1)에 따라 색/폭이 변하고, ratio ≥ 1이면 빨강 + 미세 떨림.
    /// </summary>
    public void ShowSlingshotBand(Vector2 origin, Vector2 clampedPullPos, float pullRatio)
    {
        Vector2 endPoint = clampedPullPos;
        float width;
        Color startCol, endCol;

        if (pullRatio >= 1f)
        {
            float nx = Mathf.PerlinNoise(Time.time * 15f, 0f) - 0.5f;
            float ny = Mathf.PerlinNoise(0f, Time.time * 15f) - 0.5f;
            endPoint += new Vector2(nx, ny) * 0.05f;
            width = 0.12f;
            startCol = new Color(1f, 0.25f, 0.2f, 0.95f);
            endCol = new Color(1f, 0.4f, 0.2f, 0.55f);
        }
        else
        {
            float intensity = Mathf.Clamp01(pullRatio);
            Color c = Color.Lerp(new Color(1f, 1f, 0.7f, 0.35f),
                                 new Color(1f, 0.7f, 0.25f, 0.9f), intensity);
            startCol = c;
            endCol = new Color(c.r, c.g, c.b, c.a * 0.4f);
            width = 0.04f + intensity * 0.06f;
        }

        _band.positionCount = 2;
        _band.SetPosition(0, (Vector3)origin);
        _band.SetPosition(1, (Vector3)endPoint);
        _band.startColor = startCol;
        _band.endColor = endCol;
        _band.startWidth = width;
        _band.endWidth = width * 0.5f;
    }

    public void HideSlingshotBand()
    {
        _band.positionCount = 0;
    }

    /// <summary>
    /// 초기 속도로 TrajectorySimulator를 돌려 궤적 점을 재배치.
    /// 에너지 소진 / 화면 밖으로 조기 종료된 구간의 점은 비활성화.
    /// </summary>
    public void ShowTrajectoryPreview(Vector2 origin, Vector2 launchVelocity)
    {
        int steps = GameConstants.ShipPhysics.TrajectoryPreviewSteps;
        int dotCount = _trajectoryDots.Length;
        int samples = TrajectorySimulator.Simulate(
            origin, launchVelocity,
            GameConstants.ShipPhysics.DefaultEnergy,
            GameConstants.ShipPhysics.DefaultDrag,
            GameConstants.ShipPhysics.DefaultEnergyDrain,
            _trajectoryBuffer, steps,
            GameConstants.ShipPhysics.FixedDt);

        for (int i = 0; i < dotCount; i++)
        {
            int sampleIdx = Mathf.RoundToInt((i + 1) * (steps - 1) / (float)dotCount);
            if (sampleIdx >= samples)
            {
                _trajectoryDots[i].SetActive(false);
                continue;
            }
            var dot = _trajectoryDots[i];
            dot.SetActive(true);
            dot.transform.position = (Vector3)_trajectoryBuffer[sampleIdx];

            var sr = dot.GetComponent<SpriteRenderer>();
            float t = i / (float)(dotCount - 1);
            float alpha = Mathf.Lerp(0.85f, 0.25f, t);
            sr.color = new Color(1f, 0.95f, 0.6f, alpha);
        }
    }

    public void HideTrajectoryPreview()
    {
        if (_trajectoryDots == null) return;
        for (int i = 0; i < _trajectoryDots.Length; i++)
            if (_trajectoryDots[i] != null) _trajectoryDots[i].SetActive(false);
    }

    public void SpawnShip(Vector2 position)
    {
        HideOriginIndicator();

        _shipGo = new GameObject("Ship");
        var sr = _shipGo.AddComponent<SpriteRenderer>();
        sr.sprite = UIFactory.MakePixel();
        sr.color = new Color(1f, 0.9f, 0.5f, 1f);
        sr.sortingOrder = GameConstants.SortingOrder.SpellEffect;
        _shipGo.transform.localScale = Vector3.one * 0.15f;
        _shipGo.transform.position = (Vector3)position;

        _trail = _shipGo.AddComponent<TrailRenderer>();
        _trail.startWidth = 0.08f;
        _trail.endWidth = 0.01f;
        _trail.time = 2f;
        _trail.material = GameConstants.SpriteMaterial;
        _trail.startColor = new Color(1f, 0.8f, 0.3f, 0.8f);
        _trail.endColor = new Color(1f, 0.5f, 0.1f, 0f);
        _trail.sortingOrder = GameConstants.SortingOrder.SpellEffect - 1;
    }

    public void UpdateShipPosition(Vector2 position, Vector2 velocity)
    {
        if (_shipGo == null) return;
        _shipGo.transform.position = (Vector3)position;

        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            _shipGo.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void DestroyShip()
    {
        if (_shipGo != null) Object.Destroy(_shipGo, 0.5f);
        _shipGo = null;
    }

    void CreateBand()
    {
        var go = new GameObject("SlingshotBand");
        go.transform.SetParent(transform);
        _band = go.AddComponent<LineRenderer>();
        _band.startWidth = 0.06f;
        _band.endWidth = 0.03f;
        _band.material = GameConstants.SpriteMaterial;
        _band.startColor = new Color(1f, 0.9f, 0.4f, 0.5f);
        _band.endColor = new Color(1f, 0.9f, 0.4f, 0.15f);
        _band.sortingOrder = GameConstants.SortingOrder.Label;
        _band.positionCount = 0;
    }

    void CreateOriginIndicator()
    {
        // 중앙 점
        _originDot = new GameObject("OriginDot");
        _originDot.transform.SetParent(transform);
        var sr = _originDot.AddComponent<SpriteRenderer>();
        sr.sprite = UIFactory.MakePixel();
        sr.color = new Color(1f, 0.85f, 0.35f, 0.9f);
        sr.sortingOrder = GameConstants.SortingOrder.Label;
        _originDot.transform.localScale = Vector3.one * 0.22f;
        _originDot.SetActive(false);

        // 클릭 게이트 링 (LineRenderer loop로 원형)
        var ringGo = new GameObject("GateRing");
        ringGo.transform.SetParent(transform);
        _gateRing = ringGo.AddComponent<LineRenderer>();
        _gateRing.loop = true;
        _gateRing.useWorldSpace = true;
        _gateRing.startWidth = 0.03f;
        _gateRing.endWidth = 0.03f;
        _gateRing.material = GameConstants.SpriteMaterial;
        var ringCol = new Color(1f, 0.85f, 0.4f, 0.35f);
        _gateRing.startColor = ringCol;
        _gateRing.endColor = ringCol;
        _gateRing.sortingOrder = GameConstants.SortingOrder.Label - 1;
        _gateRing.positionCount = 0;
    }

    void UpdateGateRing(Vector2 center, float radius)
    {
        const int segments = 32;
        _gateRing.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float t = (i / (float)segments) * Mathf.PI * 2f;
            _gateRing.SetPosition(i,
                new Vector3(center.x + Mathf.Cos(t) * radius,
                            center.y + Mathf.Sin(t) * radius, 0f));
        }
    }

    void CreateTrajectoryDots()
    {
        int count = GameConstants.ShipPhysics.TrajectoryPreviewDotCount;
        _trajectoryDots = new GameObject[count];
        _trajectoryBuffer = new Vector2[GameConstants.ShipPhysics.TrajectoryPreviewSteps];

        var sprite = UIFactory.MakePixel();
        for (int i = 0; i < count; i++)
        {
            var dot = new GameObject($"TrajDot_{i}");
            dot.transform.SetParent(transform);
            var sr = dot.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(1f, 0.95f, 0.6f, 0.8f);
            sr.sortingOrder = GameConstants.SortingOrder.Label - 1;
            dot.transform.localScale = Vector3.one * 0.06f;
            dot.SetActive(false);
            _trajectoryDots[i] = dot;
        }
    }
}
