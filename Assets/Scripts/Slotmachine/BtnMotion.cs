using UnityEngine;

[RequireComponent(typeof(Outline))]
public class BtnMotion : MonoBehaviour, IInteractable
{
    // public string InteractionPrompt => "버튼 누르기";
    public bool IsInteractable
    {
        get
        {
            if (slotMachine == null || slotMachine.IsActivating) return false;
            return GameManager.instance.levelData._level == slotMachine.MachineLevel;
        }
    }

    [Header("Outline Settings")]
    [SerializeField] private Outline outline;
    [SerializeField] private Color enabledColor = Color.yellow;
    [SerializeField] private Color disabledColor = Color.red;

    private Animator btnAnim;
    [SerializeField] private bool isIncreaseButton;

    [SerializeField] private SlotMachine slotMachine;

    void Awake()
    {
        btnAnim = GetComponent<Animator>();
        if (btnAnim == null)
        {
            Debug.LogError("BtnMotion 스크립트가 있는 오브젝트에 Animator 컴포넌트 추가 바람...");
        }

        outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    private void Start()
    {
        if (slotMachine == null)
        {
            slotMachine = GetComponentInParent<SlotMachine>();
        }

        if (slotMachine != null)
        {
            slotMachine.OnActivationStart += SetDisabledColor;
            slotMachine.OnActivationEnd += SetEnabledColor;
            GameManager.instance.levelManager.OnLevelUp += (level) => UpdateOutlineColor();
        }
        UpdateOutlineColor();
    }

    private void OnDisable()
    {
        if (slotMachine == null) return;
        slotMachine.OnActivationStart -= SetDisabledColor;
        slotMachine.OnActivationEnd -= SetEnabledColor;
        GameManager.instance.levelManager.OnLevelUp -= (level) => UpdateOutlineColor();
    }

    public void Interact()
    {
        if (!IsInteractable) return;

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

    private void SetEnabledColor()
    {
        if (outline != null)
        {
            outline.OutlineColor = enabledColor;
        }
    }

    private void SetDisabledColor()
    {
        if (outline != null)
        {
            outline.OutlineColor = disabledColor;
        }
    }

    private void UpdateOutlineColor()
    {
        if (IsInteractable) SetEnabledColor();
        else SetDisabledColor();
    }
}
