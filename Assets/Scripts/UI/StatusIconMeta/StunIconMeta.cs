using UnityEngine;

[StatusIconId("stun")]
public class StunIconMeta : IStatusIconMeta
{
    public Color Color => new Color(1f, 0.95f, 0.3f, 1f); // 노랑
    public string Label => "ST";
}
