using UnityEngine;
using System.Collections;

public class BtnMotion : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "버튼 누르기";
    private Animator btnAnim;

    void Awake()
    {
        // 스크립트가 시작될 때 Animator 컴포넌트를 자동으로 찾아옵니다.
        // 이 스크립트와 Animator가 항상 같은 게임 오브젝트에 있다는 가정 하에 작동합니다.
        btnAnim = GetComponent<Animator>();
        if (btnAnim == null)
        {
            Debug.LogError("BtnMotion 스크립트가 있는 오브젝트에 Animator 컴포넌트 추가 바람...");
        }
    }

    public void Interact()
    {
        PressBtn();
    }

    // 레버를 당겼을 때 호출될 함수입니다.
    public void PressBtn()
    {
        // leverAnim이 할당되었는지 확인 후 애니메이션 실행
        if (btnAnim != null)
        {
            Debug.Log("버튼 누름...");
            btnAnim.SetTrigger("Press");
        }
        else
        {
            Debug.LogWarning("버튼 애니메이션을 실행할 수 없음..");
        }

        // TODO
        // 버튼 누를 시 Increase(), Decrease()
    }
}
