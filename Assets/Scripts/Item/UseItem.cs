using System;
using UnityEngine;

public struct SymbolEffectData
{
    public FlagSymbols Symbols;
    public float Amount;
    public UseType OriginalUseType;
}

public struct PatternEffectData
{
    public FlagPatterns Patterns;
    public float Amount;
}

public struct StressEffectData
{
    public StressType StressType;
    public float Amount;
}

public class UseItem
{
    public event Action<SymbolEffectData> OnSymbolEffectReady;
    public event Action<PatternEffectData> OnPatternEffectReady;
    public event Action<StressEffectData> OnStressEffectReady;

    public void Use(ItemDataModel model)
    {
        Debug.Log($"--- 아이템 사용 시작: {model.name} ---");

        bool hasGoodEffect = model.hasRisk == HasRisk.Good || model.hasRisk == HasRisk.Both;
        bool hasRiskEffect = model.hasRisk == HasRisk.Risk || model.hasRisk == HasRisk.Both;

        if (hasGoodEffect)
        {
            CheckEffect(model.itemEffect);
        }

        if (hasRiskEffect)
        {
            CheckEffect(model.itemRiskEffect);
        }

        if (!hasGoodEffect && !hasRiskEffect)
        {
            Debug.Log("적용할 효과가 없는 아이템입니다.");
        }
    }

    private void CheckEffect(ItemEffectModel effectModel)
    {
        switch (effectModel.useType)
        {
            case UseType.Symbol:
            case UseType.SymbolReward:
                SymbolEffectData symbolData = new SymbolEffectData
                {
                    Symbols = effectModel.flagSymbols,
                    Amount = effectModel.amount,
                    OriginalUseType = effectModel.useType
                };
                Debug.Log($"[효과 적용] 심볼 | 타입: {symbolData.OriginalUseType}, 대상: {symbolData.Symbols}, 값: {symbolData.Amount}");
                OnSymbolEffectReady?.Invoke(symbolData);
                break;

            case UseType.PartternReward:
                PatternEffectData patternData = new PatternEffectData
                {
                    Patterns = effectModel.flagPatterns,
                    Amount = effectModel.amount
                };
                Debug.Log($"[효과 적용] 패턴 | 대상: {patternData.Patterns}, 값: {patternData.Amount}");
                OnPatternEffectReady?.Invoke(patternData);
                break;

            case UseType.Stress:
                StressEffectData stressData = new StressEffectData
                {
                    StressType = effectModel.stressType,
                    Amount = effectModel.amount
                };
                Debug.Log($"[효과 적용] 스트레스 | 대상: {stressData.StressType}, 값: {stressData.Amount}");
                OnStressEffectReady?.Invoke(stressData);
                break;

            default:
                break;
        }
    }
}
