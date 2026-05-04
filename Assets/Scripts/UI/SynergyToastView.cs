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
        if (_elapsed >= GameConstants.SynergyToast.Lifetime) { Destroy(_currentGo); _currentGo = null; return; }

        float t = _elapsed / GameConstants.SynergyToast.Lifetime;
        float scaleUpEnd = GameConstants.SynergyToast.ScaleUpEnd;
        float scaleEnd = GameConstants.SynergyToast.ScaleEnd;
        float scale = t < scaleUpEnd
            ? Mathf.Lerp(GameConstants.SynergyToast.ScaleStart, scaleEnd, t / scaleUpEnd)
            : scaleEnd;
        _currentGo.transform.localScale = Vector3.one * scale;
        _currentGo.transform.position += Vector3.up * GameConstants.SynergyToast.RiseSpeed * Time.deltaTime;

        if (_currentText != null)
        {
            float fadeStart = GameConstants.SynergyToast.FadeOutStart;
            float alpha = t < fadeStart
                ? 1f
                : 1f - ((t - fadeStart) / GameConstants.SynergyToast.FadeOutWindow);
            var c = _currentText.color;
            _currentText.color = new Color(c.r, c.g, c.b, alpha);
        }
    }

    void SpawnNext(SynergyRuleSO rule)
    {
        _elapsed = 0f;
        var cam = CameraService.Instance?.Camera;
        float y = cam != null
            ? cam.transform.position.y + cam.orthographicSize * GameConstants.SynergyToast.CameraYRatio
            : 5f;

        _currentGo = new GameObject("SynergyToast");
        _currentGo.transform.position = new Vector3(0, y, 0);
        _currentGo.transform.localScale = Vector3.one * GameConstants.SynergyToast.ScaleStart;

        _currentText = _currentGo.AddComponent<TextMesh>();
        string label = !string.IsNullOrEmpty(rule.displayName) ? rule.displayName : rule.synergyEffectId;
        _currentText.text = label + " !";
        _currentText.fontSize = GameConstants.SynergyToast.FontSize;
        _currentText.anchor = TextAnchor.MiddleCenter;
        _currentText.alignment = TextAlignment.Center;
        _currentText.color = GameConstants.Colors.SynergyToastText;
        _currentText.characterSize = GameConstants.SynergyToast.CharacterSize;

        var mr = _currentGo.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = GameConstants.SortingOrder.SynergyToast;
    }
}
