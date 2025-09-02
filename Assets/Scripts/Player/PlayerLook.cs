using UnityEngine;

/// <summary>
/// 플레이어 시점 및 카메라 회전 관리
/// </summary>
public class PlayerLook : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float lookSensitivity = 3f;
    [SerializeField] private float cameraRotationLimit = 80f;
    [SerializeField] private Camera mainCam;

    [Header("Fixed View Settings")]
    [SerializeField] private float cameraEnterSpeed = 5f; // 고정 시점으로 전환하는 속도
    [SerializeField] private float cameraReturnSpeed = 5f; // 플레이어 시점으로 복귀하는 속도
    [SerializeField] private Vector2 edgePanDeadZone = new Vector2(0.7f, 0.7f); // 화면 중앙의 데드존 크기 (0~1)
    [SerializeField] private float edgePanMaxAngleX = 5f; // 상하 시야 이동 최대 각도
    [SerializeField] private float edgePanMaxAngleY = 10f; // 좌우 시야 이동 최대 각도

    private float currentCameraRotationX = 0f;
    private Rigidbody rb;

    // 시점 고정 상태 관련 변수
    private bool isViewFixed = false;
    private bool isReturningToPlayer = false; // 카메라 복귀 상태
    private Transform cameraTarget;
    private Vector3 originalCamPosition; // 카메라 원래 위치

    /// <summary>
    /// 현재 시점이 고정된 상태인지 여부
    /// </summary>
    public bool IsViewFixed => isViewFixed;

    /// <summary>
    /// 플레이어의 메인 카메라
    /// </summary>
    public Camera PlayerCamera => mainCam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalCamPosition = mainCam.transform.localPosition;

        // 커서 초기 상태 설정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 시점이 고정된 상태에서 ESC 키를 누르면 시점 고정 해제
        if (isViewFixed && Input.GetKeyDown(KeyCode.Q))
        {
            UnfixViewPoint();
        }
    }

    void LateUpdate()
    {
        // 현재 상태에 따라 적절한 카메라 처리 함수를 호출
        if (isViewFixed)
        {
            // 고정 모드로 진입
            HandleFixedView();
        }
        else if (isReturningToPlayer)
        {
            // 고정 모드 해제하고 플레이어에게 캠 복귀
            ReturnNomalView();
        }
        else
        {
            // 일반 시점 모드
            HandleNormalView();
        }
    }

    private void CharacterRotation()
    {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(characterRotationY));
    }

    private void CameraRotation()
    {
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;

        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        mainCam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    /// <summary>
    /// 일반 플레이 모드 시 시점 처리
    /// </summary>
    private void HandleNormalView()
    {
        CharacterRotation();
        CameraRotation();
    }

    /// <summary>
    /// 고정 모드일 때 카메라 움직임 처리
    /// </summary>
    private void HandleFixedView()
    {
        // 부드럽게 이동
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, cameraTarget.position, Time.deltaTime * cameraEnterSpeed);

        // 마우스 위치 따라서 추가적인 회전 담당
        Quaternion finalTargetRotation = CalculateEdgePanRotation();

        // 부드럽게 회전
        mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, finalTargetRotation, Time.deltaTime * cameraEnterSpeed);
    }

    /// <summary>
    /// HandleFixedView에서 사용(읽지마)
    /// 고정 모드일 때 마우스 위치에 따라 카메라를 추가로 회전시키는 것을 계산
    /// </summary>
    /// <returns> 최종 쿼터니언 값 반환</returns>
    private Quaternion CalculateEdgePanRotation()
    {
        // 마우스 위치를 중앙 기준 오프셋 값으로 계산
        float mouseOffsetX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float mouseOffsetY = (Input.mousePosition.y / Screen.height) - 0.5f;

        float targetAngleY = 0f;
        float targetAngleX = 0f;

        float deadZoneHalfWidth = edgePanDeadZone.x / 2f;
        float deadZoneHalfHeight = edgePanDeadZone.y / 2f;

        // 데드존을 벗어났는지 확인하고 회전 각도 계산 (좌우)
        if (Mathf.Abs(mouseOffsetX) > deadZoneHalfWidth)
        {
            float panRangeX = 0.5f - deadZoneHalfWidth;
            float panInputX = Mathf.Sign(mouseOffsetX) * (Mathf.Abs(mouseOffsetX) - deadZoneHalfWidth);
            float normalizedPanX = Mathf.Clamp(panInputX / panRangeX, -1f, 1f);
            targetAngleY = normalizedPanX * edgePanMaxAngleY;
        }

        // 데드존을 벗어났는지 확인하고 회전 각도 계산 (상하)
        if (Mathf.Abs(mouseOffsetY) > deadZoneHalfHeight)
        {
            float panRangeY = 0.5f - deadZoneHalfHeight;
            float panInputY = Mathf.Sign(mouseOffsetY) * (Mathf.Abs(mouseOffsetY) - deadZoneHalfHeight);
            float normalizedPanY = Mathf.Clamp(panInputY / panRangeY, -1f, 1f);
            targetAngleX = normalizedPanY * edgePanMaxAngleX * -1f; // Y축은 반전
        }

        // 원래 목표 회전값에 오프셋 회전을 추가
        Quaternion panOffset = Quaternion.Euler(targetAngleX, targetAngleY, 0f);
        return cameraTarget.rotation * panOffset;
    }

    /// <summary>
    /// 고정 모드 해제 시 플레이어에게 카메라 복귀
    /// </summary>
    private void ReturnNomalView()
    {
        // 원래 위치로 부드럽게 복귀
        Vector3 targetPosition = transform.TransformPoint(originalCamPosition);
        Quaternion targetRotation = transform.rotation; // 플레이어 회전 따라감

        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPosition, Time.deltaTime * cameraReturnSpeed);
        mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, targetRotation, Time.deltaTime * cameraReturnSpeed);

        // 복귀 완료(거의 도착하면 댐ㅇㅇ)
        if (Vector3.Distance(mainCam.transform.position, targetPosition) < 0.01f)
        {
            isReturningToPlayer = false;
            mainCam.transform.localPosition = originalCamPosition;
            mainCam.transform.localRotation = Quaternion.identity;
            currentCameraRotationX = 0f;
        }
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

        // 시점 고정 해제 즉시 커서를 잠그고 숨깁니다.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// 고정 시점으로 전환하는 속도
    /// </summary>
    public void SetCameraEnterSpeed(float newSpeed)
    {
        if (newSpeed > 0)
        {
            cameraEnterSpeed = newSpeed;
        }
    }

    /// <summary>
    /// 플레이어 시점으로 복귀하는 속도
    /// </summary>
    public void SetCameraReturnSpeed(float newSpeed)
    {
        if (newSpeed > 0)
        {
            cameraReturnSpeed = newSpeed;
        }
    }
}