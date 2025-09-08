using UnityEngine;

public class Unlock : MonoBehaviour, IInteractable
{
    [Tooltip("이 열쇠로 열 수 있는 문")]
    public Door targetDoor;

    // public string InteractionPrompt => "열쇠 줍기";

    public void Interact()
    {
        if (targetDoor != null && GameManager.instance != null)
        {
            // 소지금의 10% 소모
            int cost = (int)(GameManager.instance.money._gold * 0.1f);
            GameManager.instance.money.SpendGold(cost);

            // 문 잠금 해제
            targetDoor.Unlock();
            Debug.Log($"'{targetDoor.name}' 문의 잠금을 해제했습니다. (소모된 골드: {cost})");
        }

        // 상호작용 후 열쇠 오브젝트는 비활성화
        gameObject.SetActive(false);
    }
}
