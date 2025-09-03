using UnityEngine;
using UnityEngine.Events;
public class AutoLockTrigger : MonoBehaviour, IInteractable
{
    [Tooltip("상호작용 시 발생할 이벤트입니다.")]
    public UnityEvent OnInteracted;

    [Tooltip("한 번만")]
    public bool interactOnce = true;

    public void Interact()
    {
        OnInteracted?.Invoke();

        if (interactOnce)
        {
            this.enabled = false;
        }
    }
}
