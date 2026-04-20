using UnityEngine;

/// <summary>
/// 전체 배경 세트 카탈로그. Inspector 미리보기·에디터 도구 iterate용.
/// BackgroundSetGenerator가 자동 갱신한다.
///
/// 매핑(Key↔Set)은 BackgroundBindingTable이 담당 — 관심사 분리.
/// </summary>
[CreateAssetMenu(fileName = "BackgroundLibrary", menuName = "Data/Background Library")]
public class BackgroundLibrary : ScriptableObject
{
    public SpaceBackgroundSet[] spaceSets;
    public GroundBackgroundSet[] groundSets;

    public SpaceBackgroundSet FindSpace(string name)
    {
        if (string.IsNullOrEmpty(name) || spaceSets == null) return null;
        foreach (var s in spaceSets)
            if (s != null && s.setName == name) return s;
        return null;
    }

    public GroundBackgroundSet FindGround(string name)
    {
        if (string.IsNullOrEmpty(name) || groundSets == null) return null;
        foreach (var g in groundSets)
            if (g != null && g.setName == name) return g;
        return null;
    }
}
