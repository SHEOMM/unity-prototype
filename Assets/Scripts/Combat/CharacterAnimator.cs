using UnityEngine;

/// <summary>
/// 캐릭터 스프라이트 애니메이션 재생기. 상태 머신(예: PlayerCharacterView)이 Play(clip) 호출로 전환.
///
/// 성능 원칙 (PlanetAnimator와 동일):
///  - frames[] 공유 참조 → 인스턴스별 메모리 중복 0
///  - Update: float += + FloorToInt + 비교 (같은 프레임이면 early-out)
///  - sprite 교체만 1/fps마다 1회, GC 0
///
/// loop=false면 마지막 프레임에서 클램프 정지.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class CharacterAnimator : MonoBehaviour
{
    SpriteRenderer _sr;
    CharacterAnimationClip _clip;
    Sprite[] _frames;
    float _interval;
    float _elapsed;
    int _currentFrame = -1;

    public CharacterAnimationClip CurrentClip => _clip;

    /// <summary>동일 clip이 이미 재생 중이면 재시작하지 않음 (끊김 방지).</summary>
    public void Play(CharacterAnimationClip clip)
    {
        if (clip == null) { enabled = false; return; }
        if (_clip == clip) return;  // 같은 clip 재지정 시 유지

        _clip = clip;
        _sr = _sr != null ? _sr : GetComponent<SpriteRenderer>();
        _frames = clip.frames;
        _interval = 1f / Mathf.Max(0.1f, clip.fps);
        _elapsed = 0f;
        _currentFrame = 0;

        if (_frames != null && _frames.Length > 0)
            _sr.sprite = _frames[0];

        enabled = _frames != null && _frames.Length >= 2;
    }

    void Update()
    {
        if (_frames == null || _frames.Length < 2) return;

        _elapsed += Time.deltaTime;
        int raw = Mathf.FloorToInt(_elapsed / _interval);
        int next;
        if (_clip.loop)
        {
            next = raw % _frames.Length;
        }
        else
        {
            next = raw < _frames.Length ? raw : _frames.Length - 1;
        }

        if (next == _currentFrame) return;
        _currentFrame = next;
        _sr.sprite = _frames[next];
    }
}
