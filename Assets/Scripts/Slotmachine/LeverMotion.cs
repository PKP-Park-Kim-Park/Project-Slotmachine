using UnityEngine;
using System.Collections;

public class LeverMotion : MonoBehaviour, IInteractable
{
    [Tooltip("레버와 상호작용 시 색상을 변경할 Outline 컴포넌트")]
    public Outline outline;

    private Animator leverAnim;
    private bool isInteractable = true;
    private Color originalOutlineColor;

    public string InteractionPrompt
    {
        get
        {
            if (isInteractable)
                return "레버 당기기";
            else
                return ""; // 상호작용 불가 상태일 때는 프롬프트를 표시하지 않음
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
            leverAnim.SetTrigger("Pull");
            StartCoroutine(LeverCooldown());
        }
        else
        {
            Debug.LogWarning("레버 애니메이션을 실행할 수 없음..");
        }

        // TODO
        // 레버 당길 시 Spin()
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

        // 3초
        yield return new WaitForSeconds(3f);

        // 아웃라인 색상을 원래대로
        if (outline != null)
        {
            outline.OutlineColor = originalOutlineColor;
        }
        isInteractable = true;
    }
}
