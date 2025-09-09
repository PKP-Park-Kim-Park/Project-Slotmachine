using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ItemManager Instance { get; private set; }

    // 모든 아이템 데이터를 담고 있는 ScriptableObject
    [SerializeField] private ItemData itemData;

    // 모든 아이템 데이터를 저장할 배열
    private List<ItemDataModel> allItems = new List<ItemDataModel>();

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
            Debug.LogError("ItmeData ScriptableObject가 할당되지 않았습니다!");
        }
    }

    // 아이템 ID로 아이템 데이터 모델을 가져오는 메서드
    public ItemDataModel GetItemData(int itemID)
    {
        return allItems.Find(item => item.id == itemID);
    }
}