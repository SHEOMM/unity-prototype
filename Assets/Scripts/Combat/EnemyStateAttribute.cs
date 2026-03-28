using System;

/// <summary>
/// IEnemyBehavior 클래스에 부착하여 필요한 IEnemyState 타입을 선언한다.
/// Enemy 초기화 시 자동으로 해당 상태 컴포넌트를 부착한다.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EnemyStateAttribute : Attribute
{
    public Type StateType { get; }
    public EnemyStateAttribute(Type stateType) { StateType = stateType; }
}
