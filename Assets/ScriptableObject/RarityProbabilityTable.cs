using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct RarityChances
{
    public int level; // 확률이 적용될 최소 레벨
    [Range(0f, 1f)] public float commonChance;
    [Range(0f, 1f)] public float rareChance;
    [Range(0f, 1f)] public float uniqueChance;
    [Range(0f, 1f)] public float legendaryChance;
}

[CreateAssetMenu(fileName = "RarityProbabilityTable", menuName = "Scriptable Objects/RarityProbabilityTable")]
public class RarityProbabilityTable : ScriptableObject
{
    public List<RarityChances> chancesByLevel;
}