using UnityEngine;

/// <summary>
/// 행성 스프라이트 애니메이션 재생기. PlanetAnimationClip의 frames 배열을 순차 루프.
///
/// 성능 원칙:
///  - frames 배열은 clip SO 소유 → 인스턴스 N개 있어도 공유 참조만 유지 (메모리 중복 0)
///  - 매 프레임: float 덧셈 + FloorToInt + 비교 1회 (동일 프레임이면 early-out, sprite 교체 없음)
///  - sprite 교체만 1/fps마다 1회 발생 (10 FPS 기본 → 0.1초당 1회, 포인터 할당만)
///  - timeOffset 랜덤화로 모든 행성이 같은 순간에 프레임 교체하지 않게 분산 (스파이크 회피)
///
/// 수명: PlanetBody.Initialize가 clip 존재 시 AddComponent + Play 호출.
/// 비활성 GameObject는 Update 자체가 불리지 않아 0 비용.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlanetAnimator : MonoBehaviour
{
    SpriteRenderer _sr;
    Sprite[] _frames;
    float _interval;    // 1 / fps
    float _elapsed;
    int _currentFrame = -1;

    public void Play(PlanetAnimationClip clip)
    {
        if (clip == null || clip.frames == null || clip.frames.Length == 0)
        {
            enabled = false;
            return;
        }

        _sr = GetComponent<SpriteRenderer>();
        _frames = clip.frames;
        _interval = 1f / Mathf.Max(0.1f, clip.fps);

        // 위상 분산: 인스턴스마다 시작 시점을 무작위로 배치해 모든 행성이
        // 같은 프레임 경계에 정렬되지 않도록 한다 (GC 스파이크·시각 동기화 회피).
        _elapsed = Random.Range(0f, _interval * _frames.Length);

        // 즉시 첫 프레임 설정 (Update 첫 호출 이전에도 정지 이미지 안정)
        _currentFrame = Mathf.FloorToInt(_elapsed / _interval) % _frames.Length;
        _sr.sprite = _frames[_currentFrame];
    }

    void Update()
    {
        // 단일 프레임 또는 초기화 전: Update 자체 무의미
        if (_frames == null || _frames.Length < 2) return;

        _elapsed += Time.deltaTime;
        int next = Mathf.FloorToInt(_elapsed / _interval) % _frames.Length;
        if (next == _currentFrame) return;  // 같은 프레임이면 early-out (대부분의 틱)
        _currentFrame = next;
        _sr.sprite = _frames[next];
    }
}
