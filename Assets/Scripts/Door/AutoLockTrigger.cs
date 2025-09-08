using UnityEngine;
public class AutoLockTrigger : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (GameManager.instance != null)
        {
            Debug.Log("AutoLockTrigger 발동! 모든 문을 잠급니다.");
            GameManager.instance.TriggerLockAllDoors();
        }
    }
}
