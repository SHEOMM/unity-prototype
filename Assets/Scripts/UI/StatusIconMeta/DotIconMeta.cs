using UnityEngine;

[StatusIconId("dot")]
public class DotIconMeta : IStatusIconMeta
{
    public Color Color => new Color(1f, 0.3f, 0.3f, 1f); // 빨강
    public string Label => "DoT";
}
