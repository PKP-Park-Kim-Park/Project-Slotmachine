using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shop : MonoBehaviour
{
    [SerializeField] private RarityProbabilityTable rarityTable;
    // 상점에서 판매할 아이템 목록입니다.
    private List<ItemDataModel> shopItems = new List<ItemDataModel>();

    // 상점 초기화
    private void Start()
    {
        // ItemManager가 로드될 때까지 기다립니다.
        if (ItemManager.Instance != null)
        {
            LoadShopItems();
        }
        else
        {
            Debug.LogError("ItemManager 인스턴스를 찾을 수 없습니다! ItemManager 스크립트가 씬에 있는지 확인하세요.");
        }

        // GameManager의 세션 리셋 이벤트에 상점 아이템 리로드 메서드를 구독합니다.
        if (GameManager.instance != null)
        {
            GameManager.instance.OnResetSession += LoadShopItems;
        }
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴 시 메모리 누수를 방지하기 위해 이벤트를 해제합니다.
        if (GameManager.instance != null)
        {
            GameManager.instance.OnResetSession -= LoadShopItems;
        }
    }

    // 아이템 구매
    public void BuyItem(int itemID)
    {
        ItemDataModel buyItem = shopItems.Find(item => item.id == itemID);
        if (buyItem.id != 0)
        {
            // 플레이어의 재화 확인 및 차감 로직
            // 인벤토리에 아이템 추가 로직
            bool result = ItemManager.Instance.TryAddItemToInventory(buyItem.id, buyItem.price, buyItem.sprite);

            if(result)
            {
                Debug.Log($"{buyItem.name} 아이템을 구매했습니다.");
            }
            else
            {
                Debug.Log($"{buyItem.name} 아이템 구매 실패");
            }
        }
    }

    public void RemoveItem(int itemID)
    {
        ItemDataModel removeItem = shopItems.Find(item => item.id == itemID);
        if (removeItem.id != 0)
        {
            // 플레이어의 인벤토리에서 아이템 제거 로직
            // 플레이어의 재화 추가 로직
            ItemManager.Instance.RemoveItemToInventory(removeItem.id);
            Debug.Log($"{removeItem.name} 아이템을 판매했습니다.");
        }
    }

    // 상점 아이템 로드
    private void LoadShopItems()
    {
        shopItems.Clear();
        GenerateShopItems();
        Debug.Log($"상점 아이템 {shopItems.Count}개 로드 완료.");
    }

    // 상점 리롤
    public void RerollShop()
    {
        GameManager.instance.BuyItem(5);
        shopItems.Clear();
        GenerateShopItems();
        Debug.Log("상점 아이템이 리롤되었습니다.");
    }

    private void GenerateShopItems()
    {
        if (rarityTable == null)
        {
            Debug.LogError("RarityProbabilityTable이 할당되지 않았습니다. 인스펙터 창에서 할당해주세요.");
            return;
        }

        LevelData currentLevel = ItemManager.Instance.GetCurrentLevelData();
        if (currentLevel == null)
        {
            Debug.LogError("레벨 데이터를 가져올 수 없습니다. GameManager의 초기화 상태를 확인하세요.");
            return;
        }

        // 현재 레벨보다 작거나 같은 레벨 중 가장 높은 레벨의 데이터를 찾습니다.
        RarityChances currentChances = rarityTable.chancesByLevel
            .Where(c => c.level <= currentLevel._level)
            .OrderByDescending(c => c.level)
            .FirstOrDefault();

        // 유효성 검사 추가
        if (EqualityComparer<RarityChances>.Default.Equals(currentChances, default(RarityChances)))
        {
            Debug.LogError("RarityProbabilityTable에 유효한 확률 데이터가 없습니다. 상점 아이템을 생성할 수 없습니다.");
            return; // 상점 아이템 생성 중단
        }

        // 확률 변수들을 ScriptableObject에서 가져온 값으로 초기화합니다.
        float commonChance = currentChances.commonChance;
        float rareChance = currentChances.rareChance;
        float uniqueChance = currentChances.uniqueChance;
        float legendaryChance = currentChances.legendaryChance;

        List<ItemDataModel> allItems = ItemManager.Instance.GetAllItems();
        if (allItems == null || allItems.Count == 0)
        {
            Debug.LogWarning("ItemManager에 로드된 아이템이 없습니다.");
            return;
        }

        // 아이템 중복 방지를 위해 전체 아이템 목록을 복사합니다.
        List<ItemDataModel> availableItems = new List<ItemDataModel>(allItems);

        shopItems.Clear(); // 기존 상점 아이템 초기화

        // 확률에 따라 3개의 아이템을 선택합니다.
        for (int i = 0; i < 3; i++)
        {
            if (availableItems.Count == 0)
            {
                Debug.LogWarning("더 이상 선택 가능한 아이템이 없습니다. 상점을 채울 수 없습니다.");
                break;
            }

            float rand = Random.value;
            Rarity rarityToSelect;

            if (rand < legendaryChance)
            {
                rarityToSelect = Rarity.Legendary;
            }
            else if (rand < legendaryChance + uniqueChance)
            {
                rarityToSelect = Rarity.Unique;
            }
            else if (rand < legendaryChance + uniqueChance + rareChance)
            {
                rarityToSelect = Rarity.Rare;
            }
            else
            {
                rarityToSelect = Rarity.Common;
            }

            // 해당 등급의 아이템만 필터링합니다.
            List<ItemDataModel> filteredItems = availableItems.Where(item => item.rarity == rarityToSelect).ToList();

            ItemDataModel selectedItem = default(ItemDataModel);

            if (filteredItems.Count > 0)
            {
                // 필터링된 아이템 중에서 랜덤으로 선택합니다.
                int randomIndex = Random.Range(0, filteredItems.Count);
                selectedItem = filteredItems[randomIndex];
                Debug.Log($"슬롯 {i + 1}: [ID: {selectedItem.id}] {selectedItem.name} ({selectedItem.rarity}) 추가.");
            }
            else
            {
                // 원하는 등급의 아이템이 없을 경우, 다른 등급에서 무작위로 선택합니다.
                Debug.LogWarning($"{rarityToSelect} 등급의 아이템이 없습니다. 다른 아이템을 추가합니다.");
                int randomIndex = Random.Range(0, availableItems.Count);
                selectedItem = availableItems[randomIndex];
                Debug.Log($"슬롯 {i + 1}: [ID: {selectedItem.id}] {selectedItem.name} ({selectedItem.rarity}) 추가 (대체).");
            }

            shopItems.Add(selectedItem);
            availableItems.Remove(selectedItem);
        }
    }
}


