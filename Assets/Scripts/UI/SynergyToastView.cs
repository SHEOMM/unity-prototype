using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시너지 발동 알림 토스트. SynergyDispatcher.OnSynergyFired 이벤트 구독 →
/// 큐에 쌓고 순차 재생 (중첩 방지).
///
/// Observer 패턴: Dispatcher는 UI를 모름. 구독자가 0개여도 Dispatcher 동작 무결성.
/// </summary>
public class SynergyToastView : MonoBehaviour
{
    private SynergyDispatcher _dispatcher;
    private readonly Queue<SynergyRuleSO> _pending = new Queue<SynergyRuleSO>();
    private GameObject _currentGo;
    private TextMesh _currentText;
    private float _elapsed;
    private const float Lifetime = 1.5f;
    private const float RiseSpeed = 0.3f;

    public void Bind(SynergyDispatcher dispatcher)
    {
        Unbind();
        _dispatcher = dispatcher;
        if (_dispatcher != null) _dispatcher.OnSynergyFired += HandleFired;
    }

    void Unbind()
    {
        if (_dispatcher != null) _dispatcher.OnSynergyFired -= HandleFired;
        _dispatcher = null;
    }

    void OnDestroy() { Unbind(); }

    void HandleFired(SynergyRuleSO rule, SynergyContext ctx)
    {
        if (rule == null) return;
        _pending.Enqueue(rule);
    }

    void Update()
    {
        if (_currentGo == null && _pending.Count > 0)
        {
            SpawnNext(_pending.Dequeue());
            return;
        }
        if (_currentGo == null) return;

        _elapsed += Time.deltaTime;
        if (_elapsed >= Lifetime) { Destroy(_currentGo); _currentGo = null; return; }

        float t = _elapsed / Lifetime;
        float scale = t < 0.2f ? Mathf.Lerp(0.1f, 0.22f, t / 0.2f) : 0.22f;
        _currentGo.transform.localScale = Vector3.one * scale;
        _currentGo.transform.position += Vector3.up * RiseSpeed * Time.deltaTime;

        if (_currentText != null)
        {
            float alpha = t < 0.7f ? 1f : 1f - ((t - 0.7f) / 0.3f);
            var c = _currentText.color;
            _currentText.color = new Color(c.r, c.g, c.b, alpha);
        }
    }

    void SpawnNext(SynergyRuleSO rule)
    {
        _elapsed = 0f;
        var cam = CameraService.Instance?.Camera;
        float y = cam != null ? cam.transform.position.y + cam.orthographicSize * 0.7f : 5f;

        _currentGo = new GameObject("SynergyToast");
        _currentGo.transform.position = new Vector3(0, y, 0);
        _currentGo.transform.localScale = Vector3.one * 0.1f;

        _currentText = _currentGo.AddComponent<TextMesh>();
        string label = !string.IsNullOrEmpty(rule.displayName) ? rule.displayName : rule.synergyEffectId;
        _currentText.text = label + " !";
        _currentText.fontSize = 48;
        _currentText.anchor = TextAnchor.MiddleCenter;
        _currentText.alignment = TextAlignment.Center;
        _currentText.color = new Color(1f, 0.9f, 0.3f, 1f);
        _currentText.characterSize = 0.1f;

        var mr = _currentGo.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = GameConstants.SortingOrder.SynergyToast;
    }
}
