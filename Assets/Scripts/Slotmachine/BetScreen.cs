using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BetScreen : MonoBehaviour
{
    public SlotMachine slotMachine; // SlotMachine 오브젝트를 연결
    private TextMeshProUGUI bettingGoldText;

    void Start()
    {
        bettingGoldText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (slotMachine != null && bettingGoldText != null)
        {
            // 값을 텍스트로 표시
            bettingGoldText.text = slotMachine.BettingGold.ToString() + " $";
        }
    }
}
