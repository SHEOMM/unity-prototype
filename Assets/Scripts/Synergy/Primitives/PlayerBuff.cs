using System;
using UnityEngine;

/// <summary>
/// 플레이어에게 임시 속성 데미지 보너스를 부여. 반환 IDisposable을 Dispose하거나
/// duration 경과 시 자동 복원.
///
/// 내부적으로 GameObject를 만들어 코루틴 타이머 유지 (MonoBehaviour 호스트 없이 Dispose 복원).
/// </summary>
public static class PlayerBuff
{
    public static IDisposable Apply(Element element, float bonus, float duration)
    {
        if (PlayerState.Instance == null) return NullDisposable.Instance;
        PlayerState.Instance.AddElementBonus(element, bonus);
        var scope = new PlayerBuffScope(element, bonus, duration);
        scope.Start();
        return scope;
    }

    private sealed class PlayerBuffScope : IDisposable
    {
        private readonly Element _element;
        private readonly float _bonus;
        private readonly float _duration;
        private GameObject _host;
        private bool _disposed;

        public PlayerBuffScope(Element element, float bonus, float duration)
        {
            _element = element;
            _bonus = bonus;
            _duration = duration;
        }

        public void Start()
        {
            _host = new GameObject("PlayerBuff(" + _element + ")");
            var driver = _host.AddComponent<TimerDriver>();
            driver.OnFinished = Dispose;
            driver.Begin(_duration);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (PlayerState.Instance != null) PlayerState.Instance.AddElementBonus(_element, -_bonus);
            if (_host != null) UnityEngine.Object.Destroy(_host);
        }
    }

    private sealed class NullDisposable : IDisposable
    {
        public static readonly NullDisposable Instance = new NullDisposable();
        public void Dispose() { }
    }

    private sealed class TimerDriver : MonoBehaviour
    {
        public System.Action OnFinished;
        private float _remaining;
        public void Begin(float duration) { _remaining = duration; }
        void Update()
        {
            _remaining -= Time.deltaTime;
            if (_remaining <= 0f)
            {
                OnFinished?.Invoke();
                OnFinished = null;
            }
        }
    }
}
