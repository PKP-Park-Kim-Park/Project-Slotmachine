using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 슬롯머신 심볼들의 배당률 데이터를 관리하는 클래스.
/// </summary>
public class SymbolData
{
    // 심볼 ID와 배당률을 연결하는 딕셔너리
    private Dictionary<int, float> symbolOdds = new Dictionary<int, float>();

    public SymbolData()
    {
        // 생성자에서 심볼 배당률 데이터를 초기화합니다.
        // Symbols 열거형에 맞춰 데이터를 추가합니다.
        symbolOdds.Add((int)Symbols.Cherry, 1.0f);
        symbolOdds.Add((int)Symbols.Lemon, 2.0f);
        symbolOdds.Add((int)Symbols.Orange, 3.0f);
        symbolOdds.Add((int)Symbols.Bell, 4.0f);
        symbolOdds.Add((int)Symbols.Diamond, 5.0f);
        symbolOdds.Add((int)Symbols.Star, 6.0f);
        symbolOdds.Add((int)Symbols.Seven, 7.0f); // 가장 높은 배당률
    }

    /// <summary>
    /// 특정 심볼 ID의 배당률을 가져옵니다.
    /// </summary>
    /// <param name="symbolId">배당률을 조회할 심볼 ID</param>
    /// <returns>해당 심볼의 배당률. 존재하지 않을 경우 0.0f 반환</returns>
    public float GetOdds(int symbolId)
    {
        if (symbolOdds.TryGetValue(symbolId, out float odds))
        {
            return odds;
        }

        Debug.LogWarning($"SymbolData에서 심볼 ID '{symbolId}'에 해당하는 배당률을 찾을 수 없습니다.");
        return 0.0f;
    }
}
