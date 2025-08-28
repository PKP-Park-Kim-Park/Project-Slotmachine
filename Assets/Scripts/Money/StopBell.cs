using UnityEngine;

public class StopBell : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "StopBell";

    private Money money;

    private bool stop = false;

    void Start()
    {
        money = new Money();
    }

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
            Debug.LogWarning("게임이 정지 상태입니다.");

            if (money != null)
            {
                Debug.LogWarning("ConvertToken 실행.");
                money.ConvertToken();
            }
            else
            {
                Debug.LogWarning("Money 인스턴스가 할당되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogWarning("게임이 정지 상태가 아닙니다.");
        }
    }
}
