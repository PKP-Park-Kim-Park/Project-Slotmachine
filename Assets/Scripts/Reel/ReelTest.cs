using UnityEngine;
using System.Collections;

public class ReelTest : MonoBehaviour
{
    // 테스트할 Reel 컴포넌트를 할당할 변수
    [SerializeField] private Reel targetReel;

    void Update()
    {
        // 'S' 키를 눌렀을 때만 RelocateSymbols() 함수를 호출합니다.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q 키 눌림: RelocateSymbols() 테스트 시작");
            targetReel.RelocateSymbols();

            // Reel에 배치된 심볼을 확인하는 로직
            string symbolsInReel = "현재 릴 심볼: ";
            for (int i = 0; i < targetReel.row.Length; i++)
            {
                // Symbols 열거형을 사용해 숫자 값을 심볼 이름으로 변환하여 출력
                symbolsInReel += (Symbols)targetReel.row[i] + (i < targetReel.row.Length - 1 ? ", " : "");
            }
            Debug.Log(symbolsInReel);
        }

        // '3'번 키를 누르면 StopSpin() 함수를 호출하고 결과를 받습니다.
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3번 키 눌림: StopSpin() 테스트 시작");

            // StopSpin()을 호출할 때 Reel에 있는 현재 값을 그대로 전달합니다.
            int[] resultSymbols = targetReel.StopSpin(targetReel.row);

            // 결과값을 디버그 로그에 출력합니다.
            string resultOutput = "선택된 릴 심볼: ";
            for (int i = 0; i < resultSymbols.Length; i++)
            {
                resultOutput += (Symbols)resultSymbols[i] + (i < resultSymbols.Length - 1 ? ", " : "");
            }
            Debug.Log(resultOutput);
        }

    }
}
