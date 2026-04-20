using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 비행 한 번의 결과. 마법 명령 목록과 히트 행성을 담는다. Ship 파이프라인이 소비.
/// </summary>
public class SpellResult
{
    public List<PlanetBody> hitPlanets = new List<PlanetBody>();
    public List<SpellCommand> commands = new List<SpellCommand>();
    public float totalDamage;
}

/// <summary>
/// 개별 마법 명령. 지상 전투 시스템이 이것을 받아 실행한다.
/// </summary>
[System.Serializable]
public class SpellCommand
{
    public Element element;
    public float damage;
    public int hitCount = 1;
    public string sourceName;
    public SpellVisualType visualType;
    public string visualId;
    public Vector2 targetPosition;
}

public enum SpellVisualType
{
    Strike,
    Projectile,
    AreaOfEffect
}
