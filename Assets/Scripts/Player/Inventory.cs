using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<int> inventorySpace = new List<int>();
    private int inventorySpaceMaxCount = 5;

    private void Start()
    {
        ItemManager.Instance.OnCheckCanAddInventoryItem += CanGetItem;
        ItemManager.Instance.OnRemoveInventoryItem += DeleteItem;
        ItemManager.Instance.OnAddInventoryItem += GetItem;
    }

    private void OnDestroy()
    {
        ItemManager.Instance.OnCheckCanAddInventoryItem -= CanGetItem;
        ItemManager.Instance.OnRemoveInventoryItem -= DeleteItem;
        ItemManager.Instance.OnAddInventoryItem -= GetItem;
    }

    public bool CanGetItem(int id)
    {
        if (inventorySpace.Count >= inventorySpaceMaxCount)
        {
            Debug.Log("인벤토리 꽉 참");
            return false;
        }

        foreach (int i in inventorySpace)
        {
            if (i == id)
            {
                Debug.Log("아이템 중복");
                return false;
            }
        }

        return true;
    }

    public void GetItem(int id, Image image)
    {
        Debug.Log("아이템 추가 " + id);
        inventorySpace.Add(id);

        //디버그용
        foreach(int i in inventorySpace)
        {
            Debug.Log("현제 소유 아이템 ::: " + i);
        }
    }

    public bool DeleteItem(int id)
    {
        if(inventorySpace.Contains(id))
        {
            inventorySpace.Remove(id);
            Debug.Log("아이템 삭제 " + id);
            return true;
        }
        else
        {
            Debug.Log("삭제할 아이템이 없음 " + id);
            return false;
        }
    }
}
