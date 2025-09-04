using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SymbolWeightProcessor
{
    private readonly List<SymbolWeight.WeightedSymbol> weightedSymbols;
    private readonly string sourceName;
    private float totalProbability;

    public SymbolWeightProcessor(SymbolWeight symbolWeight)
    {
        // ScriptableObject의 데이터 복사
        weightedSymbols = symbolWeight.WeightedSymbols.Select(ws => new SymbolWeight.WeightedSymbol
        {
            symbol = ws.symbol,
            probability = ws.probability
        }).ToList();

        this.sourceName = symbolWeight.name;
        RecalculateTotalProbability();
    }

    // 확률의 합계를 계산
    public void RecalculateTotalProbability()
    {
        totalProbability = weightedSymbols.Sum(s => s.probability);
    }

    public int GetRandomWeightedSymbol()
    {
        if (totalProbability <= 0)
        {
            Debug.LogError("총 확률이 0이거나 음수입니다. SymbolWeight 스크립트 오브젝트의 확률을 올바르게 설정해주세요.");
            if (weightedSymbols != null && weightedSymbols.Count > 0)
                return (int)weightedSymbols[0].symbol;
            return 1; // 기본 심볼 값
        }

        float randomNumber = Random.Range(0f, totalProbability);
        float currentProbability = 0f;

        foreach (var symbol in weightedSymbols)
        {
            currentProbability += symbol.probability;
            if (randomNumber < currentProbability)
            {
                return (int)symbol.symbol;
            }
        }

        // 오류 발생 시 기본값 반환
        if (weightedSymbols != null && weightedSymbols.Count > 0)
            return (int)weightedSymbols[weightedSymbols.Count - 1].symbol;
        return 1;
    }

    public void LogAllProbabilities()
    {
        string probabilityLog = $"[{this.sourceName}] 현재 심볼 확률: ";
        foreach (var s in weightedSymbols)
        {
            probabilityLog += $"{s.symbol}: {s.probability:F2}% ";
        }
        probabilityLog += $" (총합: {totalProbability:F2}%)";
        Debug.Log(probabilityLog);
    }

    /// <summary>
    /// 스핀 직전에 심볼들의 확률의 총합이 100% 미만 또는 초과일 때 100%으로 보정
    /// </summary>
    public void NormalizeProbabilities()
    {
        if (weightedSymbols == null || weightedSymbols.Count == 0) return;

        RecalculateTotalProbability();

        if (totalProbability <= 0)
        {
            Debug.LogError($"[{this.sourceName}] 모든 심볼의 확률이 0이어서 정규화할 수 없습니다. 기본값으로 재설정...");
            SetDefaultProbabilities();
            RecalculateTotalProbability();
            return;
        }

        // 총합이 100이 되도록 함
        foreach (var symbol in weightedSymbols)
        {
            symbol.probability = (symbol.probability / totalProbability) * 100f;
        }
        RecalculateTotalProbability(); // 합계 재계산.
    }

    // 특정 심볼의 확률을 조정하고 나머지 심볼들의 확률을 보정하는 메서드
    public void SetProbability(Symbols symbol, float change)
    {
        if (weightedSymbols == null || weightedSymbols.Count == 0) return;

        var targetSymbol = weightedSymbols.FirstOrDefault(s => s.symbol == symbol);
        if (targetSymbol == null)
        {
            Debug.LogError($"'{symbol}' 심볼을 찾을 수 없습니다.");
            return;
        }

        // 1. 타겟 심볼 확률 변경 (0~100% 범위 제한)
        float oldProbability = targetSymbol.probability;
        targetSymbol.probability = Mathf.Clamp(targetSymbol.probability + change, 0f, 100f);
        float actualChange = targetSymbol.probability - oldProbability;

        // 변화가 없다면 조기 종료
        if (Mathf.Abs(actualChange) < 0.0001f)
        {
            return;
        }

        // 2. 나머지 심볼들
        var otherSymbols = weightedSymbols.Where(s => s.symbol != symbol).ToList();
        if (otherSymbols.Count == 0)
        {
            Debug.LogWarning("다른 심볼이 없어 보정할 수 없습니다.");
            RecalculateTotalProbability();
            return;
        }

        // 3. 남은 심볼들에게 "반대로" 보정 분배
        float remainingChange = -actualChange;
        int maxIterations = 10; // 무한 루프 방지
        int iterations = 0;

        while (Mathf.Abs(remainingChange) > 0.0001f && iterations < maxIterations)
        {
            var validSymbols = otherSymbols.Where(s =>
                (remainingChange > 0 && s.probability < 100f) ||  // 증가 필요 시 100% 미만 심볼
                (remainingChange < 0 && s.probability > 0f)      // 감소 필요 시 0% 초과 심볼
            ).ToList();

            if (validSymbols.Count == 0) break;

            float correctionPerSymbol = remainingChange / validSymbols.Count;
            float redistributed = 0f;

            foreach (var s in validSymbols)
            {
                float oldProb = s.probability;
                s.probability = Mathf.Clamp(s.probability + correctionPerSymbol, 0f, 100f);
                redistributed += (s.probability - oldProb);
            }

            remainingChange -= redistributed;
            iterations++;
        }

        RecalculateTotalProbability();

        Debug.Log($"'{symbol}' 심볼 확률 {actualChange:+0.00;-0.00}% 변경됨. (현재: {targetSymbol.probability:F2}%)");
        LogAllProbabilities();
    }

    // 디폴트 확률 설정(100 / 7)
    private void SetDefaultProbabilities()
    {
        weightedSymbols.Clear();
        int symbolCount = System.Enum.GetNames(typeof(Symbols)).Length;
        float equalProbability = 100f / symbolCount;

        for (int i = 1; i <= symbolCount; i++)
        {
            weightedSymbols.Add(new SymbolWeight.WeightedSymbol { symbol = (Symbols)i, probability = equalProbability });
        }
    }
}