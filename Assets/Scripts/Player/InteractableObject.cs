using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public void Interact()
    {
        // �� �Լ��� ȣ��Ǹ� ��ȣ�ۿ� ������ �����մϴ�.
        // ��: ��ü�� ���� �ٲٱ�
        GetComponent<Renderer>().material.color = Color.red;
        Debug.Log("��ü�� ��ȣ�ۿ��߽��ϴ�!");
    }
}