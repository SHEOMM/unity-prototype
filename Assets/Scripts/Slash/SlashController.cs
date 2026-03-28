using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 슬래시 입력→판정→해석→비주얼을 통합 관리하는 컨트롤러.
/// CombatManager에서 입력 처리 로직을 분리.
/// </summary>
public class SlashController : MonoBehaviour
{
    public static SlashController Instance { get; private set; }

    private SlashInput _input;
    private SlashDetector _detector;
    private SlashResolver _resolver;
    private SlashVisual _visual;
    private SpellEffectManager _spellFx;

    private Vector2 _dragStart;
    private bool _active;

    public System.Action<SlashResult> OnSlashComplete;
    public System.Action<List<CometBody>> OnCometsHit;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void Initialize(SlashInput input, SlashDetector detector, SlashResolver resolver, SlashVisual visual, SpellEffectManager spellFx)
    {
        _input = input;
        _detector = detector;
        _resolver = resolver;
        _visual = visual;
        _spellFx = spellFx;
    }

    public void Activate()
    {
        if (_active || _input == null) return;
        _active = true;
        _input.OnDragStart += OnDragStart;
        _input.OnDragUpdate += OnDragUpdate;
        _input.OnDragEnd += OnDragEnd;
    }

    public void Deactivate()
    {
        if (!_active || _input == null) return;
        _active = false;
        _input.OnDragStart -= OnDragStart;
        _input.OnDragUpdate -= OnDragUpdate;
        _input.OnDragEnd -= OnDragEnd;
    }

    void OnDragStart(Vector2 pos) { _dragStart = pos; }

    void OnDragUpdate(Vector2 current)
    {
        var hits = _detector.DetectHits(_dragStart, current);
        _visual.ShowLine(_dragStart, current, hits.Count);
        _detector.HighlightHits(_dragStart, current);
    }

    void OnDragEnd(Vector2 start, Vector2 end)
    {
        _visual.HideLine();
        _detector.ClearHighlights();

        // 혜성 포착
        var comets = _detector.DetectComets(start, end);
        if (comets.Count > 0)
        {
            foreach (var comet in comets)
                comet.Capture();
            OnCometsHit?.Invoke(comets);
        }

        // 행성 효과
        var hits = _detector.DetectHits(start, end);
        if (hits.Count == 0) return;

        var result = _resolver.Resolve(hits);
        _spellFx.ExecuteSpells(result);
        OnSlashComplete?.Invoke(result);
    }
}
