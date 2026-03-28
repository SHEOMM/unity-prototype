using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 슬래시 한 번의 결과. 마법 명령 목록과 시저지 정보를 담는다.
/// </summary>
public class SlashResult
{
    public List<PlanetBody> hitPlanets = new List<PlanetBody>();
    public List<SpellCommand> commands = new List<SpellCommand>();
    public List<SynergyDefinitionSO> activatedSynergies = new List<SynergyDefinitionSO>();
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
