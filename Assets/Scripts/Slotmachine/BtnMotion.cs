using UnityEngine;

public class BtnMotion : MonoBehaviour, IInteractable
{
    // public string InteractionPrompt => "버튼 누르기";
    private Animator btnAnim;
    [SerializeField] private bool isIncreaseButton;

    private SlotMachine slotMachine;

    void Awake()
    {
        btnAnim = GetComponent<Animator>();
        if (btnAnim == null)
        {
            Debug.LogError("BtnMotion 스크립트가 있는 오브젝트에 Animator 컴포넌트 추가 바람...");
        }
    }

    private void Start()
    {
        slotMachine = GetComponentInParent<SlotMachine>();
    }

    public void Interact()
    {
        PressBtn();
    }

    public void PressBtn()
    {
        // leverAnim이 할당되었는지 확인 후 애니메이션 실행
        if (btnAnim != null)
        {
            // Debug.Log("버튼 누름...");
            btnAnim.SetTrigger("Press");
        }
        else
        {
            Debug.LogWarning("버튼 애니메이션을 실행할 수 없음..");
        }

        // TODO
        // 버튼 누를 시 Increase(), Decrease()
        slotMachine.Bet(isIncreaseButton);
    }
}
