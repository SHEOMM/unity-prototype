using System;

/// <summary>
/// IStatusIconMeta 구현체에 붙이는 id. StatusIconRegistry가 리플렉션으로 수집.
/// IStatusEffect.IconId와 매칭되어 대응 아이콘 meta를 조회.
/// 예: [StatusIconId("stun")] class StunIconMeta : IStatusIconMeta { ... }
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StatusIconIdAttribute : Attribute
{
    public string Id { get; }
    public StatusIconIdAttribute(string id) { Id = id; }
}
