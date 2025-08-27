using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public void Interact()
    {
        // 이 함수가 호출되면 상호작용 로직을 실행합니다.
        // 예: 물체의 색깔 바꾸기
        GetComponent<Renderer>().material.color = Color.red;
        Debug.Log("물체와 상호작용했습니다!");
    }
}