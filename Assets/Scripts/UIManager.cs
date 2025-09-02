using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI tokenText;
    private void Start()
    {
        // GameManager �ν��Ͻ��� ��ȿ���� Ȯ���մϴ�.
        if (GameManager.instance != null && GameManager.instance.money != null)
        {
            // Money Ŭ������ OnMoneyChanged �̺�Ʈ�� UpdateUI �޼��带 ����մϴ�.
            GameManager.instance.money.OnMoneyChanged += UpdateUI;

            // �ʱ� UI�� ������Ʈ�Ͽ� ���� �� �ùٸ� ���� ǥ�õǵ��� �մϴ�.
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
        // ������Ʈ�� �ı��� �� �̺�Ʈ ������ �����Ͽ� �޸� ������ �����մϴ�.
        if (GameManager.instance != null && GameManager.instance.money != null)
        {
            GameManager.instance.money.OnMoneyChanged -= UpdateUI;
        }
    }
}
