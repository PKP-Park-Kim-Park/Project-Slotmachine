using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ItemManager Instance { get; private set; }

    // 모든 아이템 데이터를 담고 있는 ScriptableObject
    [SerializeField] private ItmeData itmeData;

    // 모든 아이템 데이터를 저장할 배열
    private List<ItemDataModel> allItems = new List<ItemDataModel>();

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            // 씬이 전환되어도 파괴되지 않게 설정
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
        if (itmeData != null)
        {
            allItems = itmeData.itemDataModels;
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
        // 람다식을 사용하여 리스트에서 ID에 맞는 아이템을 찾습니다.
        return allItems.Find(item => item.id == itemID);
    }
}