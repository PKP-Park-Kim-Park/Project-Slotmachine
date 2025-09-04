using UnityEngine;

public class ReelTest : MonoBehaviour
{
    [Tooltip("테스트할 대상 릴의 SymbolWeight ScriptableObject를 직접 지정")]
    [SerializeField] private SymbolWeight targetProbabilities;
    private SymbolWeightProcessor testProcessor;

    void Update()
    {
        if (targetProbabilities == null)
        {
            Debug.LogWarning("테스트할 SymbolWeight를 할당해주세요.");
            return;
        }

        // 숫자 키 1~7을 누르면 각 심볼의 확률을 10%씩 증가
        HandleProbabilityTestInput();
    }

    private void OnDisable()
    {
        // Play 모드를 종료할 때 testProcessor를 null로 만들어 직렬화 문제를 방지합니다.
        testProcessor = null;
    }

    /// <summary>
    /// 숫자 키 입력으로 확률 조작.
    /// 이 테스트는 임시 SymbolWeightProcessor를 사용하여 확률을 조작하고 결과를 로깅합니다.
    /// SlotMachine이 중앙에서 확률을 관리하므로, 이 테스트는 독립적인 확률 계산 검증용입니다.
    /// </summary>
    private void HandleProbabilityTestInput()
    {
        // 테스트 프로세서가 없으면 초기화
        if (testProcessor == null)
        {
            testProcessor = new SymbolWeightProcessor(targetProbabilities);
            Debug.Log("테스트용 SymbolWeightProcessor가 초기화되었습니다.");
            testProcessor.LogAllProbabilities();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1번 키 입력: Cherry 심볼 확률 10% 증가");
            testProcessor.SetProbability(Symbols.Cherry, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2번 키 입력: Lemon 심볼 확률 10% 증가");
            testProcessor.SetProbability(Symbols.Lemon, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3번 키 입력: Orange 심볼 확률 10% 증가");
            testProcessor.SetProbability(Symbols.Orange, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("4번 키 입력: Diamond 심볼 확률 10% 증가");
            testProcessor.SetProbability(Symbols.Diamond, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("5번 키 입력: Bell 심볼 확률 10% 증가");
            testProcessor.SetProbability(Symbols.Bell, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Debug.Log("6번 키 입력: Star 심볼 확률 10% 증가");
            testProcessor.SetProbability(Symbols.Star, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Debug.Log("7번 키 입력: Seven 심볼 확률 10% 증가");
            testProcessor.SetProbability(Symbols.Seven, 10f);
        }
    }
}
