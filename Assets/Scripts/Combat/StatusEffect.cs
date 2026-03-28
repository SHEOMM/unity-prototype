using UnityEngine;

/// <summary>
/// 적에게 적용되는 상태이상. 독, 둔화 등.
/// Enemy의 Update에서 틱된다.
/// </summary>
[System.Serializable]
public class StatusEffect
{
    public StatusType type;
    public float duration;
    public float tickInterval;
    public float value;

    private float _elapsed;
    private float _tickTimer;

    public bool IsExpired => _elapsed >= duration;

    public void Tick(Enemy target, float dt)
    {
        _elapsed += dt;
        _tickTimer += dt;

        switch (type)
        {
            case StatusType.Poison:
                if (_tickTimer >= tickInterval)
                {
                    _tickTimer = 0f;
                    target.TakeDamage(value);
                }
                break;
            case StatusType.Slow:
                target.moveSpeed *= (1f - value);
                break;
        }
    }
}

public enum StatusType
{
    Poison,     // 지속 데미지
    Slow,       // 이동 둔화
    Charm       // 매혹 (미래 확장용)
}
