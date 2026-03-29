using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 원형 스코프 컨트롤러. 마우스 추적 → 클릭 시 스냅샷 → 효과 해석 → 발동.
/// SlashController의 원형 버전.
/// </summary>
public class ScopeController : MonoBehaviour
{
    public static ScopeController Instance { get; private set; }

    [Tooltip("스냅샷 쿨타임 (초)")]
    public float cooldownTime = 1.5f;

    private ScopeInput _input;
    private ScopeDetector _detector;
    private SlashResolver _resolver;
    private ScopeVisual _visual;
    private SpellEffectManager _spellFx;

    private bool _active;
    private float _cooldownTimer;

    public System.Action<SlashResult> OnScopeComplete;
    public System.Action<List<CometBody>> OnCometsHit;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void Initialize(ScopeInput input, ScopeDetector detector, SlashResolver resolver, ScopeVisual visual, SpellEffectManager spellFx)
    {
        _input = input;
        _detector = detector;
        _resolver = resolver;
        _visual = visual;
        _spellFx = spellFx;
        _visual.Initialize(detector.scopeRadius);
    }

    public void Activate()
    {
        if (_active || _input == null) return;
        _active = true;
        _input.OnCursorMove += OnCursorMove;
        _input.OnSnapshot += OnSnapshot;
    }

    public void Deactivate()
    {
        if (!_active || _input == null) return;
        _active = false;
        _input.OnCursorMove -= OnCursorMove;
        _input.OnSnapshot -= OnSnapshot;
        _visual.Hide();
        _detector.ClearHighlights();
    }

    void Update()
    {
        if (_cooldownTimer > 0)
            _cooldownTimer -= Time.deltaTime;
    }

    bool IsOnCooldown => _cooldownTimer > 0;
    float CooldownRatio => IsOnCooldown ? _cooldownTimer / cooldownTime : 0f;

    void OnCursorMove(Vector2 pos)
    {
        var hits = _detector.DetectPlanets(pos);
        _visual.Show(pos, hits.Count, IsOnCooldown, CooldownRatio);
        _detector.HighlightInScope(pos);
    }

    void OnSnapshot(Vector2 center)
    {
        if (IsOnCooldown) return;

        _cooldownTimer = cooldownTime;
        _detector.ClearHighlights();

        // 혜성 포착
        var comets = _detector.DetectComets(center);
        if (comets.Count > 0)
        {
            foreach (var comet in comets)
                comet.Capture();
            OnCometsHit?.Invoke(comets);
        }

        // 행성 효과 — SlashResolver를 그대로 재사용
        var hits = _detector.DetectPlanets(center);
        if (hits.Count == 0) return;

        var result = _resolver.Resolve(hits);
        _spellFx.ExecuteSpells(result);
        OnScopeComplete?.Invoke(result);
    }
}
