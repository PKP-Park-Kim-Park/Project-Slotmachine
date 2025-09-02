using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RewardPatternOddsEntry
{
    public Patterns patternType;
    public float rewardOddsMultiplier;
}

[CreateAssetMenu(fileName = "PatternRewardOdds", menuName = "RewardOdds/PatternRewardOdds")]
public class PatternRewardOdds : ScriptableObject
{
    public List<RewardPatternOddsEntry> rewardPatterns;
}
