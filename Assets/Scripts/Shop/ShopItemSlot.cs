using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용한다고 가정합니다.

public class ShopItemSlot : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Button buyButton;

    private ItemDataModel currentItemData;
    private Shop parentShop; // 어떤 상점의 슬롯인지 참조

    public void SetupSlot(ItemDataModel itemData, Shop shop)
    {
        currentItemData = itemData;
        parentShop = shop;

        if (itemData.sprite != null)
        {
            itemImage.sprite = itemData.sprite;
            itemImage.enabled = true; // 이미지가 있을 때만 보이도록
        }
        else
        {
            itemImage.enabled = false; // 이미지가 없으면 숨김
        }
        itemNameText.text = itemData.name;
        itemPriceText.text = $"{itemData.price} Gold"; // 예시로 Gold를 사용

        buyButton.onClick.RemoveAllListeners(); // 기존 리스너 제거
        buyButton.onClick.AddListener(OnBuyButtonClicked);
    }

    private void OnBuyButtonClicked()
    {
        if (currentItemData.id != 0)
        {
            Debug.Log($"{currentItemData.name} 구매 시도.");
            parentShop.BuyItem(currentItemData.id); // 상점 스크립트의 구매 메서드 호출
        }
    }

    // ClearSlot 메서드에서 currentItemData를 null로 설정할 수 없으므로, 대신 기본값을 할당합니다.
    public void ClearSlot()
    {
        currentItemData = default(ItemDataModel); // null 대신 기본값 할당
        itemImage.enabled = false;
        itemNameText.text = "Empty";
        itemPriceText.text = "";
        buyButton.onClick.RemoveAllListeners();
        buyButton.interactable = false;
    }
}