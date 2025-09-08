using UnityEngine;
using System;
using System.Collections.Generic;

public enum ItemType
{
    None,
    CanUseItem,
    Skill
}

public enum Goods
{
    None,
    Gold,
    Tokken
}

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
    Seven = 64
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
public struct ItemEffetModel
{
    public UseType useType;
    public FlagSymbols flagSymbols;
    public FlagPatterns flagPatterns;
    public StressType stressType;
    public int amount;
}

[Serializable]
public struct ItemDataModel
{
    public int id;
    public ItemType itemType;
    public string name;
    public Goods goods;
    public int price;
    public Rarity rarity;
    public HasRisk hasRisk;
    public ItemEffetModel itmeEffect;
    public ItemEffetModel itmeRiskEffect;
}

[CreateAssetMenu(fileName = "ItmeData", menuName = "Scriptable Objects/ItmeData")]
public class ItmeData : ScriptableObject
{
    public List<ItemDataModel> canUseItemDataModels;
    public List<ItemDataModel> SkillDataModels;

    private void OnValidate()
    {
        for (int i = 0; i < canUseItemDataModels.Count; i++)
        {
            ItemDataModel item = canUseItemDataModels[i];
            item.itemType = ItemType.CanUseItem; 
            item.goods = Goods.Gold;
            canUseItemDataModels[i] = item;  
        }

        for (int i = 0; i < SkillDataModels.Count; i++)
        {
            ItemDataModel item = SkillDataModels[i];
            item.itemType = ItemType.Skill;
            item.goods = Goods.Tokken;
            SkillDataModels[i] = item;
        }
    }
}
