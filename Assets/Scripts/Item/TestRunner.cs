using UnityEngine;
using System.Linq; // Linq 사용을 위해 추가

public class TestRunner : MonoBehaviour
{
    [SerializeField] private int itemToTestId = 1;
    [SerializeField] private int skillToTestId = 1;
    private Shop shop;
    private SkillManager skillManager;

    [Header("원본 데이터 확인용")]
    [SerializeField] private PatternRewardOdds patternOddsData; // 패턴 배율 원본 SO
    [SerializeField] private SymbolRewardOdds symbolOddsData;   // 심볼 배율 원본 SO
    [SerializeField] private SymbolWeight symbolWeightData;     // 심볼 가중치 원본 SO

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

            // --- 테스트 코드 ---
            // 아이템 사용 직후 원본 ScriptableObject의 값을 확인합니다.
            // 아이템 효과가 즉시 적용된다고 가정합니다. (ItemManager의 UseItem 로직에 따라 달라질 수 있음)
            // 예: 1번 아이템이 'Seven' 심볼 배율과 'Column_3' 패턴 배율을 변경한다고 가정
            if (patternOddsData != null)
            {
                Debug.Log($"[테스트] 원본 패턴 SO 'Column_3' 배율: {patternOddsData.rewardPatterns.Find(p => p.patternType == Patterns.Column_3).rewardOddsMultiplier}");
            }
            if (symbolOddsData != null)
            {
                Debug.Log($"[테스트] 원본 심볼 SO 'Banana' 배율: {symbolOddsData.rewardSymbols.Find(s => s.symbolType == Symbols.Banana).rewardOddsMultiplier}");
            }
            // --- 테스트 종료 ---
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            skillManager.ToggleSkill(skillToTestId);
        }

        // R 키를 누르면 상점을 리롤합니다.
        if (Input.GetKeyDown(KeyCode.R))
        {
            shop.RemoveItem(itemToTestId);
        }
    }
}