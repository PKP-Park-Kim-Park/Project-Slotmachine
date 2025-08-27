using UnityEngine;
using System.Collections;

public class LeverMotion : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "레버 당기기";
    private Animator leverAnim;

    void Awake()
    {
        // 스크립트가 시작될 때 Animator 컴포넌트를 자동으로 찾아옵니다.
        // 이 스크립트와 Animator가 항상 같은 게임 오브젝트에 있다는 가정 하에 작동합니다.
        leverAnim = GetComponent<Animator>();
        if (leverAnim == null)
        {
            Debug.LogError("LeverMotion 스크립트가 있는 오브젝트에 Animator 컴포넌트 추가 바람...");
        }
    }

    public void Interact()
    {
        PullLever();
    }

    // 레버를 당겼을 때 호출될 함수입니다.
    public void PullLever()
    {
        // leverAnim이 할당되었는지 확인 후 애니메이션 실행
        if (leverAnim != null)
        {
            Debug.Log("레버 당김...");
            leverAnim.SetTrigger("Pull");
        }
        else
        {
            Debug.LogWarning("레버 애니메이션을 실행할 수 없음..");
        }

        // TODO
        // 레버 당길 시 Spin()
    }
}
