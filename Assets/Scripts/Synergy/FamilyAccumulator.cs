using System.Collections.Generic;

/// <summary>
/// 발사체 비행 동안 SynergyFamily별 접촉 횟수를 누적.
/// SynergyDispatcher가 BeginFlight 시 Reset, OnHit마다 Record.
/// SynergyRuleMatcher가 Count로 임계 조건 판정.
/// </summary>
public class FamilyAccumulator
{
    private readonly Dictionary<SynergyFamily, int> _counts = new Dictionary<SynergyFamily, int>();

    public void Reset() => _counts.Clear();

    public void Record(SynergyFamily family)
    {
        _counts.TryGetValue(family, out int n);
        _counts[family] = n + 1;
    }

    public int Count(SynergyFamily family)
    {
        _counts.TryGetValue(family, out int n);
        return n;
    }

    public int Total
    {
        get
        {
            int sum = 0;
            foreach (var kv in _counts) sum += kv.Value;
            return sum;
        }
    }
}
