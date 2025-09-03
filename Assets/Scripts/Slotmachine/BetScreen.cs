using UnityEngine;
using TMPro;

public class BetScreen : MonoBehaviour
{
    public SlotMachine slotMachine;
    private TextMeshProUGUI bettingGoldText;

    private void Awake()
    {
        bettingGoldText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (slotMachine != null)
        {
            slotMachine.OnBetGoldChanged += BetGoldChanged;
            BetGoldChanged(slotMachine.BettingGold);
        }
    }

    private void OnDisable()
    {
        if (slotMachine != null)
        {
            slotMachine.OnBetGoldChanged -= BetGoldChanged;
        }
    }

    private void BetGoldChanged(int newBettingGold)
    {
        // 1,000 단위 쉼표 추가
        bettingGoldText.text = newBettingGold.ToString("N0") + " $";
    }
}
