using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<int> inventorySpace = new List<int>();
    private int inventorySpaceMaxCount = 5;

    public bool CanGetItem(int id)
    {
        if (inventorySpace.Count > inventorySpaceMaxCount)
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

        GetItem(id);
        return true;
    }

    public void GetItem(int id)
    {
        Debug.Log("아이템 추가 " + id);
        inventorySpace.Add(id);
    }

    public void DeleteItem(int id)
    {
        if(inventorySpace.Contains(id))
        {
            inventorySpace.Remove(id);
            Debug.Log("아이템 삭제 " + id);
        }

        Debug.Log("삭제할 아이템이 없음 " + id);
    }
}
