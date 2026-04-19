using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아군 유닛 버프. 공격력/이동속도를 일시 배율 적용. PlayerBuff 패턴 그대로.
/// 반환 IDisposable을 Dispose하거나 duration 경과 시 자동 복원.
/// 다중 대상은 ApplyToAll 편의 메소드 사용.
/// </summary>
public static class AllyBuff
{
    public static IDisposable Apply(AllyUnit target, float damageMultiplier, float speedMultiplier, float duration)
    {
        if (target == null) return NullDisposable.Instance;
        var scope = new AllyBuffScope(target, damageMultiplier, speedMultiplier, duration);
        scope.Start();
        return scope;
    }

    public static List<IDisposable> ApplyToAll(IEnumerable<AllyUnit> targets, float damageMultiplier, float speedMultiplier, float duration)
    {
        var list = new List<IDisposable>();
        if (targets == null) return list;
        foreach (var a in targets)
        {
            var scope = Apply(a, damageMultiplier, speedMultiplier, duration);
            if (scope != null) list.Add(scope);
        }
        return list;
    }

    private sealed class AllyBuffScope : IDisposable
    {
        private readonly AllyUnit _target;
        private readonly float _dmgMult;
        private readonly float _spdMult;
        private readonly float _duration;
        private float _origDamage;
        private float _origSpeed;
        private GameObject _host;
        private bool _disposed;

        public AllyBuffScope(AllyUnit target, float dmg, float spd, float duration)
        {
            _target = target;
            _dmgMult = dmg;
            _spdMult = spd;
            _duration = duration;
        }

        public void Start()
        {
            if (_target == null) { _disposed = true; return; }
            _origDamage = _target.attackDamage;
            _origSpeed = _target.moveSpeed;
            _target.attackDamage *= _dmgMult;
            _target.moveSpeed *= _spdMult;

            _host = new GameObject("AllyBuff");
            var driver = _host.AddComponent<TimerDriver>();
            driver.OnFinished = Dispose;
            driver.Begin(_duration);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_target != null)
            {
                _target.attackDamage = _origDamage;
                // moveSpeed는 Update에서 매 프레임 _baseSpeed로 리셋되므로 명시적 복원 불필요.
                // 다만 Dispose 시점이 Update 중이면 남아있을 수 있으므로 안전하게 리셋.
                _target.moveSpeed = _origSpeed;
            }
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
        public Action OnFinished;
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
