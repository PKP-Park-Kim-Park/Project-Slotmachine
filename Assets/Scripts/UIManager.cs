using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI tokenText;

    public TextMeshProUGUI stressText;
    public Slider stressSlider;

    [SerializeField] private PlayerStress playerStress;
    private void Start()
    {
        // GameManager 인스턴스가 유효한지 확인합니다.
        if (GameManager.instance != null && GameManager.instance.money != null)
        {
            // Money 클래스의 OnMoneyChanged 이벤트에 UpdateUI 메서드를 등록합니다.
            GameManager.instance.money.OnMoneyChanged += UpdateGoldTokenUI;
            UpdateGoldTokenUI();
        }
        if (playerStress != null)
        {
            playerStress.OnStressChanged.AddListener(UpdateStressUI);
            if (stressSlider != null)
            {
                stressSlider.maxValue = playerStress.maxStress;
            }
            UpdateStressUI(playerStress.currentStress);
        }
    }
    private void UpdateGoldTokenUI()
    {
        if (goldText != null && tokenText != null)
        {
            goldText.text = $"{GameManager.instance.money._gold}";
            tokenText.text = $"{GameManager.instance.money._token}";
        }
    }
    private void UpdateStressUI(float currentStress)
    {
        if (stressText != null)
        {
            // 스트레스 값을 소수점 없이 정수로 표시합니다.
            stressText.text = $"{Mathf.RoundToInt(currentStress)}";
        }
        if (stressSlider != null)
        {
            stressSlider.value = currentStress;
        }
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 구독을 해제하여 메모리 누수를 방지합니다.
        if (GameManager.instance != null && GameManager.instance.money != null)
        {
            GameManager.instance.money.OnMoneyChanged -= UpdateGoldTokenUI;
        }
        // 오브젝트가 파괴될 때 스트레스 관련 이벤트 구독을 해제합니다.
        if (playerStress != null)
        {
            playerStress.OnStressChanged.RemoveListener(UpdateStressUI);
        }
    }
}
