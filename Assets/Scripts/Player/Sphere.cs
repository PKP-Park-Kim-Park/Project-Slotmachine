using UnityEngine;

public class Sphere : MonoBehaviour, IInteractable
{
    // IInteractable �������̽��� ��ȣ�ۿ� ������Ʈ ����.
    // �÷��̾ ������Ʈ�� �ٶ� �� UI�� ǥ�õ� �ؽ�Ʈ�Դϴ�.
    public string InteractionPrompt => "Sphere �׽�Ʈ��";
    // Sphere�� Material�� ������ ����
    private Material sphereMaterial;

    void Start()
    {
        // ���� �� �ڽ��� MeshRenderer���� Material ������Ʈ ��������
        sphereMaterial = GetComponent<MeshRenderer>().material;
    }

    // IInteractable �������̽��� Interact() �޼ҵ� ����.
    // �÷��̾ ��ȣ�ۿ� ��ư(���콺 ���� Ŭ��)�� ������ �� �Լ��� ȣ��˴ϴ�.
    public void Interact()
    {
        // Interact()�� ȣ��Ǹ� ������ �����ϰ� ����
        sphereMaterial.color = new Color(Random.value, Random.value, Random.value);
        Debug.Log("Sphere�� ������ ����Ǿ����ϴ�!");
    }
}
