using UnityEngine;

public class Sphere : MonoBehaviour, IInteractable
{
    // IInteractable 인터페이스의 상호작용 프롬프트 구현.
    // 플레이어가 오브젝트를 바라볼 때 UI에 표시될 텍스트입니다.
    public string InteractionPrompt => "Sphere 테스트용";
    // Sphere의 Material을 저장할 변수
    private Material sphereMaterial;

    void Start()
    {
        // 시작 시 자신의 MeshRenderer에서 Material 컴포넌트 가져오기
        sphereMaterial = GetComponent<MeshRenderer>().material;
    }

    // IInteractable 인터페이스의 Interact() 메소드 구현.
    // 플레이어가 상호작용 버튼(마우스 왼쪽 클릭)을 누르면 이 함수가 호출됩니다.
    public void Interact()
    {
        // Interact()가 호출되면 색상을 랜덤하게 변경
        sphereMaterial.color = new Color(Random.value, Random.value, Random.value);
        Debug.Log("Sphere의 색상이 변경되었습니다!");
    }
}
