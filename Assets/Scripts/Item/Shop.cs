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
        shopItems.Clear();
        GenerateShopItems();
        Debug.Log($"상점 아이템 {shopItems.Count}개 로드 완료.");

    }

    // 상점 리롤
    public void RerollShop()
    {
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

        RarityChances currentChances = rarityTable.chancesByLevel.FirstOrDefault(c => currentLevel._level == c.level);

        if (currentChances.level == 0)
        {
            Debug.LogWarning($"레벨 {currentLevel._level}에 해당하는 확률 데이터가 없습니다. 기본 확률을 적용합니다.");
            // 기본 확률을 적용하기 위해 리스트의 첫 번째 항목을 가져옵니다.
            // 이 로직은 리스트에 최소한 하나의 항목이 있고 오름차순으로 정렬되어 있음을 가정합니다.
            currentChances = rarityTable.chancesByLevel.FirstOrDefault();
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

        // 확률에 따라 3개의 아이템을 선택합니다.
        for (int i = 0; i < 3; i++)
        {
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

            // 선택된 희귀도를 확인합니다.
            Debug.Log($"슬롯 {i + 1}에 선택된 희귀도: {rarityToSelect}");

            List<ItemDataModel> filteredItems = allItems.Where(item => item.rarity == rarityToSelect).ToList();

            // 해당 등급의 아이템이 있을 경우, 그 중에서 랜덤으로 추가
            if (filteredItems.Count > 0)
            {
                int randomIndex = Random.Range(0, filteredItems.Count);
                shopItems.Add(filteredItems[randomIndex]);
            }
            // 해당 등급의 아이템이 없을 경우, 모든 아이템 중에서 랜덤으로 추가
            else
            {
                Debug.LogWarning($"{rarityToSelect} 등급의 아이템이 없습니다. 다른 아이템을 추가합니다.");
                shopItems.Add(allItems[Random.Range(0, allItems.Count)]);
            }
        }
    }

    // 상점 아이템 목록을 가져오는 public 메서드
    public List<ItemDataModel> GetShopItems()
    {
        return shopItems;
    }

}