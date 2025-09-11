using UnityEngine;

public class TestRunner : MonoBehaviour
{
    [SerializeField] private int itemToTestId = 1;
    [SerializeField] private int skillToTestId = 1;
    private Shop shop;
    private SkillManager skillManager;

    private void Start()
    {
        shop = GetComponent<Shop>();
        skillManager= GetComponent<SkillManager>();

        Debug.Log("--- 상점 및 아이템 사용 테스트 준비 완료 ---");
    }

    private void Update()
    {
        // 1번 키를 누르면 아이템을 구매하고 사용합니다.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            shop.BuyItem(itemToTestId);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            skillManager.ToggleSkill(skillToTestId);
        }

        // R 키를 누르면 상점을 판매합니다.
        if (Input.GetKeyDown(KeyCode.R))
        {
            shop.RemoveItem(itemToTestId);
        }
        // T 키를 누르면 상점을 리롤합니다.
        if (Input.GetKeyDown(KeyCode.T))
        {
            shop.RerollShop();
        }
    }
}