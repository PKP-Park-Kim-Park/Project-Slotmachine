using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    // ������ ������ ������ �����մϴ�.
    [SerializeField] private ItmeData itmeData;
    // �������� �Ǹ��� ������ ����Դϴ�.
    private List<ItemDataModel> shopItems = new List<ItemDataModel>();

    // ���� �ʱ�ȭ
    private void Start()
    {
        // ���� ���� �� ���� �������� �ʱ�ȭ�ϰų� �ε��ϴ� ����
        // ����: �̸� ���ǵ� �����۵��� �ε�
        LoadShopItems();
    }

    // ������ ����
    public void BuyItem(int itemID)
    {
        ItemDataModel buyItem = shopItems.Find(item => item.id == itemID);
        if (buyItem.id != 0)
        {
            // �÷��̾��� ��ȭ Ȯ�� �� ���� ����
            // �κ��丮�� ������ �߰� ����
            Debug.Log($"{buyItem.name} �������� �����߽��ϴ�.");
        }
    }
    public void SellItem(ItemDataModel sellItem)
    {
        if (sellItem.id != 0)
        {
            // �÷��̾��� �κ��丮���� ������ ���� ����
            // �÷��̾��� ��ȭ �߰� ����

            Debug.Log($"{sellItem.name} �������� �Ǹ��߽��ϴ�.");
        }
    }

    // ���� ������ �ε�
    private void LoadShopItems()
    {
        // ItmeData ScriptableObject���� ������ ����� ������
        // shopItems ����Ʈ�� ä��ϴ�.
        // �� �κ��� ������ ������ ������ ���� �ٸ��� ������ �� �ֽ��ϴ�.
        if (itmeData != null && itmeData.itemDataModels.Count > 0)
        {
            shopItems.AddRange(itmeData.itemDataModels);
        }
    }

    // ���� ����
    public void RerollShop()
    {
        // shopItems ����Ʈ�� ����
        shopItems.Clear();

        // ���ο� �������� �������� �����Ͽ� �߰��ϴ� ����
        // ����: ItmeData���� ������ ������ 5�� ����
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, itmeData.itemDataModels.Count);
            shopItems.Add(itmeData.itemDataModels[randomIndex]);
        }
    }
}