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
        Debug.Log("��ų ���� �ʱ�ȭ �Ϸ�.");
    }

    /// <summary>
    /// [�ٽ� �Լ�] UI���� ��ų�� Ŭ������ �� ȣ���� ���� �Լ�.
    /// ����, Ȱ��ȭ, ��Ȱ��ȭ, ��ü ������ ��� ó���մϴ�.
    /// </summary>
    public void OnClickSkillInShop(int id)
    {
        SkillDataModel skill = FindSkillById(id);
        if (skill.id == 0)
        {
            return; // ��ȿ���� ���� ��ų
        }

        bool isOwned = ownedSkillIds.Contains(id);
        bool isActive = activeSkillIds.Contains(id);

        // --- �ó����� 1: ������ ��ų�� Ŭ���� ��� ---
        if (isOwned)
        {
            if (isActive) // �̹� Ȱ��ȭ�� ��ų -> ��Ȱ��ȭ
            {
                DeactivateSkill(id);
            }
            else // ���������� ��Ȱ��ȭ�� ��ų -> Ȱ��ȭ (��ü ���� ����)
            {
                TryActivateWithSwap(skill);
            }
        }
        // --- �ó����� 2: �������� ���� ��ų�� Ŭ���� ��� ---
        else
        {
            // ���� �õ�
            if (CanBuySkill(skill))
            {
                ProcessPurchase(skill); // ���� ó��
                TryActivateWithSwap(skill); // ���� �� ��� Ȱ��ȭ (��ü ���� ����)
            }
        }
    }

    private void TryActivateWithSwap(SkillDataModel skillToActivate)
    {
        // ��Ģ : ���� ������ ��ų�� �̹� Ȱ��ȭ�Ǿ� �ִ°�?
        // FirstOrDefault: ���ǿ� �´� ù��° ��Ҹ� ã�ų� ������ �⺻��(0) ��ȯ
        int conflictingSkillId = activeSkillIds.FirstOrDefault(id => FindSkillById(id).level == skillToActivate.level);

        if (conflictingSkillId != 0)
        {
            // �浹�ϴ� ��ų�� ������, ���� ��Ȱ��ȭ (�ڵ� ��ü)
            DeactivateSkill(conflictingSkillId);
            Debug.Log($"[{conflictingSkillId}] ��ų�� �ڵ����� ��Ȱ��ȭ�ϰ� ��ü�� �غ��մϴ�.");
        }

        // ��� ��Ģ ���, ��ų Ȱ��ȭ
        ActivateSkill(skillToActivate.id);
    }

    // �ܼ� ���� ó�� (������ ����)
    private void ProcessPurchase(SkillDataModel skill)
    {
        SkillManager.instance.BuySkill(skill.price);
        ownedSkillIds.Add(skill.id);

        int nextLevel = skill.level + 1;
        if (nextLevel > maxUnlockedLevel)
        {
            maxUnlockedLevel = nextLevel;
        }
        Debug.Log($"[{skill.id}] ��ų ���� �Ϸ�.");
    }

    // ���� ���� ���θ� Ȯ��
    private bool CanBuySkill(SkillDataModel skill)
    {
        if (ownedSkillIds.Contains(skill.id))
        {
            Debug.Log("���� �Ұ�: �̹� ������ ��ų�Դϴ�.");
            return false;
        }
        if (skill.level > maxUnlockedLevel)
        {
            Debug.Log($"���� �Ұ�: {skill.level}������ ���� �رݵ��� �ʾҽ��ϴ�.");
            return false;
        }
        if (!SkillManager.instance.CheckCanBuySkill(skill.price))
        {
            Debug.Log("���� �Ұ�: ��ȭ�� �����մϴ�.");
            return false;
        }
        return true;
    }

    // [����] �ܼ� Ȱ��ȭ (����Ʈ �߰� �� SkillManager ȣ��)
    private void ActivateSkill(int id)
    {
        if (activeSkillIds.Contains(id))
        {
            return; // �ߺ� Ȱ��ȭ ����
        }

        activeSkillIds.Add(id);
        SkillManager.instance.ToggleSkill(id);
        Debug.Log($"[{id}] ��ų Ȱ��ȭ. (Ȱ�� ����: {activeSkillIds.Count})");
    }

    // [����] �ܼ� ��Ȱ��ȭ (����Ʈ ���� �� SkillManager ȣ��)
    private void DeactivateSkill(int id)
    {
        if (!activeSkillIds.Contains(id))
        {
            return; // ��Ȱ��ȭ�� �� �� ��Ȱ��ȭ ����
        }

        activeSkillIds.Remove(id);
        SkillManager.instance.ToggleSkill(id);
        Debug.Log($"[{id}] ��ų ��Ȱ��ȭ. (Ȱ�� ����: {activeSkillIds.Count})");
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