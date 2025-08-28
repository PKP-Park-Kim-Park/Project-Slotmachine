using UnityEngine;

/// <remarks>
/// 필요한 Animator 설정:
/// - States: Closed, OpenForward
/// - Triggers: OpenForward, Close
///
/// Transitions:
/// - Any State -> OpenForward (Condition: OpenForward trigger)
/// - OpenForward -> Closed (Condition: Close trigger)
/// </remarks>
public class Door : MonoBehaviour, IInteractable
{
    [Tooltip("문이 잠겼는지 여부를 설정합니다.")]
    public bool locked = false;

    private Animator animator;
    private bool isOpen = false;
    private int openForwardTriggerHash;
    private int closeTriggerHash;

    public string InteractionPrompt => "문 움직임";

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Door 스크립트가 부착된 오브젝트에 Animator 컴포넌트가 없습니다.", this);
        }

        openForwardTriggerHash = Animator.StringToHash("OpenForward");
        closeTriggerHash = Animator.StringToHash("Close");
    }

    public void Interact()
    {
        if (animator != null && animator.IsInTransition(0))
        {
            return;
        }

        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        if (locked || isOpen)
        {
            return;
        }

        if (animator == null)
        {
            return;
        }

        animator.SetTrigger(openForwardTriggerHash);

        isOpen = true;
    }

    public void Close()
    {
        if (!isOpen)
        {
            return;
        }

        if (animator == null)
        {
            return;
        }

        animator.SetTrigger(closeTriggerHash);
        isOpen = false;
    }
}
