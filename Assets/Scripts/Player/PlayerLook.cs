using UnityEngine;

/// <summary>
/// 플레이어 시점 및 카메라 회전 관리
/// </summary>
public class PlayerLook : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float lookSensitivity = 3f;
    [SerializeField] private float cameraRotationLimit = 80f;
    [SerializeField] private Camera theCamera;

    [Header("Fixed View Settings")]
    [SerializeField] private float cameraTransitionSpeed = 5f;
    [SerializeField] private Vector2 edgePanDeadZone = new Vector2(0.7f, 0.7f); // 화면 중앙의 데드존 크기 (0~1)
    [SerializeField] private float edgePanMaxAngleX = 5f; // 상하 시야 이동 최대 각도
    [SerializeField] private float edgePanMaxAngleY = 10f; // 좌우 시야 이동 최대 각도

    private float currentCameraRotationX = 0f;
    private Rigidbody myRigid;

    // 시점 고정 상태 관련 변수
    private bool isViewFixed = false;
    private bool isReturningToPlayer = false; // 카메라 복귀 상태
    private Transform cameraTarget;
    private Vector3 originalCameraLocalPosition;

    /// <summary>
    /// 현재 시점이 고정된 상태인지 여부
    /// </summary>
    public bool IsViewFixed => isViewFixed;

    /// <summary>
    /// 플레이어의 메인 카메라
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
            theCamera.transform.position = Vector3.Lerp(theCamera.transform.position, cameraTarget.position, Time.deltaTime * cameraTransitionSpeed);

            float mouseOffsetX = (Input.mousePosition.x / Screen.width) - 0.5f;
            float mouseOffsetY = (Input.mousePosition.y / Screen.height) - 0.5f;

            float targetAngleY = 0f;
            float targetAngleX = 0f;

            float deadZoneHalfWidth = edgePanDeadZone.x / 2f;
            float deadZoneHalfHeight = edgePanDeadZone.y / 2f;

            if (Mathf.Abs(mouseOffsetX) > deadZoneHalfWidth)
            {
                float panRangeX = 0.5f - deadZoneHalfWidth;
                float panInputX = Mathf.Sign(mouseOffsetX) * (Mathf.Abs(mouseOffsetX) - deadZoneHalfWidth);
                float normalizedPanX = Mathf.Clamp(panInputX / panRangeX, -1f, 1f);
                targetAngleY = normalizedPanX * edgePanMaxAngleY;
            }

            if (Mathf.Abs(mouseOffsetY) > deadZoneHalfHeight)
            {
                float panRangeY = 0.5f - deadZoneHalfHeight;
                float panInputY = Mathf.Sign(mouseOffsetY) * (Mathf.Abs(mouseOffsetY) - deadZoneHalfHeight);
                float normalizedPanY = Mathf.Clamp(panInputY / panRangeY, -1f, 1f);
                targetAngleX = normalizedPanY * edgePanMaxAngleX * -1f; // Y축은 반전
            }

            Quaternion panOffset = Quaternion.Euler(targetAngleX, targetAngleY, 0f);
            Quaternion finalTargetRotation = cameraTarget.rotation * panOffset;

            theCamera.transform.rotation = Quaternion.Slerp(theCamera.transform.rotation, finalTargetRotation, Time.deltaTime * cameraTransitionSpeed);
        }
        else if (isReturningToPlayer)
        {
            Vector3 targetPosition = transform.TransformPoint(originalCameraLocalPosition);
            Quaternion targetRotation = transform.rotation * Quaternion.identity; // 원래 로컬 회전은 Quaternion.identity

            theCamera.transform.position = Vector3.Lerp(theCamera.transform.position, targetPosition, Time.deltaTime * cameraTransitionSpeed);
            theCamera.transform.rotation = Quaternion.Slerp(theCamera.transform.rotation, targetRotation, Time.deltaTime * cameraTransitionSpeed);

            if (Vector3.Distance(theCamera.transform.position, targetPosition) < 0.01f)
            {
                isReturningToPlayer = false;
                theCamera.transform.localPosition = originalCameraLocalPosition;
                theCamera.transform.localRotation = Quaternion.identity;
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