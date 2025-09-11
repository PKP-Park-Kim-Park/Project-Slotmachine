using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillShop : MonoBehaviour
{
    private Dictionary<int, List<SkillDataModel>> skillsByLevel = new Dictionary<int, List<SkillDataModel>>();
    private List<int> ownedSkillIds = new List<int>();
    private List<int> activeSkillIds = new List<int>();
    private int maxUnlockedLevel = 1;

    void Start()
    {
        InitializeSkillShop();
    }

    private void InitializeSkillShop()
    {
        SkillData skillDatas = SkillManager.instance.GetSkillData();
        foreach (SkillDataModel skillData in skillDatas.skillDataModels)
        {
            if (!skillsByLevel.ContainsKey(skillData.level))
            {
                skillsByLevel[skillData.level] = new List<SkillDataModel>();
            }
            skillsByLevel[skillData.level].Add(skillData);
        }
        Debug.Log("스킬 상점 초기화 완료.");
    }

    /// <summary>
    /// [핵심 함수] UI에서 스킬을 클릭했을 때 호출할 단일 함수.
    /// 구매, 활성화, 비활성화, 교체 로직을 모두 처리합니다.
    /// </summary>
    public void OnClickSkillInShop(int id)
    {
        SkillDataModel skill = FindSkillById(id);
        if (skill.id == 0)
        {
            return; // 유효하지 않은 스킬
        }

        bool isOwned = ownedSkillIds.Contains(id);
        bool isActive = activeSkillIds.Contains(id);

        // --- 시나리오 1: 소유한 스킬을 클릭한 경우 ---
        if (isOwned)
        {
            if (isActive) // 이미 활성화된 스킬 -> 비활성화
            {
                DeactivateSkill(id);
            }
            else // 소유했지만 비활성화된 스킬 -> 활성화 (교체 로직 포함)
            {
                TryActivateWithSwap(skill);
            }
        }
        // --- 시나리오 2: 소유하지 않은 스킬을 클릭한 경우 ---
        else
        {
            // 구매 시도
            if (CanBuySkill(skill))
            {
                ProcessPurchase(skill); // 구매 처리
                TryActivateWithSwap(skill); // 구매 후 즉시 활성화 (교체 로직 포함)
            }
        }
    }

    private void TryActivateWithSwap(SkillDataModel skillToActivate)
    {
        // 규칙 : 같은 레벨의 스킬이 이미 활성화되어 있는가?
        // FirstOrDefault: 조건에 맞는 첫번째 요소를 찾거나 없으면 기본값(0) 반환
        int conflictingSkillId = activeSkillIds.FirstOrDefault(id => FindSkillById(id).level == skillToActivate.level);

        if (conflictingSkillId != 0)
        {
            // 충돌하는 스킬이 있으면, 먼저 비활성화 (자동 교체)
            DeactivateSkill(conflictingSkillId);
            Debug.Log($"[{conflictingSkillId}] 스킬을 자동으로 비활성화하고 교체를 준비합니다.");
        }

        // 모든 규칙 통과, 스킬 활성화
        ActivateSkill(skillToActivate.id);
    }

    // 단순 구매 처리 (데이터 변경)
    private void ProcessPurchase(SkillDataModel skill)
    {
        SkillManager.instance.BuySkill(skill.price);
        ownedSkillIds.Add(skill.id);

        int nextLevel = skill.level + 1;
        if (nextLevel > maxUnlockedLevel)
        {
            maxUnlockedLevel = nextLevel;
        }
        Debug.Log($"[{skill.id}] 스킬 구매 완료.");
    }

    // 구매 가능 여부만 확인
    private bool CanBuySkill(SkillDataModel skill)
    {
        if (ownedSkillIds.Contains(skill.id))
        {
            Debug.Log("구매 불가: 이미 소유한 스킬입니다.");
            return false;
        }
        if (skill.level > maxUnlockedLevel)
        {
            Debug.Log($"구매 불가: {skill.level}레벨이 아직 해금되지 않았습니다.");
            return false;
        }
        if (!SkillManager.instance.CheckCanBuySkill(skill.price))
        {
            Debug.Log("구매 불가: 재화가 부족합니다.");
            return false;
        }
        return true;
    }

    // [헬퍼] 단순 활성화 (리스트 추가 및 SkillManager 호출)
    private void ActivateSkill(int id)
    {
        if (activeSkillIds.Contains(id))
        {
            return; // 중복 활성화 방지
        }

        activeSkillIds.Add(id);
        SkillManager.instance.ToggleSkill(id);
        Debug.Log($"[{id}] 스킬 활성화. (활성 슬롯: {activeSkillIds.Count})");
    }

    // [헬퍼] 단순 비활성화 (리스트 제거 및 SkillManager 호출)
    private void DeactivateSkill(int id)
    {
        if (!activeSkillIds.Contains(id))
        {
            return; // 비활성화된 걸 또 비활성화 방지
        }

        activeSkillIds.Remove(id);
        SkillManager.instance.ToggleSkill(id);
        Debug.Log($"[{id}] 스킬 비활성화. (활성 슬롯: {activeSkillIds.Count})");
    }

    private SkillDataModel FindSkillById(int id)
    {
        foreach (var skillList in skillsByLevel.Values)
        {
            var foundSkill = skillList.FirstOrDefault(skill => skill.id == id);
            if (foundSkill.id != 0)
            {
                return foundSkill;
            }
        }
        return default;
    }
}