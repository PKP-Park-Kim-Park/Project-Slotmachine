using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 이동 속도, 시야 감도, 카메라 회전 제한 값
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float lookSensitivity = 3f;
    [SerializeField] private float cameraRotationLimit = 80f;
    [SerializeField] private Camera theCamera;
    [SerializeField] private float interactRange = 3f; // 상호작용 가능한 거리

    private float currentCameraRotationX;
    private Rigidbody myRigid;

    // 현재 바라보고 있는 Outline 컴포넌트를 저장할 변수
    private Outline currentLookAtOutline;
    // 현재 바라보고 있는 InteractableObject 컴포넌트를 저장할 변수
    private InteractableObject currentLookAtInteractable;
    // 현재 레이캐스트에 맞은 오브젝트의 Transform을 저장
    private Transform currentHitTransform;

    void Start()
    {
        // Rigidbody 컴포넌트 가져오기
        myRigid = GetComponent<Rigidbody>();
        // 마우스 커서 숨기기 및 고정
        Cursor.lockState = CursorLockMode.Locked;
    }
    void FixedUpdate()
    {
        // 키보드 입력에 따른 이동
        Move();

    }
    void Update()
    {
        //상호 작용 가능한 물체를 바라보고 있는지 체크
        Interaction();
    }
    void LateUpdate()
    {
        // 마우스 좌우 움직임에 따른 캐릭터 회전
        CharacterRotation();
        // 마우스 상하 움직임에 따른 카메라 회전
        CameraRotation();
    }

    private void Move()
    {
        // 수평, 수직 입력 값 가져오기 (W,A,S,D)
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        // 이동 방향 계산
        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        // 속도를 적용하여 최종 이동 벡터 생성
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        // Rigidbody를 사용하여 물리적으로 이동
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CharacterRotation()
    {
        // 마우스 좌우 움직임 입력 값 가져오기
        float _yRotation = Input.GetAxisRaw("Mouse X");
        // 회전 벡터 계산
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;

        // Rigidbody를 사용하여 캐릭터 회전
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    private void CameraRotation()
    {
        // 마우스 상하 움직임 입력 값 가져오기
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        // 카메라 회전 값 계산
        float _cameraRotationX = _xRotation * lookSensitivity;

        currentCameraRotationX -= _cameraRotationX;
        // 카메라 회전 각도 제한
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        // 카메라 회전 적용
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    public void Interaction()
    {
        Ray ray = theCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // 1. 레이캐스트를 발사하여 현재 바라보는 오브젝트를 확인합니다.
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // 'Interactable' 태그가 있는 새로운 오브젝트를 바라보고 있는지 확인
            if (hit.transform.CompareTag("Interactable") && hit.transform != currentHitTransform)
            {
                // 이전에 바라보던 오브젝트가 있다면 외곽선을 비활성화
                if (currentLookAtOutline != null)
                {
                    currentLookAtOutline.enabled = false;
                }

                // 새로운 오브젝트의 Outline 컴포넌트를 찾아 저장하고 활성화
                currentLookAtOutline = hit.transform.GetComponent<Outline>();
                if (currentLookAtOutline != null)
                {
                    currentLookAtOutline.enabled = true;
                }

                // 새로운 오브젝트의 InteractableObject 컴포넌트를 찾아 저장
                currentLookAtInteractable = hit.transform.GetComponent<InteractableObject>();

                // 현재 바라보는 오브젝트의 Transform을 갱신
                currentHitTransform = hit.transform;
                Debug.Log("상호작용 가능한 물체를 바라보고 있습니다!");
            }
            // 기존에 바라보던 오브젝트를 계속 바라보고 있는 경우
            else if (hit.transform == currentHitTransform)
            {
                // 아무것도 하지 않음 (버벅거림 방지)
            }
        }
        else // 레이캐스트에 아무것도 맞지 않았을 경우
        {
            // 이전에 바라보던 오브젝트가 있었다면 외곽선을 비활성화
            if (currentLookAtOutline != null)
            {
                currentLookAtOutline.enabled = false;
            }
            // 현재 바라보는 오브젝트 정보 초기화
            currentLookAtOutline = null;
            currentLookAtInteractable = null;
            currentHitTransform = null;
        }

        // 2. 마우스 클릭 입력 처리
        if (currentLookAtInteractable != null && Input.GetMouseButtonDown(0))
        {
            currentLookAtInteractable.Interact();
        }
    }

}