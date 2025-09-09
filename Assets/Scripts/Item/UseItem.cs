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

    private const float USE_MULTIPLIER = 1.0f;
    private const float REMOVE_MULTIPLIER = -1.0f;

    public void Use(ItemDataModel model)
    {
        Debug.Log($"--- 아이템 사용 시작: {model.name} ---");
        ProcessItemEffects(model, USE_MULTIPLIER);
    }

    public void Remove(ItemDataModel model)
    {
        Debug.Log($"--- 아이템 취소 시작: {model.name} ---");
        ProcessItemEffects(model, REMOVE_MULTIPLIER);
    }

    private void ProcessItemEffects(ItemDataModel model, float multiplier)
    {
        bool hasGoodEffect = model.hasRisk == HasRisk.Good || model.hasRisk == HasRisk.Both;
        bool hasRiskEffect = model.hasRisk == HasRisk.Risk || model.hasRisk == HasRisk.Both;

        if (hasGoodEffect)
        {
            CheckEffect(model.itemEffect, model.itemEffect.amount * multiplier);
        }

        if (hasRiskEffect)
        {
            CheckEffect(model.itemRiskEffect, model.itemRiskEffect.amount * multiplier);
        }

        if (!hasGoodEffect && !hasRiskEffect)
        {
            Debug.Log("적용할 효과가 없는 아이템입니다.");
        }
    }

    private void CheckEffect(ItemEffectModel effectModel, float amount)
    {

        switch (effectModel.useType)
        {
            case UseType.Symbol:
            case UseType.SymbolReward:
                SymbolEffectData symbolData = new SymbolEffectData
                {
                    Symbols = effectModel.flagSymbols,
                    Amount = amount,
                    OriginalUseType = effectModel.useType
                };
                Debug.Log($"[효과 적용] 심볼 | 타입: {symbolData.OriginalUseType}, 대상: {symbolData.Symbols}, 값: {amount}");
                OnSymbolEffectReady?.Invoke(symbolData);
                break;

            case UseType.PartternReward:
                PatternEffectData patternData = new PatternEffectData
                {
                    Patterns = effectModel.flagPatterns,
                    Amount = amount
                };
                Debug.Log($"[효과 적용] 패턴 | 대상: {patternData.Patterns}, 값: {amount}");
                OnPatternEffectReady?.Invoke(patternData);
                break;

            case UseType.Stress:
                StressEffectData stressData = new StressEffectData
                {
                    StressType = effectModel.stressType,
                    Amount = amount
                };
                Debug.Log($"[효과 적용] 스트레스 | 대상: {stressData.StressType}, 값: {amount}");
                OnStressEffectReady?.Invoke(stressData);
                break;

            default:
                break;
        }
    }
}
