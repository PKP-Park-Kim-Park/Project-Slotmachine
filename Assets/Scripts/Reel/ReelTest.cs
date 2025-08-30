using UnityEngine;
using System.Collections;
using System.Linq;

public class ReelTest : MonoBehaviour
{
    // 테스트할 Reel 컴포넌트를 할당할 변수
    [SerializeField] private Reel targetReel;

    void Update()
    {
        // 'Q' 키를 눌렀을 때만 RelocateSymbols() 함수를 호출합니다.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(TestStopSpinCoroutine());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R 키가 눌렸습니다. 릴 회전을 시작합니다.");

            // Reel의 StartSpin 코루틴을 호출
            StartCoroutine(targetReel.StartSpin());
        }

        // '1' 키를 누르면 Cherry 심볼의 확률을 10% 올립니다.
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            Debug.Log("1 키 눌림: Cherry 심볼 확률 10% 증가");
            targetReel.SetProbability(Symbols.Cherry, 10f);
        }
        // '2' 키를 누르면 Lemon 심볼의 확률을 10% 올립니다.
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            Debug.Log("2 키 눌림: Lemon 심볼 확률 10% 증가");
            targetReel.SetProbability(Symbols.Lemon, 10f);
        }
        // '3' 키를 누르면 Orange 심볼의 확률을 10% 올립니다.
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            Debug.Log("3 키 눌림: Orange 심볼 확률 10% 증가");
            targetReel.SetProbability(Symbols.Orange, 10f);
        }
        // '4' 키를 누르면 Orange 심볼의 확률을 10% 올립니다.
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            Debug.Log("4 키 눌림: Diamond 심볼 확률 10% 증가");
            targetReel.SetProbability(Symbols.Diamond, 10f);
        }
        // '5' 키를 누르면 Bell 심볼의 확률을 10% 올립니다.
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            Debug.Log("5 키 눌림: Bell 심볼 확률 10% 증가");
            targetReel.SetProbability(Symbols.Bell, 10f);
        }
        // '6' 키를 누르면 Star 심볼의 확률을 10% 올립니다.
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            Debug.Log("6 키 눌림: Star 심볼 확률 10% 증가");
            targetReel.SetProbability(Symbols.Star, 10f);
        }
        // '7' 키를 누르면 Seven 심볼의 확률을 10% 올립니다.
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            Debug.Log("7 키 눌림: Seven 심볼 확률 10% 증가");
            targetReel.SetProbability(Symbols.Seven, 10f);
        }
        // '8' 키를 누르면 Cherry 심볼의 확률을 10% 내립니다.
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            Debug.Log("8 키 눌림: Cherry 심볼 확률 10% 감소");
            targetReel.SetProbability(Symbols.Cherry, -10f);
        }
        // '9' 키를 누르면 Lemon 심볼의 확률을 10% 내립니다.
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            Debug.Log("9 키 눌림: Lemon 심볼 확률 10% 감소");
            targetReel.SetProbability(Symbols.Lemon, -10f);
        }
        // '0' 키를 누르면 Orange 심볼의 확률을 10% 내립니다.
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            Debug.Log("3 키 눌림: Orange 심볼 확률 10% 감소");
            targetReel.SetProbability(Symbols.Orange, -10f);
        }


    }

    private IEnumerator TestStopSpinCoroutine()
    {
        Debug.Log("Q 키 눌림: RelocateSymbols() 및 StopSpin() 테스트 시작");
        targetReel.RelocateSymbols();

        // Reel에 배치된 심볼을 확인하는 로직
        string symbolsInReel = "현재 릴 심볼: " + string.Join(", ", targetReel.row.Select(s => ((Symbols)s).ToString()));
        Debug.Log(symbolsInReel);

        // StopSpin()을 코루틴으로 호출하고 애니메이션이 끝날 때까지 기다립니다.
        yield return StartCoroutine(targetReel.StopSpin(targetReel.row));

        // 애니메이션이 끝난 후, GetResultSymbols()로 최종 결과를 가져옵니다.
        int[] resultSymbols = targetReel.GetResultSymbols();

        // 결과값을 디버그 로그에 출력합니다.
        string resultOutput = "선택된 릴 심볼: " + string.Join(", ", resultSymbols.Select(s => ((Symbols)s).ToString()));
        Debug.Log(resultOutput);
    }
}
