using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BetScreen : MonoBehaviour
{
    public SlotMachine slotMachine;
    private TextMeshProUGUI bettingGoldText;
    private int lastBettingGold = -1; // 이전 베팅 금액을 저장할 변수

    void Start()
    {
        bettingGoldText = GetComponent<TextMeshProUGUI>();
        // 초기 텍스트 업데이트
        UpdateBettingGoldText();
    }

    void Update()
    {
        // bettingGold 값이 변경되었는지 확인
        if (slotMachine != null && slotMachine.BettingGold != lastBettingGold)
        {
            UpdateBettingGoldText();
        }
    }

    private void UpdateBettingGoldText()
    {
        lastBettingGold = slotMachine.BettingGold;
        bettingGoldText.text = lastBettingGold.ToString() + " $";
    }
}
