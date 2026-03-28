using UnityEngine;

/// <summary>
/// 물병별의 지속 상태. 물이 일정 속도로 차오르며, 꽉 차면 자동으로 비워진다.
/// 슬래시에 포함될 시 현재 수위를 소진하여 데미지에 반영한다.
/// </summary>
public class AquariusState : MonoBehaviour, IPlanetState
{
    [Tooltip("초당 물 충전량")]
    public float fillRate = 0.15f;

    [Tooltip("최대 물 용량")]
    public float maxWater = 1f;

    /// <summary>현재 물 수위 (0 ~ maxWater)</summary>
    public float CurrentWater { get; private set; }

    public void Tick(float deltaTime)
    {
        CurrentWater += fillRate * deltaTime;
        if (CurrentWater >= maxWater)
            CurrentWater = 0f;
    }

    /// <summary>현재 수위를 소진하고 소진량을 반환한다.</summary>
    public float ConsumeWater()
    {
        float amount = CurrentWater;
        CurrentWater = 0f;
        return amount;
    }

    /// <summary>현재 수위의 0~1 비율</summary>
    public float WaterRatio => maxWater > 0f ? CurrentWater / maxWater : 0f;
}
