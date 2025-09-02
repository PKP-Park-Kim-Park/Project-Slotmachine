using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���Ըӽ� �ɺ����� ���� �����͸� �����ϴ� Ŭ����.
/// </summary>
public class SymbolData
{
    // �ɺ� ID�� ������ �����ϴ� ��ųʸ�
    private Dictionary<int, float> symbolOdds = new Dictionary<int, float>();

    public SymbolData()
    {
        // �����ڿ��� �ɺ� ���� �����͸� �ʱ�ȭ�մϴ�.
        // Symbols �������� ���� �����͸� �߰��մϴ�.
        symbolOdds.Add((int)Symbols.Cherry, 1.0f);
        symbolOdds.Add((int)Symbols.Lemon, 2.0f);
        symbolOdds.Add((int)Symbols.Orange, 3.0f);
        symbolOdds.Add((int)Symbols.Bell, 4.0f);
        symbolOdds.Add((int)Symbols.Diamond, 5.0f);
        symbolOdds.Add((int)Symbols.Star, 6.0f);
        symbolOdds.Add((int)Symbols.Seven, 7.0f); // ���� ���� ����
    }

    /// <summary>
    /// Ư�� �ɺ� ID�� ������ �����ɴϴ�.
    /// </summary>
    /// <param name="symbolId">������ ��ȸ�� �ɺ� ID</param>
    /// <returns>�ش� �ɺ��� ����. �������� ���� ��� 0.0f ��ȯ</returns>
    public float GetOdds(int symbolId)
    {
        if (symbolOdds.TryGetValue(symbolId, out float odds))
        {
            return odds;
        }

        Debug.LogWarning($"SymbolData���� �ɺ� ID '{symbolId}'�� �ش��ϴ� ������ ã�� �� �����ϴ�.");
        return 0.0f;
    }
}
