using UnityEngine;

public class ReelTest : MonoBehaviour
{
    [Tooltip("테스트할 대상 릴 지정")]
    [SerializeField]
    private Reel targetReel;

    void Update()
    {
        if (targetReel == null)
        {
            return;
        }

        // 숫자 키 1~7을 누르면 각 심볼의 확률을 10%씩 증가
        HandleProbabilityTestInput();
    }

    /// <summary>
    /// 숫자 키 입력으로 확률 조작
    /// </summary>
    private void HandleProbabilityTestInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1번 키 입력: Cherry 심볼 확률 10% 증가");
            targetReel.Probabilities.SetProbability(Symbols.Cherry, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2번 키 입력: Lemon 심볼 확률 10% 증가");
            targetReel.Probabilities.SetProbability(Symbols.Lemon, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3번 키 입력: Orange 심볼 확률 10% 증가");
            targetReel.Probabilities.SetProbability(Symbols.Orange, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("4번 키 입력: Diamond 심볼 확률 10% 증가");
            targetReel.Probabilities.SetProbability(Symbols.Diamond, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("5번 키 입력: Bell 심볼 확률 10% 증가");
            targetReel.Probabilities.SetProbability(Symbols.Bell, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Debug.Log("6번 키 입력: Star 심볼 확률 10% 증가");
            targetReel.Probabilities.SetProbability(Symbols.Star, 10f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Debug.Log("7번 키 입력: Seven 심볼 확률 10% 증가");
            targetReel.Probabilities.SetProbability(Symbols.Seven, 10f);
        }
    }
}
