using UnityEngine;

[StatusIconId("slow")]
public class SlowIconMeta : IStatusIconMeta
{
    public Color Color => new Color(0.3f, 0.85f, 1f, 1f); // 시안
    public string Label => "SL";
}
