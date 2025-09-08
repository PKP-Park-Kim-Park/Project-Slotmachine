using UnityEngine;

/// <summary>
/// 플레이어 시점 및 카메라 회전 관리
/// </summary>
public class PlayerLook : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] public float lookSensitivity = 3f;
    [SerializeField] private float maxTilt = 80f;
    [SerializeField] private Camera mainCam;

    [Header("Fixed View Settings")]
    [SerializeField] private float cameraEnterSpeed = 5f; // 고정 시점으로 전환하는 속도
    [SerializeField] private float cameraReturnSpeed = 5f; // 플레이어 시점으로 복귀하는 속도
    [SerializeField] private Vector2 edgePanDeadZone = new Vector2(0.7f, 0.7f); // 화면 중앙의 데드존 크기 (0~1)
    [SerializeField] private float edgePanMaxAngleX = 5f; // 상하 시야 이동 최대 각도
    [SerializeField] private float edgePanMaxAngleY = 10f; // 좌우 시야 이동 최대 각도

    private float currentTilt = 0f;
    private Rigidbody rb;

    public GameObject crosshair;

    // 시점 고정 상태 관련 변수
    private bool isViewFixed = false;
    private bool isReturningToPlayer = false; // 카메라 복귀 상태
    private Transform cameraTarget;
    private Vector3 originalCamPosition; // 카메라 원래 위치
    private SlotMachine currentSlotMachine; // 현재 상호작용 중인 슬롯머신

    /// <summary>
    /// 현재 시점이 고정된 상태인지 여부
    /// </summary>
    public bool IsViewFixed => isViewFixed;
    public Camera PlayerCamera => mainCam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalCamPosition = mainCam.transform.localPosition;

        lookSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", lookSensitivity);

        // 커서 초기 상태 설정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 시점이 고정된 상태이고, 슬롯머신이 작동 중이 아닐 때 Q 키를 누르면 시점 고정 해제
        if (isViewFixed && Input.GetKeyDown(KeyCode.Q) && (currentSlotMachine == null || !currentSlotMachine.IsActivating))
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
        float mouseY = Input.GetAxis("Mouse Y");
        float tiltAmount = mouseY * lookSensitivity;

        currentTilt -= tiltAmount;
        currentTilt = Mathf.Clamp(currentTilt, -maxTilt, maxTilt);

        Vector3 angles = mainCam.transform.localEulerAngles;
        angles.x = currentTilt;
        mainCam.transform.localEulerAngles = angles;
    }

    // 일반 플레이 모드 시 시점 처리
    private void HandleNormalView()
    {
        CharacterRotation();
        CameraRotation();
    }

    /// 고정 모드일 때 카메라 움직임 처리
    private void HandleFixedView()
    {
        // 크로스헤어 비활성
        crosshair.SetActive(false);

        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, cameraTarget.position, Time.deltaTime * cameraEnterSpeed);

        Quaternion finalTargetRotation = CalculateEdgePanRotation();

        mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, finalTargetRotation, Time.deltaTime * cameraEnterSpeed);
    }

    /// HandleFixedView에서 호출(계산)
    private Quaternion CalculateEdgePanRotation()
    {
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
            targetAngleX = normalizedPanY * edgePanMaxAngleX * -1f;
        }

        Quaternion panOffset = Quaternion.Euler(targetAngleX, targetAngleY, 0f);
        return cameraTarget.rotation * panOffset;
    }

    // 고정 모드 해제 시 플레이어에게 카메라 복귀
    private void ReturnNomalView()
    {
        // 크로스헤어 활성
        crosshair.SetActive(true);

        Vector3 targetPosition = transform.TransformPoint(originalCamPosition);
        Quaternion targetRotation = transform.rotation; // 플레이어 회전 따라감

        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPosition, Time.deltaTime * cameraReturnSpeed);
        mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, targetRotation, Time.deltaTime * cameraReturnSpeed);

        if (Vector3.Distance(mainCam.transform.position, targetPosition) < 0.01f)
        {
            isReturningToPlayer = false;
            mainCam.transform.localPosition = originalCamPosition;
            mainCam.transform.localRotation = Quaternion.identity;
            currentTilt = 0f;
        }
    }

    public void FixViewPoint(Transform target)
    {
        isViewFixed = true;
        isReturningToPlayer = false;
        cameraTarget = target;
        currentSlotMachine = target.GetComponentInParent<SlotMachine>(); // 상호작용한 슬롯머신 참조

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager.instance.CheckSlotMachineStateChanged(false);
    }

    public void UnfixViewPoint()
    {
        isViewFixed = false;
        isReturningToPlayer = true; // 복귀 전환 시작
        cameraTarget = null;
        currentSlotMachine = null; // 슬롯머신 참조 해제

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.instance.CheckSlotMachineStateChanged(true);
    }

    /// 고정 시점으로 전환하는 속도
    public void SetCameraEnterSpeed(float newSpeed)
    {
        if (newSpeed > 0)
        {
            cameraEnterSpeed = newSpeed;
        }
    }

    /// 플레이어 시점으로 복귀하는 속도
    public void SetCameraReturnSpeed(float newSpeed)
    {
        if (newSpeed > 0)
        {
            cameraReturnSpeed = newSpeed;
        }
    }
}
