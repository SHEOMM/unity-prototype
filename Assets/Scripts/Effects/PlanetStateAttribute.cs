using System;

/// <summary>
/// IStarEffect 클래스에 부착하여 해당 효과가 필요로 하는 IPlanetState 타입을 선언한다.
/// PlanetBody 초기화 시 자동으로 해당 상태 컴포넌트를 부착한다.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PlanetStateAttribute : Attribute
{
    public Type StateType { get; }
    public PlanetStateAttribute(Type stateType) { StateType = stateType; }
}
