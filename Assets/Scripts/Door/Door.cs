using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("구성 요소")]
    [Tooltip("상호작용 시 색상을 변경할 Outline 컴포넌트")]
    public Outline outline;

    [Header("상태")]
    [Tooltip("문이 잠겼는지 여부")]
    [SerializeField]
    private bool doorLock = false;

    /*
    public string InteractionPrompt
    {
        get
        {
            if (doorLock) return "잠겨있음";
            return isOpen ? "문 닫기" : "문 열기";
        }
    }
    */
    
    // 내부 상태 변수
    private bool isOpen = false;
    private Color originalOutlineColor;

    // 컴포넌트 및 해시
    private Animator animator;
    private int openForwardTriggerHash;
    private int closeTriggerHash;

    // Unity 생명주기 함수
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Door 스크립트가 부착된 오브젝트에 Animator 컴포넌트가 없습니다.", this);
        }

        InitializeAnimatorHashes();

        if (GameManager.instance != null)
        {
            GameManager.instance.OnUnlockDoor += Unlock;
            GameManager.instance.OnLockAllDoors += Lock;
        }

        InitializeOutline();
    }

    private void OnEnable()
    {
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnUnlockDoor -= Unlock;
            GameManager.instance.OnLockAllDoors -= Lock;
        }
    }

    /// <summary>
    /// 오브젝트와 상호작용 시 호출
    /// </summary>
    public void Interact()
    {
        // 문이 잠겨있거나 애니메이션 전환 중일 때는 상호작용 불가
        if (doorLock) return;
        if (animator != null && animator.IsInTransition(0)) return;

        ToggleDoor();
    }

    // --- 문 상태 제어 메서드 ---

    /// <summary>
    /// 문이 잠겨있는지 확인
    /// </summary>
    public bool IsLocked()
    {
        return doorLock;
    }

    /// <summary>
    /// 문 잠금
    /// </summary>
    public void Lock()
    {
        if (doorLock) return;

        // 문이 열려있으면 먼저 닫습니다.
        if (isOpen)
        {
            Close();
        }

        SetLockedState(true);
        Debug.Log("문이 잠겼습니다.");
    }

    /// <summary>
    /// 문 잠금 해제
    /// </summary>
    public void Unlock()
    {
        if (!doorLock) return;

        SetLockedState(false);
        Debug.Log("문이 잠금 해제되었습니다.");
    }

    /// <summary>
    /// 문 열기
    /// </summary>
    public void Open()
    {
        if (doorLock || isOpen || animator == null) return;

        animator.SetTrigger(openForwardTriggerHash);
        isOpen = true;
    }

    /// <summary>
    /// 문 닫기
    /// </summary>
    public void Close()
    {
        if (!isOpen || animator == null) return;

        animator.SetTrigger(closeTriggerHash);
        isOpen = false;
    }

    /// <summary>
    /// 문의 잠금 상태를 변경 및 아웃라인 변경
    /// </summary>
    private void SetLockedState(bool newLockedState)
    {
        if (doorLock == newLockedState) return;

        doorLock = newLockedState;
        UpdateDoorVisuals();
    }

    /// <summary>
    /// 성능 최적화(해시값을 미리 처리)
    /// </summary>
    private void InitializeAnimatorHashes()
    {
        openForwardTriggerHash = Animator.StringToHash("OpenForward");
        closeTriggerHash = Animator.StringToHash("Close");
    }

    /// <summary>
    /// 아웃라인 컴포넌트 초기화
    /// </summary>
    private void InitializeOutline()
    {
        if (outline != null)
        {
            originalOutlineColor = outline.OutlineColor;
            UpdateDoorVisuals(); // 초기 잠금 상태에 따라 아웃라인 색상 설정
        }
    }

    /// <summary>
    /// 문의 열림/닫힘 상태 전환
    /// </summary>
    private void ToggleDoor()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    /// <summary>
    /// 문의 상태에 따라 아웃라인 색상 변경
    /// </summary>
    private void UpdateDoorVisuals()
    {
        if (outline == null) return;

        outline.OutlineColor = doorLock ? Color.red : originalOutlineColor;
    }
}
