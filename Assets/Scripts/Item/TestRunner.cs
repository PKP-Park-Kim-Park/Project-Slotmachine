using UnityEngine;

public class TestRunner : MonoBehaviour
{
    [SerializeField] private int itemToTestId = 1;
    private ItemManager itemManager;
    private UseItem useItem;
    private Shop shop;

    private void Start()
    {
        // ItemManager와 Shop 컴포넌트를 가져옵니다.
        itemManager = ItemManager.Instance;
        shop = GetComponent<Shop>();

        if (itemManager == null || shop == null)
        {
            Debug.LogError("필요한 컴포넌트 중 하나가 누락되었습니다. ItemManager, Shop 컴포넌트가 모두 같은 게임 오브젝트에 있는지 확인하세요.");
            return;
        }

        // UseItem 클래스의 새 인스턴스를 만듭니다.
        useItem = new UseItem();

        Debug.Log("--- 상점 및 아이템 사용 테스트 준비 완료 ---");
        Debug.Log("1번 키를 눌러 상점의 첫 번째 아이템을 구매하고 사용하세요.");
        Debug.Log("R 키를 눌러 상점을 리롤하세요.");
    }

    private void Update()
    {
        // 1번 키를 누르면 아이템을 구매하고 사용합니다.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BuyAndUseFirstItem();
        }

        // R 키를 누르면 상점을 리롤합니다.
        if (Input.GetKeyDown(KeyCode.R))
        {
            RerollShop();
        }
    }

    private void BuyAndUseFirstItem()
    {
        Debug.Log("--- 아이템 구매 및 사용 시도 ---");

        // 상점에 아이템이 있는지 확인합니다.
        if (shop.GetShopItems().Count > 0)
        {
            ItemDataModel itemFromShop = shop.GetShopItems()[0];
            Debug.Log($"상점에서 '{itemFromShop.name}' 아이템을 구매 시도합니다.");
            shop.BuyItem(itemFromShop.id);

            Debug.Log($"구매한 아이템 '{itemFromShop.name}'을(를) 사용합니다.");
            useItem.Use(itemFromShop);
        }
        else
        {
            Debug.LogWarning("상점에 판매할 아이템이 없습니다. R 키를 눌러 상점을 리롤하세요.");
        }
    }

    private void RerollShop()
    {
        Debug.Log("--- 상점 리롤 시도 ---");
        shop.RerollShop();
    }
}