using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shop : MonoBehaviour
{
    [SerializeField] private RarityProbabilityTable rarityTable;
    // 상점에서 판매할 아이템 목록입니다.
    private List<ItemDataModel> shopItems = new List<ItemDataModel>();

    // --- UI 연동을 위한 추가 필드 ---
    [Header("Shop UI Settings")]
    [SerializeField] private Transform shopItemParent; // UI 슬롯들이 생성될 부모 Transform (Vertical Layout Group 등이 있는)
    [SerializeField] private GameObject shopItemSlotPrefab; // ShopItemSlot 스크립트가 붙어있는 프리팹

    private List<ShopItemSlot> currentUISlots = new List<ShopItemSlot>(); // 현재 활성화된 UI 슬롯들을 관리
    // -----------------------------------

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

    public void BuyItem(int itemID)
    {
        ItemDataModel buyItem = shopItems.Find(item => item.id == itemID);

        if (buyItem.id != 0)
        {
            // 플레이어의 재화 확인 및 차감 로직 (예시)
            if (GameManager.instance.money._gold < buyItem.price)
            {
                Debug.Log("골드가 부족합니다!");
                return;
            }
            // _gold 값을 직접 변경하지 않고 SpendGold 메서드 사용
            bool spent = GameManager.instance.money.SpendGold(buyItem.price);
            if (!spent)
            {
                Debug.Log("골드 차감에 실패했습니다!");
                return;
            }

            bool result = ItemManager.Instance.TryAddItemToInventory(buyItem.id, buyItem.price, buyItem.sprite);

            if (result)
            {
                Debug.Log($"{buyItem.name} 아이템을 구매했습니다.");
                // 상점에서 구매한 아이템 UI 슬롯을 업데이트하거나 제거 (필요하다면)
                UpdateShopUI(); // 상점 UI를 다시 그립니다.
            }
            else
            {
                Debug.Log($"{buyItem.name} 아이템 구매 실패");
            }
        }
        else
        {
            Debug.LogWarning($"ID {itemID}의 아이템을 상점 목록에서 찾을 수 없습니다.");
        }
    }

    public void RemoveItem(int itemID)
    {
        ItemDataModel removeItem = shopItems.Find(item => item.id == itemID);
        if (removeItem.id != 0)
        {
            ItemManager.Instance.RemoveItemToInventory(removeItem.id);
            Debug.Log($"{removeItem.name} 아이템을 판매했습니다.");
            // 판매 후 상점 UI 업데이트 (필요하다면)
        }
        else
        {
            Debug.LogWarning($"ID {itemID}의 아이템을 상점 목록에서 찾을 수 없습니다.");
        }
    }

    // 상점 아이템 로드 (수정됨)
    private void LoadShopItems()
    {
        shopItems.Clear();
        GenerateShopItems();
        Debug.Log($"상점 아이템 {shopItems.Count}개 로드 완료.");

        // --- 상점 UI 업데이트 호출 ---
        UpdateShopUI();
    }

    // 상점 리롤 (수정됨)
    public void RerollShop()
    {
        GameManager.instance.BuyItem(5); 
        shopItems.Clear();
        GenerateShopItems();
        Debug.Log("상점 아이템이 리롤되었습니다.");

        // --- 상점 UI 업데이트 호출 ---
        UpdateShopUI();
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

        RarityChances currentChances = rarityTable.chancesByLevel
            .Where(c => c.level <= currentLevel._level)
            .OrderByDescending(c => c.level)
            .FirstOrDefault();

        if (EqualityComparer<RarityChances>.Default.Equals(currentChances, default(RarityChances)))
        {
            Debug.LogError("RarityProbabilityTable에 유효한 확률 데이터가 없습니다. 상점 아이템을 생성할 수 없습니다.");
            return;
        }

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

        List<ItemDataModel> availableItems = new List<ItemDataModel>(allItems);

        shopItems.Clear();

        for (int i = 0; i < 3; i++) // 3개의 아이템을 상점에 표시
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

            List<ItemDataModel> filteredItems = availableItems.Where(item => item.rarity == rarityToSelect).ToList();

            ItemDataModel selectedItem = default(ItemDataModel);

            if (filteredItems.Count > 0)
            {
                int randomIndex = Random.Range(0, filteredItems.Count);
                selectedItem = filteredItems[randomIndex];
                Debug.Log($"슬롯 {i + 1}: [ID: {selectedItem.id}] {selectedItem.name} ({selectedItem.rarity}) 추가.");
            }
            else
            {
                Debug.LogWarning($"{rarityToSelect} 등급의 아이템이 없습니다. 다른 아이템을 추가합니다.");
                int randomIndex = Random.Range(0, availableItems.Count);
                selectedItem = availableItems[randomIndex];
                Debug.Log($"슬롯 {i + 1}: [ID: {selectedItem.id}] {selectedItem.name} ({selectedItem.rarity}) 추가 (대체).");
            }

            shopItems.Add(selectedItem);
            availableItems.Remove(selectedItem);
        }
    }

    // --- UI 업데이트 로직 (새로 추가) ---
    private void UpdateShopUI()
    {
        // 기존 UI 슬롯들을 모두 제거하거나 비활성화
        foreach (ShopItemSlot slot in currentUISlots)
        {
            Destroy(slot.gameObject); // GameObject를 파괴
        }
        currentUISlots.Clear();

        // 새로 생성된 상점 아이템 목록을 기반으로 UI 슬롯 생성 및 설정
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (shopItemSlotPrefab == null || shopItemParent == null)
            {
                Debug.LogError("ShopItemSlot 프리팹 또는 shopItemParent가 할당되지 않았습니다. UI 설정을 확인해주세요.");
                break;
            }

            GameObject slotGO = Instantiate(shopItemSlotPrefab, shopItemParent);
            ShopItemSlot slot = slotGO.GetComponent<ShopItemSlot>();
            if (slot != null)
            {
                slot.SetupSlot(shopItems[i], this); // 아이템 데이터와 Shop 스크립트 참조를 전달
                currentUISlots.Add(slot);
            }
            else
            {
                Debug.LogError("ShopItemSlot 프리팹에 ShopItemSlot 스크립트가 없습니다.");
            }
        }
    }
}