using UnityEngine;

/// <summary>
/// 에피사이클 궤도의 한 항. 여러 항을 합성하면 만다라 같은 복잡한 궤적이 생성된다.
/// position(t) = Σ radius · (cos(speed·t + phase), sin(speed·t + phase))
/// </summary>
[System.Serializable]
public class EpicycleTerm
{
    [Tooltip("이 원의 반지름")]
    public float radius = 1f;

    [Tooltip("각속도 (도/초)")]
    public float speed = 30f;

    [Tooltip("초기 위상 (도)")]
    public float phase = 0f;
}
