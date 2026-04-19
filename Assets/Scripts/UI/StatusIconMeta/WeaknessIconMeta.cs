using UnityEngine;

[StatusIconId("weakness")]
public class WeaknessIconMeta : IStatusIconMeta
{
    public Color Color => new Color(0.75f, 0.3f, 1f, 1f); // 보라
    public string Label => "WK";
}
