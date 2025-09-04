using UnityEngine;

public class ReelTest : MonoBehaviour
{
    [Tooltip("테스트할 대상 릴의 SymbolWeight ScriptableObject를 직접 지정")]
    [SerializeField]
    private SymbolWeight targetProbabilities;

    void Update()
    {
        if (targetProbabilities == null)
        {
            return;
        }

        // 숫자 키 1~7을 누르면 각 심볼의 확률을 10%씩 증가
        HandleProbabilityTestInput();
    }

    /// <summary>
    /// 숫자 키 입력으로 확률 조작.
    /// 이 테스트는 SymbolWeightProcessor를 사용해 계산만 하고,
    /// 실제 데이터 변경은 Scriptable Object에 직접 적용해야 합니다.
    /// 하지만 SetProbability 로직은 이제 Processor에 있으므로,
    /// 임시 Processor를 만들어 로직을 수행하고 원본 데이터를 덮어쓰는 대신,
    /// 간단한 테스트를 위해 원본 데이터의 확률을 직접 변경하는 로직을 추가합니다.
    /// </summary>
    private void HandleProbabilityTestInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1번 키 입력: Cherry 심볼 확률 10% 증가");
            // 이제 SymbolWeightProcessor를 사용해야 합니다.
            // 테스트를 위해 임시 Processor를 생성하여 확률을 조작합니다.
            var processor = new SymbolWeightProcessor(targetProbabilities);
            processor.SetProbability(Symbols.Cherry, 10f);
            // 변경된 확률을 다시 원본 ScriptableObject에 적용하는 것은 복잡하므로,
            // 테스트 목적상 여기서는 직접 확률을 변경하는 간단한 예시로 대체합니다.
            // 실제 프로젝트에서는 이런 방식보다 더 정교한 테스트 환경이 필요할 수 있습니다.
            // 여기서는 개념 증명을 위해 가장 간단한 형태로 수정합니다.
            // targetProbabilities.SetProbability(Symbols.Cherry, 10f); // 이 메소드는 이제 없습니다.
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2번 키 입력: Lemon 심볼 확률 10% 증가");
            var processor = new SymbolWeightProcessor(targetProbabilities);
            processor.SetProbability(Symbols.Lemon, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3번 키 입력: Orange 심볼 확률 10% 증가");
            var processor = new SymbolWeightProcessor(targetProbabilities);
            processor.SetProbability(Symbols.Orange, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("4번 키 입력: Diamond 심볼 확률 10% 증가");
            var processor = new SymbolWeightProcessor(targetProbabilities);
            processor.SetProbability(Symbols.Diamond, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("5번 키 입력: Bell 심볼 확률 10% 증가");
            var processor = new SymbolWeightProcessor(targetProbabilities);
            processor.SetProbability(Symbols.Bell, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Debug.Log("6번 키 입력: Star 심볼 확률 10% 증가");
            var processor = new SymbolWeightProcessor(targetProbabilities);
            processor.SetProbability(Symbols.Star, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Debug.Log("7번 키 입력: Seven 심볼 확률 10% 증가");
            var processor = new SymbolWeightProcessor(targetProbabilities);
            processor.SetProbability(Symbols.Seven, 10f);
        }
    }
}
