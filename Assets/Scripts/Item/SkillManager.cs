using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("필수 데이터 및 컴포넌트")]
    [SerializeField] private SkillData allSkillData;
    [SerializeField] private PlayerStress playerStress;
    // ... 스킬 로직에 필요한 다른 컴포넌트들을 여기에 추가 ...

    // --- 내부 데이터 ---
    private Dictionary<int, SkillDataModel> skillDatabase;
    private Dictionary<ESkill, IToggleableSkill> skillLogics;
    private HashSet<int> activeSkillIds = new HashSet<int>();

    // --- 외부 통신용 이벤트 ---
    public event Action<int> OnSkillActivated;
    public event Action<int> OnSkillDeactivated;

    private void Awake()
    {
        InitializeDatabase();
        InitializeLogics();
    }

    // --- 초기화 메서드 ---
    private void InitializeDatabase()
    {
        skillDatabase = new Dictionary<int, SkillDataModel>();
        foreach (var model in allSkillData.skillDataModels)
        {
            if (skillDatabase.ContainsKey(model.id))
            {
                Debug.LogWarning($"[SkillManager] 중복된 스킬 ID({model.id})가 있습니다: {model.skill.ToString()}");
                continue;
            }
            skillDatabase.Add(model.id, model);
        }
    }

    private void InitializeLogics()
    {
        skillLogics = new Dictionary<ESkill, IToggleableSkill>();

        // 여기에 모든 스킬 로직을 '수동으로' 등록합니다.
        // 이 부분이 데이터와 로직을 연결하는 '접착제' 역할을 합니다.
        skillLogics.Add(ESkill.My_precious, new MyPreciousEffect());
        skillLogics.Add(ESkill.AromaPerfume, new AromaPerfumeEffect(playerStress));
    }

    /// <summary>
    /// 스킬을 활성화/비활성화합니다. 패시브/토글 스킬에 사용됩니다.
    /// </summary>
    public void ToggleSkill(int skillId)
    {
        if (skillDatabase.TryGetValue(skillId, out SkillDataModel data) == false)
        {
            Debug.LogError($"[SkillManager] 존재하지 않는 스킬 ID: {skillId}");
            return;
        }

        if (activeSkillIds.Contains(skillId))
        {
            DeactivateSkill(skillId);
        }
        else
        {
            ActivateSkill(skillId);
        }
    }

    private void ActivateSkill(int skillId)
    {
        SkillDataModel data = skillDatabase[skillId];
        if (skillLogics.TryGetValue(data.skill, out IToggleableSkill logic))
        {
            logic.Activate(data, this.gameObject);
            activeSkillIds.Add(skillId);
            OnSkillActivated?.Invoke(skillId); // UI 등 다른 시스템에 알림
        }
    }

    private void DeactivateSkill(int skillId)
    {
        SkillDataModel data = skillDatabase[skillId];
        if (skillLogics.TryGetValue(data.skill, out IToggleableSkill logic))
        {
            logic.Deactivate(data, this.gameObject);
            activeSkillIds.Remove(skillId);
            OnSkillDeactivated?.Invoke(skillId); // UI 등 다른 시스템에 알림
        }
    }

    // --- 데이터 조회 및 상태 확인용 public 메서드 ---
    public SkillDataModel GetSkillData(int skillId)
    {
        skillDatabase.TryGetValue(skillId, out SkillDataModel data);
        return data;
    }
}
