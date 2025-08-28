using UnityEngine;

/// <summary>
/// 상호작용 시 카메라 시점을 고정해야 하는 오브젝트에 추가하는 컴포넌트입니다.
/// </summary>
public class ViewFixObject : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;

    public Transform GetCameraTarget()
    {
        return cameraTarget;
    }
}
