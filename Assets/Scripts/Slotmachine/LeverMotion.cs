using UnityEngine;
using System.Collections;
using UnityEngine.Events;


public class LeverMotion : MonoBehaviour, IInteractable
{
    [Tooltip("레버를 당겼을 때 호출될 이벤트")]
    public UnityEvent OnLeverPulled;

    [Header("Outline Settings")]
    [Tooltip("상호작용 가능/불가능 상태를 표시할 Outline 컴포넌트")]
    [SerializeField] private Outline outline;
    [SerializeField] private Color enabledColor = Color.yellow;
    [SerializeField] private Color disabledColor = Color.red;

    [Tooltip("레버의 상태를 제어할 SlotMachine 컴포넌트")]
    [SerializeField] private SlotMachine slotMachine;

    [Header("Effects")]
    [Tooltip("레버를 당길 때 재생될 스파크 스프라이트 애니메이터")]
    [SerializeField] private Animator sparkAnimator;

    private Animator leverAnim;
    private Coroutine sparkCoroutine;
    private int pullTriggerHash;

    public bool IsInteractable
    {
        get
        {
            if (slotMachine == null || slotMachine.IsActivating) return false;

            // 플레이어 레벨과 슬롯머신 레벨이 같은지 확인
            return GameManager.instance.levelData._level == slotMachine.MachineLevel;
        }
    }

    /*
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
    */

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

        // 애니메이션 트리거 문자열을 해시값으로 변환
        pullTriggerHash = Animator.StringToHash("Pull");
    }

    private void OnEnable()
    {
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
        if (slotMachine != null)
        {
            slotMachine.OnActivationStart -= SetDisabledColor;
            slotMachine.OnActivationEnd -= SetEnabledColor;
            GameManager.instance.levelManager.OnLevelUp -= (level) => UpdateOutlineColor();
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
        OnLeverPulled?.Invoke();
    }

    /// <summary>
    /// 아웃라인 색전환
    /// </summary>
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

    /// <summary>
    /// 레버 당길때 스파크
    /// </summary>
    public void PlaySparkEffect()
    {
        if (sparkAnimator == null) return;

        if (sparkCoroutine != null)
        {
            StopCoroutine(sparkCoroutine);
        }
        sparkCoroutine = StartCoroutine(SparkEffectSequence());
    }

    private IEnumerator SparkEffectSequence()
    {
        // Debug.Log("스파크 효과 재생!");

        sparkAnimator.gameObject.SetActive(true);
        sparkAnimator.Play(0, -1, 0f);

        AnimatorClipInfo[] clipInfo = sparkAnimator.GetCurrentAnimatorClipInfo(0);
        float animationLength = clipInfo.Length > 0 ? clipInfo[0].clip.length : 1.0f;

        yield return new WaitForSeconds(animationLength);

        sparkAnimator.gameObject.SetActive(false);
        sparkCoroutine = null;
    }
}
