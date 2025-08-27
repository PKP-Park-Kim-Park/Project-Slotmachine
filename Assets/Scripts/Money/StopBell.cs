using UnityEngine;

public class StopBell : MonoBehaviour
{
    private bool stop;

    public Money money;
    public void StopGame()
    {
        stop = true;

        // 게임 정지 bool로 체크후 컨버트 토큰 하기
        if (stop && money != null)
        {
            Debug.LogWarning("ConvertToken 실행.");
            money.ConvertToken();
        }
        else if(money == null)
        {
            //게임 매니저에서 초기화해서 받아야댐
            Debug.LogWarning("Money 컴포넌트 할당 X.");
            
        }
        else if(!stop)
        {
            Debug.LogWarning("게임 정지 상태 X.");
        }
    }

}
