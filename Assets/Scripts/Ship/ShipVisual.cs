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

    private GameObject _gaugeBg;
    private GameObject _gaugeFill;
    private const float GaugeWidth = 2f;
    private const float GaugeHeight = 0.24f;
    private const float GaugeOffsetY = 0.6f;

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

    private const float MaxChargeWidth = 0.12f;
    private const float BaseChargeWidth = 0.04f;
    private const float ChargeWidthGain = 0.06f;
    private const float EndAlphaScale = 0.4f;       // 비활성 시 끝 알파 = 시작 알파 × 이 값

    /// <summary>
    /// 슬링샷 고무줄을 origin → clampedPullPos로 렌더.
    /// pullRatio(0~1)에 따라 색/폭이 변하고, ratio ≥ 1이면 빨강 + 미세 떨림.
    /// </summary>
    public void ShowSlingshotBand(Vector2 origin, Vector2 clampedPullPos, float pullRatio)
    {
        Vector2 endPoint = clampedPullPos;
        if (pullRatio >= 1f) endPoint += SlingshotJitter();

        var (startCol, endCol, width) = ResolveBandStyle(pullRatio);

        _band.positionCount = 2;
        _band.SetPosition(0, (Vector3)origin);
        _band.SetPosition(1, (Vector3)endPoint);
        _band.startColor = startCol;
        _band.endColor = endCol;
        _band.startWidth = width;
        _band.endWidth = width * 0.5f;
    }

    /// <summary>최대 충전 시 미세 떨림 오프셋 (Perlin 노이즈 기반).</summary>
    static Vector2 SlingshotJitter()
    {
        float freq = GameConstants.VFXAnimation.SlingshotJitterFrequency;
        float amp  = GameConstants.VFXAnimation.SlingshotJitterAmplitude;
        float nx = Mathf.PerlinNoise(Time.time * freq, 0f) - 0.5f;
        float ny = Mathf.PerlinNoise(0f, Time.time * freq) - 0.5f;
        return new Vector2(nx, ny) * amp;
    }

    /// <summary>당김 비율에 따른 (시작색, 끝색, 폭). 최대 충전(=1)이면 빨강 고정 폭.</summary>
    static (Color start, Color end, float width) ResolveBandStyle(float pullRatio)
    {
        if (pullRatio >= 1f)
            return (GameConstants.Colors.SlingshotBandMaxStart,
                    GameConstants.Colors.SlingshotBandMaxEnd,
                    MaxChargeWidth);

        float intensity = Mathf.Clamp01(pullRatio);
        Color c = Color.Lerp(GameConstants.Colors.SlingshotBandIdle,
                             GameConstants.Colors.SlingshotBandCharged, intensity);
        Color end = new Color(c.r, c.g, c.b, c.a * EndAlphaScale);
        float width = BaseChargeWidth + intensity * ChargeWidthGain;
        return (c, end, width);
    }

    public void HideSlingshotBand()
    {
        _band.positionCount = 0;
    }

    /// <summary>
    /// 초기 속도로 TrajectorySimulator를 돌려 궤적 점을 재배치.
    /// 에너지 소진 / 화면 밖 / 행성 착지로 조기 종료된 구간의 점은 비활성화.
    /// </summary>
    public void ShowTrajectoryPreview(Vector2 origin, Vector2 launchVelocity,
        float energy = GameConstants.ShipPhysics.DefaultEnergy)
    {
        int steps = GameConstants.ShipPhysics.TrajectoryPreviewSteps;
        int dotCount = _trajectoryDots.Length;
        int samples = TrajectorySimulator.Simulate(
            origin, launchVelocity,
            energy,
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
            var c = GameConstants.Colors.TrajectoryDot;
            sr.color = new Color(c.r, c.g, c.b, alpha);
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
        sr.color = GameConstants.Colors.ShipBody;
        sr.sortingOrder = GameConstants.SortingOrder.SpellEffect;
        _shipGo.transform.localScale = Vector3.one * 0.15f;
        _shipGo.transform.position = (Vector3)position;

        _trail = _shipGo.AddComponent<TrailRenderer>();
        _trail.startWidth = 0.08f;
        _trail.endWidth = 0.01f;
        _trail.time = 2f;
        _trail.material = GameConstants.SpriteMaterial;
        _trail.startColor = GameConstants.Colors.ShipTrailStart;
        _trail.endColor = GameConstants.Colors.ShipTrailEnd;
        _trail.sortingOrder = GameConstants.SortingOrder.SpellEffect - 1;

        var sprite = UIFactory.MakePixel();

        _gaugeBg = new GameObject("EnergyGaugeBg");
        var bgSr = _gaugeBg.AddComponent<SpriteRenderer>();
        bgSr.sprite = sprite;
        bgSr.color = GameConstants.Colors.EnergyGaugeBg;
        bgSr.sortingOrder = GameConstants.SortingOrder.SpellEffect + 1;
        _gaugeBg.transform.localScale = new Vector3(GaugeWidth, GaugeHeight, 1f);

        _gaugeFill = new GameObject("EnergyGaugeFill");
        var fillSr = _gaugeFill.AddComponent<SpriteRenderer>();
        fillSr.sprite = sprite;
        fillSr.color = GameConstants.Colors.EnergyGaugeHigh;
        fillSr.sortingOrder = GameConstants.SortingOrder.SpellEffect + 2;
        _gaugeFill.transform.localScale = new Vector3(GaugeWidth, GaugeHeight, 1f);
    }

    public void SetTrailEnabled(bool enabled)
    {
        if (_trail != null) _trail.enabled = enabled;
    }

    public void UpdateEnergyGauge(Vector2 shipPos, float ratio)
    {
        if (_gaugeBg == null || _gaugeFill == null) return;
        ratio = Mathf.Clamp01(ratio);

        float gaugeY = shipPos.y + GaugeOffsetY;
        _gaugeBg.transform.position = new Vector3(shipPos.x, gaugeY, 0f);

        float fillWidth = GaugeWidth * ratio;
        float fillX = shipPos.x - GaugeWidth * 0.5f + fillWidth * 0.5f;
        _gaugeFill.transform.position = new Vector3(fillX, gaugeY, 0f);
        _gaugeFill.transform.localScale = new Vector3(fillWidth, GaugeHeight, 1f);

        // 게이지 색상: 0~50% red→yellow, 50~100% yellow→green
        var fillSr = _gaugeFill.GetComponent<SpriteRenderer>();
        if (fillSr != null)
            fillSr.color = ratio > 0.5f
                ? Color.Lerp(GameConstants.Colors.EnergyGaugeMid, GameConstants.Colors.EnergyGaugeHigh, (ratio - 0.5f) * 2f)
                : Color.Lerp(GameConstants.Colors.EnergyGaugeLow, GameConstants.Colors.EnergyGaugeMid, ratio * 2f);
    }

    public void HideEnergyGauge()
    {
        if (_gaugeBg != null) _gaugeBg.SetActive(false);
        if (_gaugeFill != null) _gaugeFill.SetActive(false);
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
        if (_gaugeBg != null) Object.Destroy(_gaugeBg);
        _gaugeBg = null;
        if (_gaugeFill != null) Object.Destroy(_gaugeFill);
        _gaugeFill = null;
    }

    void CreateBand()
    {
        var go = new GameObject("SlingshotBand");
        go.transform.SetParent(transform);
        _band = go.AddComponent<LineRenderer>();
        _band.startWidth = 0.06f;
        _band.endWidth = 0.03f;
        _band.material = GameConstants.SpriteMaterial;
        _band.startColor = GameConstants.Colors.SlingshotBandRest;
        _band.endColor = GameConstants.Colors.SlingshotBandRestEnd;
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
        sr.color = GameConstants.Colors.OriginDot;
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
        var ringCol = GameConstants.Colors.GateRing;
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
            sr.color = GameConstants.Colors.TrajectoryDot;
            sr.sortingOrder = GameConstants.SortingOrder.Label - 1;
            dot.transform.localScale = Vector3.one * 0.06f;
            dot.SetActive(false);
            _trajectoryDots[i] = dot;
        }
    }
}
