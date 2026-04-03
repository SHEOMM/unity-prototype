using UnityEngine;

/// <summary>
/// 우주선 비주얼. 스프라이트 + TrailRenderer + 조준선.
/// ShipModel(모델)의 상태를 읽어 표시만 한다.
/// </summary>
public class ShipVisual : MonoBehaviour
{
    private GameObject _shipGo;
    private SpriteRenderer _shipSr;
    private TrailRenderer _trail;
    private LineRenderer _aimLine;
    private bool _flying;

    public void Initialize()
    {
        CreateAimLine();
    }

    public void ShowAimLine(Vector2 start, Vector2 end)
    {
        _aimLine.positionCount = 2;
        _aimLine.SetPosition(0, (Vector3)start);
        _aimLine.SetPosition(1, (Vector3)end);
    }

    public void HideAimLine()
    {
        _aimLine.positionCount = 0;
    }

    public void SpawnShip(Vector2 position)
    {
        _flying = true;
        _shipGo = new GameObject("Ship");
        _shipSr = _shipGo.AddComponent<SpriteRenderer>();
        _shipSr.sprite = UIFactory.MakePixel();
        _shipSr.color = new Color(1f, 0.9f, 0.5f, 1f);
        _shipSr.sortingOrder = GameConstants.SortingOrder.SpellEffect;
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
        _flying = false;
        if (_shipGo != null) Object.Destroy(_shipGo, 0.5f);
        _shipGo = null;
    }

    void CreateAimLine()
    {
        var go = new GameObject("AimLine");
        go.transform.SetParent(transform);
        _aimLine = go.AddComponent<LineRenderer>();
        _aimLine.startWidth = 0.05f;
        _aimLine.endWidth = 0.03f;
        _aimLine.material = GameConstants.SpriteMaterial;
        _aimLine.startColor = new Color(1f, 1f, 1f, 0.4f);
        _aimLine.endColor = new Color(1f, 1f, 1f, 0.1f);
        _aimLine.sortingOrder = GameConstants.SortingOrder.Label;
        _aimLine.positionCount = 0;
    }
}
