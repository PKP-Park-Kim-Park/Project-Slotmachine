using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Reel 클래스는 슬롯머신 릴의 동작을 관리합니다.
// Symbols 열거형은 별도의 파일에 정의되어 있다고 가정합니다.
public class Reel : MonoBehaviour
{
    // 릴이 현재 회전 중인지 여부, 외부에서 읽기 전용으로 접근 가능
    public bool isSpinning { get; private set; }

    // 릴에 배치된 심볼을 나타내는 배열, 외부에서 읽기 전용으로 접근 가능
    public int[] row { get; private set; } = new int[7];

    // 심볼과 그에 해당하는 확률을 함께 저장하는 클래스
    [System.Serializable]
    public class WeightedSymbol
    {
        public Symbols symbol;
        [Range(0f, 100f)]
        //probability = 심볼이 나올 확률
        public float probability;
    }

    // 심볼과 확률을 설정할 수 있도록 합니다.
    private List<WeightedSymbol> weightedSymbols;

    // 모든 심볼의 총 확률의 합을 저장하는 변수
    private float totalProbability;

    private void Awake()
    {
        // 인스펙터에 설정된 값이 없으면 코드에서 초기값을 설정합니다.
        if (weightedSymbols == null || weightedSymbols.Count == 0)
        {
            SetDefaultProbabilities();
        }
        RecalculateTotalProbability();
    }

    // 슬롯 확률 초기 값 설정
    // Symbols 열거형에 정의된 심볼 개수를 가져와서 100을 나눈 뒤, 각 심볼에 동일한 확률을 할당
    private void SetDefaultProbabilities()
    {
        weightedSymbols = new List<WeightedSymbol>();
        int symbolCount = System.Enum.GetNames(typeof(Symbols)).Length;
        float equalProbability = 100f / symbolCount;

        for (int i = 1; i <= symbolCount; i++)
        {
            weightedSymbols.Add(new WeightedSymbol { symbol = (Symbols)i, probability = equalProbability });
        }

        Debug.Log("기본 확률이 코드에서 균등하게 설정되었습니다.");
    }

    // 모든 심볼의 총 확률을 재계산합니다.
    private void RecalculateTotalProbability()
    {
        totalProbability = weightedSymbols.Sum(s => s.probability);
    }

    // 특정 심볼의 확률을 조정하고 나머지 심볼들의 확률을 보정합니다.
    public void SetProbability(Symbols symbol, float change)
    {
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
            Debug.Log($"'{symbol}' 심볼 확률 변경 없음. (현재: {targetSymbol.probability:F2}%)");
            return;
        }

        // 2. 나머지 심볼들
        var otherSymbols = weightedSymbols.Where(s => s.symbol != symbol).ToList();
        if (otherSymbols.Count == 0)
        {
            Debug.LogWarning("다른 심볼이 없어 보정할 수 없습니다.");
            return;
        }

        // 3. 남은 심볼들에게 "반대로" 보정 분배
        float remainingChange = -actualChange;
        while (Mathf.Abs(remainingChange) > 0.0001f)
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
        }

        RecalculateTotalProbability();

        Debug.Log($"'{symbol}' 심볼 확률 {actualChange:+0.00;-0.00}% 변경됨. (현재: {targetSymbol.probability:F2}%)");
        Debug.Log($"보정 후 총 확률 = {totalProbability:F2}%");
        LogAllProbabilities();
    }

    // 모든 심볼의 확률을 디버그 로그에 출력하는 별도 메서드 추가
    private void LogAllProbabilities()
    {
        string probabilityLog = "현재 심볼 확률: ";
        foreach (var s in weightedSymbols)
        {
            probabilityLog += $"{s.symbol}: {s.probability:F2}% ";
        }
        Debug.Log(probabilityLog);
    }

    // 확률에 따라 나온 심볼을 릴에 재배치합니다.
    public void RelocateSymbols()
    {
        RecalculateTotalProbability();
        for (int i = 0; i < row.Length; i++)
        {
            row[i] = GetRandomWeightedSymbol();
        }
        Debug.Log("릴 심볼이 확률에 따라 재배치되었습니다.");
        LogAllProbabilities(); // 릴 재배치 시에도 확률 로그 추가
    }


    /// 확률에 따라 무작위로 심볼을 선택합니다.
    private int GetRandomWeightedSymbol()
    {
        if (totalProbability <= 0)
        {
            Debug.LogError("총 확률이 0이거나 음수입니다. 확률을 올바르게 설정해주세요.");
            return (int)weightedSymbols[0].symbol;
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
        return (int)weightedSymbols[0].symbol;
    }

    // 릴의 회전을 시작합니다.
    public IEnumerator StartSpin()
    {
        if (isSpinning)
        {
            yield break;
        }

        isSpinning = true;
        Debug.Log("릴 회전 시작!");
    }


    // 릴의 회전을 멈추고 최종 심볼 배열을 설정합니다.
    public int[] StopSpin(int[] finalRow)
    {
        isSpinning = false;
        // 최종 심볼 배열을 릴에 적용합니다.
        row = finalRow;
        Debug.Log("릴 회전 중지!");

        // 인덱스 2, 3, 4번만 출력
        int[] resultRow = new int[3];
        for (int i = 0; i < 3; i++)
        {
            resultRow[i] = row[i + 2];
        }

        return resultRow;
    }
}