using UnityEngine;
using System.Collections.Generic;
using System;

public class ItemManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ItemManager Instance { get; private set; }

    [SerializeField] private ItemData itemData; 
    private List<IItemEffectReceiver> effectReceivers = new List<IItemEffectReceiver>();
    // 모든 아이템 데이터를 저장할 배열
    private List<ItemDataModel> allItems = new List<ItemDataModel>();

    // 외부(GameManager)에서 LevelData를 요청할 때 사용하는 이벤트
    public Func<LevelData> OnRequestLevelData;

    public Func<int, bool> OnCheckCanBuyItem;
    public Func<int, bool> OnCheckCanAddInventoryItem;
    public Func<int, bool> OnRemoveInventoryItem;

    public event Action<int, Sprite> OnAddInventoryItem;
    public event Action<int> OnBuyItem;

    private UseItem useItem;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadAllItems();

        useItem = new UseItem();

        useItem.OnPatternEffectReady += EffectPattern;
        useItem.OnSymbolEffectReady += EffectSymbol;
        useItem.OnStressEffectReady += EffectStress;
    }

    // ScriptableObject에서 모든 아이템 데이터를 로드
    private void LoadAllItems()
    {
        if (itemData != null)
        {
            allItems = itemData.itemDataModels;
            Debug.Log($"아이템 데이터 {allItems.Count}개 로드 완료.");
        }
        else
        {
            Debug.LogError("ItemData ScriptableObject가 할당되지 않았습니다!");
        }
    }

    // 아이템 ID로 아이템 데이터 모델을 가져오는 메서드
    public ItemDataModel GetItemData(int itemID)
    {
        return allItems.Find(item => item.id == itemID);
    }

    internal List<ItemDataModel> GetAllItems()
    {
        return allItems;
    }

    public LevelData GetCurrentLevelData()
    {
        if (OnRequestLevelData != null)
        {
            return OnRequestLevelData.Invoke();
        }
        return null;
    }

    public bool TryAddItemToInventory(int id, int price, Sprite image)
    {
        bool result = OnCheckCanBuyItem.Invoke(price);
        bool result2 = OnCheckCanAddInventoryItem.Invoke(id);

        if(result2 == false || result == false)
        {
            return false;
        }

        Debug.Log(id + " : 구매 조건 충족");

        OnAddInventoryItem?.Invoke(id, image);
        OnBuyItem?.Invoke(price);

        ItemDataModel item = allItems.Find(item => item.id == id);
        useItem.Use(item);

        return true;
    }

    public void RemoveItemToInventory(int id)
    {
        bool result = OnRemoveInventoryItem.Invoke(id);

        if(result == true)
        {
            ItemDataModel item = allItems.Find(item => item.id == id);
            useItem.Remove(item);
        }
    }

    // --- 외부 컴포넌트 등록/해제 ---

    public void RegisterReceiver(IItemEffectReceiver receiver)
    {
        if (!effectReceivers.Contains(receiver))
        {
            effectReceivers.Add(receiver);
            // Debug.Log($"[ItemManager] {receiver.GetType().Name}가 등록되었습니다.");
        }
    }

    public void UnregisterReceiver(IItemEffectReceiver receiver)
    {
        effectReceivers.Remove(receiver);
        Debug.Log($"[ItemManager] {receiver.GetType().Name}가 등록 해제되었습니다.");
    }

    public void EffectStress(StressEffectData stressEffectData)
    {
        ApplyEffect(EffectType.Stress, stressEffectData);
    }

    public void EffectPattern(PatternEffectData patternEffectData)
    {
        ApplyEffect(EffectType.Pattern, patternEffectData);
    }

    public void EffectSymbol(SymbolEffectData symbolEffectData)
    {
        ApplyEffect(EffectType.Symbol, symbolEffectData);
    }

    private void ApplyEffect(EffectType type, object data)
    {
        foreach (var receiver in effectReceivers)
        {
            if (receiver.CanReceive(type))
            {
                receiver.ReceiveEffect(type, data);
            }
        }
    }
}