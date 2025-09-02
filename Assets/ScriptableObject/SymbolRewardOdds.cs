using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RewardSymbolOddsEntry
{
    public Symbols symbolType;
    public float rewardOddsMultiplier;
}

[CreateAssetMenu(fileName = "SymbolRewardOdds", menuName = "RewardOdds/SymbolRewardOdds")]
public class SymbolRewardOdds : ScriptableObject
{
    public List<RewardSymbolOddsEntry> rewardSymbols;
}
