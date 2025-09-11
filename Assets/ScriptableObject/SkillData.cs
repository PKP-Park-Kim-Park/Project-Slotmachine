using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SkillDataModel
{
    public int id;
    public ESkill skill;
    [TextArea]
    public string description;
    public int level;
    public int num;
    public float amount;
    public int price;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public List<SkillDataModel> skillDataModels;
}
