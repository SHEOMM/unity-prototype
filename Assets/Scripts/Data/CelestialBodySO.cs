using UnityEngine;

/// <summary>
/// 천체 공통 베이스 ScriptableObject.
/// 항성, 행성, 위성, 혜성 모두 이것을 상속한다.
/// </summary>
public abstract class CelestialBodySO : ScriptableObject
{
    [Header("기본 정보")]
    public string bodyName;
    [TextArea] public string description;
    public Sprite icon;
    public Color bodyColor = Color.white;

    [Header("속성")]
    public Element element = Element.None;

    [Header("키워드 (시저지용)")]
    public string[] keywords;
}
