using System;
using UnityEngine;

/// <summary>
/// 씬·룸타입별 배경 매핑 SO. Inspector 편집 가능.
///
/// 런타임:
///   GameManager.Awake가 BackgroundResolver.BindingTable로 주입 → SceneBoot가 Resolve*로 조회.
///
/// 편집:
///   1) Tools/Art/Seed Background Bindings 로 기본 매핑 채움
///   2) Inspector에서 자유 변경
///
/// 조회 우선순위: 해당 key에 엔트리 있으면 그것, 없으면 defaultSpace/defaultGround 폴백.
/// </summary>
[CreateAssetMenu(fileName = "BackgroundBindingTable", menuName = "Data/Background Binding Table")]
public class BackgroundBindingTable : ScriptableObject
{
    [Serializable]
    public struct SpaceEntry
    {
        public BackgroundKey key;
        public SpaceBackgroundSet set;
    }

    [Serializable]
    public struct GroundEntry
    {
        public BackgroundKey key;
        public GroundBackgroundSet set;
    }

    [Header("Space 매핑 (천상 · Map 배경)")]
    public SpaceEntry[] spaceEntries;

    [Header("Ground 매핑 (지상 배경, Combat 계열만 사용)")]
    public GroundEntry[] groundEntries;

    [Header("폴백")]
    [Tooltip("spaceEntries에 해당 key 없을 때 사용.")]
    public SpaceBackgroundSet defaultSpace;
    [Tooltip("groundEntries에 해당 key 없을 때 사용.")]
    public GroundBackgroundSet defaultGround;

    public SpaceBackgroundSet ResolveSpace(BackgroundKey key)
    {
        if (spaceEntries != null)
            foreach (var e in spaceEntries)
                if (e.key == key && e.set != null) return e.set;
        return defaultSpace;
    }

    public GroundBackgroundSet ResolveGround(BackgroundKey key)
    {
        if (groundEntries != null)
            foreach (var e in groundEntries)
                if (e.key == key && e.set != null) return e.set;
        return defaultGround;
    }
}
