using UnityEngine;
using System.Collections;
using UnityEngine.Events; // UnityEvent를 사용하기 위해 추가

public class LeverMotion : MonoBehaviour, IInteractable
{
    [Tooltip("레버와 상호작용 시 색상을 변경할 Outline 컴포넌트")]
    public Outline outline;

    [Tooltip("레버를 당긴 후 다시 당길 수 있을 때까지의 대기 시간 (초)")]
    [SerializeField] private float cooldownSeconds = 3f;

    [Tooltip("레버를 당겼을 때 호출될 이벤트")]
    public UnityEvent OnLeverPulled;

    private Animator leverAnim;
    private bool isInteractable = true;
    private Color originalOutlineColor;
    private int pullTriggerHash;

    public string InteractionPrompt
    {
        get
        {
            if (isInteractable)
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

        if (outline != null)
        {
            originalOutlineColor = outline.OutlineColor;
        }

        // 애니메이션 트리거 문자열을 해시값으로 변환
        pullTriggerHash = Animator.StringToHash("Pull");
    }

    public void Interact()
    {
        // 상호작용이 불가능한 상태이면 아무것도 않음
        if (!isInteractable)
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
            StartCoroutine(LeverCooldown());
        }
        else
        {
            Debug.LogWarning("레버 애니메이션을 실행할 수 없음..");
        }

        // UnityEvent를 호출하여 연결된 모든 함수(예: Spin())를 실행
        OnLeverPulled?.Invoke();
    }

    private IEnumerator LeverCooldown()
    {
        // 상호작용 비활성화
        isInteractable = false;

        // 아웃라인 색상을 빨간색
        if (outline != null)
        {
            outline.OutlineColor = Color.red;
        }

        // 설정된 쿨타임만큼 대기
        yield return new WaitForSeconds(cooldownSeconds);

        // 아웃라인 색상을 원래대로
        if (outline != null)
        {
            outline.OutlineColor = originalOutlineColor;
        }
        isInteractable = true;
    }
}
