using UnityEngine;

public class Crystal : MonoBehaviour
{
    // Public ������ ����� �ν����� â���� ȸ�� �ӵ��� ������ �� �ְ� �մϴ�.
    public float rotationSpeed = 50f;

    // �� �����Ӹ��� ȣ��Ǵ� �Լ�
    void Update()
    {
        // ������Ʈ�� x���� �������� rotationSpeed ��ŭ ȸ����ŵ�ϴ�.
        // Time.deltaTime�� ���Ͽ� ������ �ӵ��� ������� ������ �ӵ��� ȸ���ϰ� �մϴ�.
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
