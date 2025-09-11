using UnityEngine;
using System.Collections.Generic;
using System;

public class ItemManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ItemManager Instance { get; private set; }

    [SerializeField] private ItemData itemData;

    private PlayerStress playerStress;
    private SlotMachine targetSlotMachine;

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

    /// <summary>
    /// PlayerStress 컴포넌트를 ItemManager에 등록합니다.
    /// </summary>
    public void RegisterPlayerStress(PlayerStress stressComponent)
    {
        playerStress = stressComponent;
        Debug.Log("[ItemManager] PlayerStress가 등록되었습니다.");
    }

    /// <summary>
    /// SlotMachine 컴포넌트를 ItemManager에 등록합니다.
    /// </summary>
    public void RegisterSlotMachine(SlotMachine slotMachineComponent)
    {
        targetSlotMachine = slotMachineComponent;
        Debug.Log("[ItemManager] SlotMachine이 등록되었습니다.");
    }

    // 씬 전환 등으로 컴포넌트가 파괴될 때 참조를 null로 설정할 수 있습니다. (선택적)
    public void UnregisterPlayerStress() => playerStress = null;
    public void UnregisterSlotMachine() => targetSlotMachine = null;

    public void EffectStress(StressEffectData stressEffectData)
    {
        if (playerStress == null)
        {
            Debug.LogError("PlayerStress 컴포넌트가 ItemManager에 할당되지 않았습니다.");
            return;
        }

        float amount = stressEffectData.Amount;

        switch (stressEffectData.StressType)
        {
            case StressType.CurrentStress:
                if (amount > 0) playerStress.AddStress(amount);
                else playerStress.ReduceStress(Mathf.Abs(amount));
                break;

            case StressType.MaxStress:
                if (amount > 0) playerStress.IncreaseMaxStress(amount);
                else playerStress.DecreaseMaxStress(Mathf.Abs(amount));
                break;
        }

        Debug.Log($"스트레스 효과 적용 완료: 타입({stressEffectData.StressType}), 양({amount})");
    }

    public void EffectPattern(PatternEffectData patternEffectData)
    {
        Debug.Log("패턴 효과 진짜 발동!" + patternEffectData);
        if (targetSlotMachine != null)
        {
            targetSlotMachine.ApplyPatternEffect(patternEffectData);
        }
        else
        {
            Debug.LogWarning("[ItemManager] 아이템 효과를 적용할 SlotMachine이 지정되지 않았습니다.");
        }
    }

    public void EffectSymbol(SymbolEffectData symbolEffectData)
    {
        Debug.Log("심볼 효과 진짜 발동!" + symbolEffectData);
        if (targetSlotMachine != null)
        {
            targetSlotMachine.ApplySymbolEffect(symbolEffectData);
        }
        else
        {
            Debug.LogWarning("[ItemManager] 아이템 효과를 적용할 SlotMachine이 지정되지 않았습니다.");
        }
    }
}