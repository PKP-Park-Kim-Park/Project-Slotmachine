using UnityEngine;

public class Crystal : MonoBehaviour
{
    // Public 변수로 만들어 인스펙터 창에서 회전 속도를 조절할 수 있게 합니다.
    public float rotationSpeed = 50f;

    // 매 프레임마다 호출되는 함수
    void Update()
    {
        // 오브젝트를 x축을 기준으로 rotationSpeed 만큼 회전시킵니다.
        // Time.deltaTime을 곱하여 프레임 속도에 관계없이 일정한 속도로 회전하게 합니다.
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
