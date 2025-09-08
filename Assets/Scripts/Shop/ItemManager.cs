using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static ItemManager Instance { get; private set; }

    // ��� ������ �����͸� ��� �ִ� ScriptableObject
    [SerializeField] private ItmeData itmeData;

    // ��� ������ �����͸� ������ �迭
    private List<ItemDataModel> allItems = new List<ItemDataModel>();

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            // ���� ��ȯ�Ǿ �ı����� �ʰ� ����
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadAllItems();
    }

    // ScriptableObject���� ��� ������ �����͸� �ε�
    private void LoadAllItems()
    {
        if (itmeData != null)
        {
            allItems = itmeData.itemDataModels;
            Debug.Log($"������ ������ {allItems.Count}�� �ε� �Ϸ�.");
        }
        else
        {
            Debug.LogError("ItmeData ScriptableObject�� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }

    // ������ ID�� ������ ������ ���� �������� �޼���
    public ItemDataModel GetItemData(int itemID)
    {
        // ���ٽ��� ����Ͽ� ����Ʈ���� ID�� �´� �������� ã���ϴ�.
        return allItems.Find(item => item.id == itemID);
    }
}