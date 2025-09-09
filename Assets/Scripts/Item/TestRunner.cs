using UnityEngine;

public class TestRunner : MonoBehaviour
{
    [SerializeField] private int itemToTestId = 1;
    private ItemManager itemManager;
    private UseItem useItem;
    private Shop shop;

    private void Start()
    {
        shop = GetComponent<Shop>();

        Debug.Log("--- 상점 및 아이템 사용 테스트 준비 완료 ---");
        Debug.Log("1번 키를 눌러 상점의 첫 번째 아이템을 구매하고 사용하세요.");
        Debug.Log("R 키를 눌러 상점을 리롤하세요.");
    }

    private void Update()
    {
        // 1번 키를 누르면 아이템을 구매하고 사용합니다.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            shop.BuyItem(itemToTestId);
        }

        // R 키를 누르면 상점을 리롤합니다.
        if (Input.GetKeyDown(KeyCode.R))
        {
            shop.RemoveItem(itemToTestId);
        }
    }
}