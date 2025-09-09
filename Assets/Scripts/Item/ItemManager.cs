using UnityEngine;
using System.Collections.Generic;
using System;

public class ItemManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ItemManager Instance { get; private set; }

    // 모든 아이템 데이터를 담고 있는 ScriptableObject
    [SerializeField] private ItemData itemData;

    // 모든 아이템 데이터를 저장할 배열
    private List<ItemDataModel> allItems = new List<ItemDataModel>();

    // 외부(GameManager)에서 LevelData를 요청할 때 사용하는 이벤트
    public Func<LevelData> OnRequestLevelData;

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
}