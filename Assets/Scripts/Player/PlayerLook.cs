using UnityEngine;

/// <summary>
/// 플레이어의 시점 및 카메라 회전을 관리합니다.
/// </summary>
public class PlayerLook : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float lookSensitivity = 3f;
    [SerializeField] private float cameraRotationLimit = 80f;
    [SerializeField] private Camera theCamera;
    [SerializeField] private float cameraTransitionSpeed = 5f;

    private float currentCameraRotationX = 0f;
    private Rigidbody myRigid;

    // 시점 고정 상태 관련 변수
    private bool isViewFixed = false;
    private bool isReturningToPlayer = false; // 카메라 복귀 상태
    private Transform cameraTarget;
    private Vector3 originalCameraLocalPosition;

    /// <summary>
    /// 현재 시점이 고정된 상태인지 여부를 반환합니다.
    /// </summary>
    public bool IsViewFixed => isViewFixed;

    /// <summary>
    /// 플레이어의 메인 카메라를 반환합니다.
    /// </summary>
    public Camera PlayerCamera => theCamera;

    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        originalCameraLocalPosition = theCamera.transform.localPosition;

        // 커서 초기 상태 설정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 시점이 고정된 상태에서 ESC 키를 누르면 시점 고정 해제
        if (isViewFixed && Input.GetKeyDown(KeyCode.Escape))
        {
            UnfixViewPoint();
        }
    }

    void LateUpdate()
    {
        if (isViewFixed)
        {
            // 시점 고정 모드
            // 카메라 위치는 목표 지점으로 부드럽게 이동
            theCamera.transform.position = Vector3.Lerp(theCamera.transform.position, cameraTarget.position, Time.deltaTime * cameraTransitionSpeed);
            // 카메라 회전도 목표 지점으로 부드럽게 이동
            theCamera.transform.rotation = Quaternion.Slerp(theCamera.transform.rotation, cameraTarget.rotation, Time.deltaTime * cameraTransitionSpeed);
        }
        else if (isReturningToPlayer)
        {
            // 카메라를 플레이어의 원래 시점으로 부드럽게 복귀
            Vector3 targetPosition = transform.TransformPoint(originalCameraLocalPosition);
            Quaternion targetRotation = transform.rotation * Quaternion.identity; // 원래 로컬 회전은 Quaternion.identity

            theCamera.transform.position = Vector3.Lerp(theCamera.transform.position, targetPosition, Time.deltaTime * cameraTransitionSpeed);
            theCamera.transform.rotation = Quaternion.Slerp(theCamera.transform.rotation, targetRotation, Time.deltaTime * cameraTransitionSpeed);

            // 전환이 거의 완료되었는지 확인 (거리가 매우 가까워지면)
            if (Vector3.Distance(theCamera.transform.position, targetPosition) < 0.01f)
            {
                isReturningToPlayer = false;
                // 오차를 없애기 위해 마지막에 위치를 정확히 맞춰줌
                theCamera.transform.localPosition = originalCameraLocalPosition;
                theCamera.transform.localRotation = Quaternion.identity;
                // 카메라 상하 회전 값 초기화
                currentCameraRotationX = 0f;
            }
        }
        else
        {
            // 일반 플레이 모드일 때 캐릭터 및 카메라 회전 처리
            CharacterRotation();
            CameraRotation();
        }
    }

    private void CharacterRotation()
    {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotationY));
    }

    private void CameraRotation()
    {
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;

        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    public void FixViewPoint(Transform target)
    {
        isViewFixed = true;
        isReturningToPlayer = false; // 복귀 중이었다면 중단
        cameraTarget = target;

        // UI 조작을 위해 커서 잠금 해제
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void UnfixViewPoint()
    {
        isViewFixed = false;
        isReturningToPlayer = true; // 복귀 전환 시작
        cameraTarget = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}