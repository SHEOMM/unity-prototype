using System.Collections.Generic;

/// <summary>
/// 슬래시 효과 실행에 필요한 컨텍스트.
/// 모든 IStarEffect가 이 객체를 받아 실행한다.
/// </summary>
public class EffectContext
{
    /// <summary>효과를 발동한 행성 런타임</summary>
    public PlanetBody source;

    /// <summary>슬래시 내 순서 (0 = 맨 앞)</summary>
    public int positionIndex;

    /// <summary>전체 관통된 행성 목록 (순서대로)</summary>
    public List<PlanetBody> allHits;

    /// <summary>이 별보다 앞에 있는 별들</summary>
    public List<PlanetBody> leading;

    /// <summary>이 별보다 뒤에 있는 별들</summary>
    public List<PlanetBody> trailing;

    /// <summary>현재 지상의 적들</summary>
    public List<Enemy> enemies;

    /// <summary>위성/시저지에 의한 데미지 배율</summary>
    public float damageMultiplier = 1f;

    /// <summary>위성에 의한 추가 타격 횟수</summary>
    public int extraHits;

    /// <summary>위성에 의한 범위 배율</summary>
    public float areaMultiplier = 1f;

    /// <summary>다른 효과에 의해 재발동된 컨텍스트인가 (재발동 계열 효과의 재귀 방지)</summary>
    public bool isRetriggered;

    /// <summary>위상이 발동되었는가</summary>
    public bool isPhaseActive;

    /// <summary>결과 누적용</summary>
    public SpellResult result;
}
