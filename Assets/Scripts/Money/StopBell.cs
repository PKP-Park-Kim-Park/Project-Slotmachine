using UnityEngine;

public class StopBell : MonoBehaviour, IInteractable
{
    // public string InteractionPrompt => "StopBell";

    private bool stop = false;

    public void Interact()
    {
        StopGame();
    }

    public void StopGame()
    {
        stop = true;

        // 게임 정지 상태를 확인합니다.
        if (stop)
        {
            Debug.LogWarning("게임 초기화/토큰 발행");

            if (GameManager.instance != null)
            {
                GameManager.instance.Init();
            }
            else
            {
                Debug.LogWarning("Money 인스턴스가 할당되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogWarning("게임 초기화 실패");
        }
    }
}
