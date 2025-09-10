using UnityEngine;

public class MyPreciousEffect : IToggleableSkill
{
    public void Activate(SkillDataModel data, GameObject user)
    {
        Debug.Log($"'{data.skill.ToString()}' 활성화: 골드 {data.amount} 증가.");
        GameManager.instance.money.AddGold((int)data.amount);
    }

    public void Deactivate(SkillDataModel data, GameObject user)
    {
        Debug.Log($"'{data.skill.ToString()}' 비활성화: 골드 {data.amount} 감소.");
        GameManager.instance.money.SpendGold((int)data.amount);
    }
}
