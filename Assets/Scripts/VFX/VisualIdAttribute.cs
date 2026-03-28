using System;

/// <summary>
/// ISpellVisual 구현 클래스에 부착하여 visualId를 선언한다.
/// VisualRegistry가 리플렉션으로 자동 등록한다.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class VisualIdAttribute : Attribute
{
    public string Id { get; }
    public VisualIdAttribute(string id) { Id = id; }
}
