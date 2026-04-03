using UnityEngine;

/// <summary>
/// 우주선 비주얼. 스프라이트 + TrailRenderer + 조준선 + 발사 원점 마커.
/// ShipModel(모델)의 상태를 읽어 표시만 한다.
/// </summary>
public class ShipVisual : MonoBehaviour
{
    private GameObject _shipGo;
    private TrailRenderer _trail;
    private LineRenderer _aimLine;
    private GameObject _launchMarker;

    public void Initialize()
    {
        CreateAimLine();
    }

    /// <summary>발사 원점 마커를 생성/표시한다.</summary>
    public void ShowLaunchMarker(Vector2 origin)
    {
        if (_launchMarker == null)
        {
            _launchMarker = new GameObject("LaunchMarker");
            var sr = _launchMarker.AddComponent<SpriteRenderer>();
            sr.sprite = UIFactory.MakePixel();
            sr.color = new Color(1f, 0.9f, 0.4f, 0.6f);
            sr.sortingOrder = GameConstants.SortingOrder.Label;
            _launchMarker.transform.localScale = Vector3.one * 0.2f;
        }
        _launchMarker.transform.position = (Vector3)origin;
        _launchMarker.SetActive(true);
    }

    public void HideLaunchMarker()
    {
        if (_launchMarker != null) _launchMarker.SetActive(false);
    }

    public void ShowAimLine(Vector2 start, Vector2 end)
    {
        _aimLine.positionCount = 2;
        _aimLine.SetPosition(0, (Vector3)start);
        _aimLine.SetPosition(1, (Vector3)end);

        // 드래그 거리에 따라 색상 강도 변화
        float dist = Vector2.Distance(start, end);
        float intensity = Mathf.Clamp01(dist * 0.3f);
        Color c = Color.Lerp(new Color(1f, 1f, 1f, 0.2f), new Color(1f, 0.8f, 0.3f, 0.8f), intensity);
        _aimLine.startColor = c;
        _aimLine.endColor = new Color(c.r, c.g, c.b, c.a * 0.3f);
        _aimLine.startWidth = 0.04f + intensity * 0.06f;
        _aimLine.endWidth = 0.02f;
    }

    public void HideAimLine()
    {
        _aimLine.positionCount = 0;
    }

    public void SpawnShip(Vector2 position)
    {
        HideLaunchMarker();

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

    void CreateAimLine()
    {
        var go = new GameObject("AimLine");
        go.transform.SetParent(transform);
        _aimLine = go.AddComponent<LineRenderer>();
        _aimLine.startWidth = 0.06f;
        _aimLine.endWidth = 0.03f;
        _aimLine.material = GameConstants.SpriteMaterial;
        _aimLine.startColor = new Color(1f, 0.9f, 0.4f, 0.5f);
        _aimLine.endColor = new Color(1f, 0.9f, 0.4f, 0.15f);
        _aimLine.sortingOrder = GameConstants.SortingOrder.Label;
        _aimLine.positionCount = 0;
    }
}
