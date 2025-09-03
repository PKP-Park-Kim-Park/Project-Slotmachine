using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI tokenText;
    private void Start()
    {
        // GameManager 인스턴스가 유효한지 확인합니다.
        if (GameManager.instance != null && GameManager.instance.money != null)
        {
            // Money 클래스의 OnMoneyChanged 이벤트에 UpdateUI 메서드를 등록합니다.
            GameManager.instance.money.OnMoneyChanged += UpdateUI;

            // 초기 UI를 업데이트하여 시작 시 올바른 값이 표시되도록 합니다.
            UpdateUI();
        }
    }
    private void UpdateUI()
    {
        if (goldText != null && tokenText != null)
        {
            goldText.text = $"{GameManager.instance.money._gold}";
            tokenText.text = $"{GameManager.instance.money._token}";
        }
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 구독을 해제하여 메모리 누수를 방지합니다.
        if (GameManager.instance != null && GameManager.instance.money != null)
        {
            GameManager.instance.money.OnMoneyChanged -= UpdateUI;
        }
    }
}
