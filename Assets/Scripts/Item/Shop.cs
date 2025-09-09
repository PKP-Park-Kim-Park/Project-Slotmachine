using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    // 아이템 데이터 에셋을 참조합니다.
    [SerializeField] private ItemData itmeData;
    // 상점에서 판매할 아이템 목록입니다.
    private List<ItemDataModel> shopItems = new List<ItemDataModel>();

    // 상점 초기화
    private void Start()
    {
        // 게임 시작 시 상점 아이템을 초기화하거나 로드하는 로직
        LoadShopItems();
    }

    // 아이템 구매
    public void BuyItem(int itemID)
    {
        ItemDataModel buyItem = shopItems.Find(item => item.id == itemID);
        if (buyItem.id != 0)
        {
            // 플레이어의 재화 확인 및 차감 로직
            // 인벤토리에 아이템 추가 로직
            Debug.Log($"{buyItem.name} 아이템을 구매했습니다.");
        }
    }
    public void SellItem(ItemDataModel sellItem)
    {
        if (sellItem.id != 0)
        {
            // 플레이어의 인벤토리에서 아이템 제거 로직
            // 플레이어의 재화 추가 로직

            Debug.Log($"{sellItem.name} 아이템을 판매했습니다.");
        }
    }

    // 상점 아이템 로드
    private void LoadShopItems()
    {
        // ItmeData ScriptableObject에서 아이템 목록을 가져와
        // shopItems 리스트에 채웁니다.
        // 이 부분은 상점의 종류나 레벨에 따라 다르게 구현할 수 있습니다.
        if (itmeData != null && itmeData.itemDataModels.Count > 0)
        {
            shopItems.AddRange(itmeData.itemDataModels);
        }
    }

    // 상점 리롤
    public void RerollShop()
    {
        // shopItems 리스트를 비우고
        shopItems.Clear();

        // 새로운 아이템을 무작위로 선택하여 추가하는 로직
        // 예시: ItmeData에서 무작위 아이템 3개 선택
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, itmeData.itemDataModels.Count);
            shopItems.Add(itmeData.itemDataModels[randomIndex]);
        }
        Debug.Log("상점 아이템이 리롤되었습니다.");
    }

    // 상점 아이템 목록을 가져오는 public 메서드 (TestRunner에서 사용)
    public List<ItemDataModel> GetShopItems()
    {
        return shopItems;
    }
}