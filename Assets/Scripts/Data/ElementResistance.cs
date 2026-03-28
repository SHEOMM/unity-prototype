/// <summary>
/// 속성별 데미지 배율. 1.0=보통, 0.5=내성, 1.5=약점, 0=면역.
/// </summary>
[System.Serializable]
public struct ElementResistance
{
    public Element element;
    public float multiplier;
}

/// <summary>
/// 적 외형 타입. EnemySpriteGenerator에서 사용.
/// </summary>
public enum EnemyShape
{
    Square,     // 졸개 - 기본 사각형
    Hexagon,    // 중갑병 - 육각형
    Triangle,   // 벌떼 - 삼각형
    Circle,     // 치유사 - 원형
    Diamond,    // 방패병 - 다이아몬드
    Blob,       // 분열체 - 불규칙 형태
    Crystal,    // 속성체 - 결정형
    Crown       // 우두머리 - 왕관형
}
