using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Reel : MonoBehaviour
{
    // 릴이 현재 회전 중인지 여부
    public bool isSpinning { get; private set; }

    // 릴에 배치된 심볼을 나타내는 배열
    public int[] row { get; private set; } = new int[7];

    // 심볼과 그에 해당하는 확률을 함께 저장하는 클래스
    [System.Serializable]
    public class WeightedSymbol
    {
        public Symbols symbol;
        [Range(0f, 100f)]
        public float probability;
    }

    // 인스펙터에서 심볼과 확률을 설정할 수 있도록 합니다.
    [SerializeField] private List<WeightedSymbol> weightedSymbols;

    private float totalProbability;

    private void Awake()
    {
        // 인스펙터에 설정된 값이 없으면 코드에서 초기값을 설정합니다.
        if (weightedSymbols == null || weightedSymbols.Count == 0)
        {
            SetDefaultProbabilities();
        }

        // 총 확률이 100%인지 확인하고 보정합니다.
        CalculateTotalProbability();
    }

    private void OnValidate()
    {
        // 인스펙터에서 값이 변경될 때마다 총 확률을 업데이트합니다.
        CalculateTotalProbability();
    }

    //슬롯 확률 초기 값 설정
    private void SetDefaultProbabilities()
    {
        weightedSymbols = new List<WeightedSymbol>
        {
            new WeightedSymbol { symbol = Symbols.Cherry, probability = 15f },
            new WeightedSymbol { symbol = Symbols.Lemon, probability = 15f },
            new WeightedSymbol { symbol = Symbols.Orange, probability = 15f },
            new WeightedSymbol { symbol = Symbols.Diamond, probability = 15f },
            new WeightedSymbol { symbol = Symbols.Bell, probability = 15f },
            new WeightedSymbol { symbol = Symbols.Star, probability = 15f },
            new WeightedSymbol { symbol = Symbols.Seven, probability = 15f }
        };
        Debug.Log("기본 확률이 코드에서 설정되었습니다.");
    }

    // 총 확률을 계산하고 100%가 되도록 보정합니다.
    private void CalculateTotalProbability()
    {
        totalProbability = weightedSymbols.Sum(s => s.probability);

        // 총 확률이 100이 아닐 경우 보정
        if (totalProbability > 0 && Mathf.Abs(totalProbability - 100f) > 0.001f)
        {
            float correctionFactor = 100f / totalProbability;
            foreach (var symbol in weightedSymbols)
            {
                symbol.probability *= correctionFactor;
            }
            totalProbability = 100f; // 보정 후 총합을 100으로 설정
        }
    }


    // 확률에 따라 나온 심볼을 릴에 재배치합니다.
    public void RelocateSymbols()
    {
        for (int i = 0; i < row.Length; i++)
        {
            row[i] = GetRandomWeightedSymbol();
        }
        Debug.Log("릴 심볼이 확률에 따라 재배치되었습니다.");
    }


    /// 확률에 따라 무작위로 심볼을 선택합니다. 
    private int GetRandomWeightedSymbol()
    {
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
            //Debug.Log($"인덱스 {i}의 심볼 값: {resultRow[i]}");
        }

        return resultRow;
    }
}
