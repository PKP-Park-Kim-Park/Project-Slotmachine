using UnityEngine;

public interface IToggleableSkill
{
    void Activate(SkillDataModel data, GameObject user);
    void Deactivate(SkillDataModel data, GameObject user);
}
