using UnityEngine;

public class AromaPerfumeEffect : IToggleableSkill
{
    private PlayerStress playerStress;
    public AromaPerfumeEffect(PlayerStress stress) 
    { 
        this.playerStress = stress; 
    }

    public void Activate(SkillDataModel data, GameObject user)
    {
        Debug.Log($"'{data.skill.ToString()}' 활성화: 최대 스트레스 {data.amount} 증가.");
        playerStress.IncreaseMaxStress((int)data.amount);
    }

    public void Deactivate(SkillDataModel data, GameObject user)
    {
        Debug.Log($"'{data.skill.ToString()}' 비활성화: 최대 스트레스 {data.amount} 감소.");
        playerStress.DecreaseMaxStress((int)data.amount);
    }
}
