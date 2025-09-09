using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public enum UseType
{
    None,
    Symbol,
    SymbolReward,
    PartternReward,
    Stress
}

[Flags]
public enum FlagPatterns
{
    None = 0,
    Column_3 = 1,
    Column_4 = 2,
    Column_5 = 4,
    Row_3 = 8,
    LowerDiagonal = 16,
    UpperDiagonal = 32,
    Zig = 64,
    Zag = 128,
    Up = 256,
    Down = 512,
    Eyes = 1024,
    Jackpot = 2048
}

[Flags]
public enum FlagSymbols
{
    None = 0,
    Cherry = 1,
    Banana = 2,
    Carrot = 4,
    Bell = 8,
    Diamond = 16,
    Star = 32,
    Seven = 64,
    Skull = 128
}

public enum StressType
{
    None,
    CurrentStress,
    MaxStress
}

public enum Rarity
{
    None,
    Common,
    Rare,
    Unique,
    Legendary
}

public enum HasRisk
{
    None,
    Good,
    Risk,
    Both
}

[Serializable]
public struct ItemEffectModel
{
    public UseType useType;
    public FlagSymbols flagSymbols;
    public FlagPatterns flagPatterns;
    public StressType stressType;
    public float amount;
}

[Serializable]
public struct ItemDataModel
{
    public int id;
    public string name;
    public int price;
    public Rarity rarity;
    public HasRisk hasRisk;
    public Image image;
    public ItemEffectModel itemEffect;
    public ItemEffectModel itemRiskEffect;
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public List<ItemDataModel> itemDataModels;
}
