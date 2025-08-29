using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    // Inspector 필드
    [Header("구성 요소")]
    [Tooltip("상호작용 시 색상을 변경할 Outline 컴포넌트")]
    public Outline outline;

    [Tooltip("이 오브젝트와 상호작용 시 문이 자동 잠김")]
    public AutoLockTrigger lockTrigger;

    [Header("상태")]
    [Tooltip("문이 잠겼는지 여부")]
    [SerializeField]
    private bool doorLock = false;
    
    /// <summary>
    /// 문 잠금 여부, 아웃라인 색상 변경
    /// </summary>
    public bool locked
    {
        get => doorLock;
        set
        {
            // 상태가 변경되지 않았으면 아무것도 하지 않음
            if (doorLock == value) return; 
            doorLock = value;
            UpdateDoorVisuals();
        }
    }

    /// <summary>
    /// 상호작용 텍스트
    /// </summary>
    public string InteractionPrompt
    {
        get
        {
            if (locked) return "잠겨있음";
            return isOpen ? "문 닫기" : "문 열기";
        }
    }

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
        InitializeOutline();
    }

    private void OnEnable()
    {
        if (lockTrigger != null)
        {
            lockTrigger.OnInteracted.AddListener(AutoCloseAndLock);
        }
    }

    private void OnDisable()
    {
        if (lockTrigger != null)
        {
            lockTrigger.OnInteracted.RemoveListener(AutoCloseAndLock);
        }
    }

    /// <summary>
    /// 오브젝트와 상호작용 시 호출
    /// </summary>
    public void Interact()
    {
        // 문이 잠겨있거나 애니메이션 전환 중일 때는 상호작용 불가
        if (locked) return;
        if (animator != null && animator.IsInTransition(0)) return;

        ToggleDoor();
    }

    // 문 제어
    // 열기
    public void Open()
    {
        if (locked || isOpen || animator == null) return;

        animator.SetTrigger(openForwardTriggerHash);
        isOpen = true;
    }

    // 닫기
    public void Close()
    {
        if (!isOpen || animator == null) return;

        animator.SetTrigger(closeTriggerHash);
        isOpen = false;
    }

    /// <summary>
    /// 문 잠금
    /// </summary>
    public void Lock()
    {
        if (locked) return;

        locked = true;
        Debug.Log("문이 잠겼습니다.");
    }

    /// <summary>
    /// 문 잠금 해제
    /// </summary>
    public void Unlock()
    {
        if (!locked) return;

        locked = false;
        Debug.Log("문이 잠금 해제되었습니다.");
    }

    /// <summary>
    /// 문 자동 잠금. lockTrigger에 의해 호출
    /// </summary>
    public void AutoCloseAndLock()
    {
        if (isOpen)
        {
            Close();
        }

        locked = true;
        Debug.Log("문이 자동으로 닫히고 잠겼습니다.");
    }

    // 헬퍼 함수
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
