using UnityEngine;

/// <summary>
/// 상태이상 UI 표시용 메타데이터 계약. 색상 + 2~3자 라벨.
/// 실제 스프라이트 에셋 대신 UIFactory.MakePixel() + 색상으로 간이 아이콘을 그린다.
/// ISP: View에 필요한 최소 표면만 노출.
/// </summary>
public interface IStatusIconMeta
{
    Color Color { get; }
    string Label { get; }
}
