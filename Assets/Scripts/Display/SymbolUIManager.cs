using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 이 네임스페이스를 추가합니다.
using System.Collections.Generic;
using System.Linq;

public class SymbolUIManager : MonoBehaviour
{
    public SymbolWeight symbolWeightData;
    public List<TextMeshPro> probabilityTexts;

    public SymbolRewardOdds symbolRewardOddsData;
    public List<TextMeshPro> oddsTexts;

    private void Start()
    {
        UpdateProbabilitiesUI();
        UpdateOddsUI();
    }

    // Unity 에디터에서 값이 변경될 때 호출됩니다.
    private void OnValidate()
    {
        UpdateProbabilitiesUI();
        UpdateOddsUI();
    }


    private void UpdateProbabilitiesUI()
    {
        if (symbolWeightData == null || probabilityTexts == null || 
            symbolWeightData.WeightedSymbols.Count != probabilityTexts.Count)
        {
            Debug.LogError("데이터 또는 UI 텍스트 리스트의 설정이 올바르지 않습니다.");
            return;
        }

        for (int i = 0; i < symbolWeightData.WeightedSymbols.Count; i++)
        {
            SymbolWeight.WeightedSymbol symbolData = symbolWeightData.WeightedSymbols[i];
            TextMeshPro textComponent = probabilityTexts[i];

            // "Symbol: 확률%" 형식으로 텍스트를 만듭니다.
            textComponent.text = $"Symbol : {symbolData.probability.ToString("F1")}%";
        }
    }
    private void UpdateOddsUI()
    {
        if (symbolRewardOddsData == null || oddsTexts == null || 
            symbolRewardOddsData.rewardSymbols.Count != oddsTexts.Count)
        {
            Debug.LogError("데이터 또는 UI 텍스트 리스트의 설정이 올바르지 않습니다.");
            return;
        }
        for (int i = 0; i < symbolRewardOddsData.rewardSymbols.Count; i++)
        {
            RewardSymbolOddsEntry oddsData = symbolRewardOddsData.rewardSymbols[i];
            TextMeshPro textComponent = oddsTexts[i];
            // "Symbol: 배율x" 형식으로 텍스트를 만듭니다.
            textComponent.text = $"$ : {oddsData.rewardOddsMultiplier.ToString("F1")}x";

        }
    }
}