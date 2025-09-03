using UnityEngine;
using System.Collections;
using UnityEngine.Events; // UnityEvent를 사용하기 위해 추가

public class LeverMotion : MonoBehaviour, IInteractable
{
    [Tooltip("레버와 상호작용 시 색상을 변경할 Outline 컴포넌트")]
    public Outline outline;

    [Tooltip("레버를 당겼을 때 호출될 이벤트")]
    public UnityEvent OnLeverPulled;

    [Tooltip("레버의 상태를 제어할 SlotMachine 컴포넌트")]
    [SerializeField] private SlotMachine slotMachine;

    private Animator leverAnim;
    private Color originalOutlineColor;
    private int pullTriggerHash;

    // isInteractable 프로퍼티를 SlotMachine의 상태와 직접 연동합니다.
    public bool IsInteractable { get { return slotMachine != null && !slotMachine.IsActivating; } }
    public string InteractionPrompt
    {
        get
        {
            if (IsInteractable)
                return "레버 당기기";
            else
                return "당기기 실패"; // 상호작용 불가 상태
        }
    }

    void Awake()
    {
        leverAnim = GetComponent<Animator>();
        if (leverAnim == null)
        {
            Debug.LogError("LeverMotion 스크립트가 있는 오브젝트에 Animator 컴포넌트 추가 바람...");
        }
        
        if (slotMachine == null)
        {
            Debug.LogError("LeverMotion 스크립트에 SlotMachine 컴포넌트가 할당되지 않았습니다. Inspector에서 할당해주세요.");
        }

        // SlotMachine의 상태 변경 이벤트에 구독합니다.
        if (slotMachine != null)
        {
            slotMachine.OnActivationStart += HandleActivationStart;
            slotMachine.OnActivationEnd += HandleActivationEnd;
        }

        if (outline != null)
        {
            originalOutlineColor = outline.OutlineColor;
        }

        // 애니메이션 트리거 문자열을 해시값으로 변환
        pullTriggerHash = Animator.StringToHash("Pull");
    }

    private void OnDestroy()
    {
        // 컴포넌트가 파괴될 때 이벤트 구독을 해제하여 메모리 누수를 방지합니다.
        if (slotMachine != null)
        {
            slotMachine.OnActivationStart -= HandleActivationStart;
            slotMachine.OnActivationEnd -= HandleActivationEnd;
        }
    }
    public void Interact()
    {
        // 상호작용이 불가능한 상태이면 아무것도 않음
        if (!IsInteractable)
        {
            return;
        }

        PullLever();
    }

    public void PullLever()
    {
        // leverAnim이 할당되었는지 확인 후 애니메이션 실행
        if (leverAnim != null)
        {
            Debug.Log("레버 당김...");
            leverAnim.SetTrigger(pullTriggerHash);
        }
        else
        {
            Debug.LogWarning("레버 애니메이션을 실행할 수 없음..");
        }

        // UnityEvent를 호출하여 연결된 모든 함수를 실행 => 현재는 Spin()만 연결
        OnLeverPulled?.Invoke();
    }

    // 슬롯 활성
    private void HandleActivationStart()
    {
        if (outline != null)
        {
            outline.OutlineColor = Color.red;
        }
    }

    // 슬롯 비활성
    private void HandleActivationEnd()
    {
        if (outline != null)
        {
            outline.OutlineColor = originalOutlineColor;
        }
    }
}
