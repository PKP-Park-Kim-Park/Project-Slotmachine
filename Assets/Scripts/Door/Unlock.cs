using UnityEngine;

// 이 스크립트는 IInteractable 인터페이스를 사용합니다.
// 프로젝트에 있는 Interaction 관련 스크립트가 필요합니다.
public class Unlock : MonoBehaviour, IInteractable
{
    [Tooltip("이 열쇠로 열 수 있는 문")]
    public Door targetDoor;

    // public string InteractionPrompt => "열쇠 줍기";

    public void Interact()
    {
        if (targetDoor != null)
        {
            // 문 잠금 해제
            targetDoor.Unlock();
            Debug.Log($"'{targetDoor.name}' 문의 잠금을 해제했습니다.");
        }
        else
        {
            Debug.LogWarning("열쇠에 연결된 문이 없습니다.", this);
        }

        // 상호작용 후 열쇠 오브젝트는 비활성화
        gameObject.SetActive(false);
    }
}
