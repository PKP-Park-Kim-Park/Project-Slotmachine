using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    // 시점 고정 시 상호작용할 레이어
    [SerializeField] private LayerMask slotMachineInteractableLayer;
    // 기본 상호작용 레이어
    [SerializeField] private LayerMask defaultInteractableLayer;

    [Header("Dependencies")]
    [SerializeField] private PlayerLook playerLook; // 시점 및 카메라 제어 스크립트

    private Rigidbody myRigid;
    private Outline currentLookAtOutline;
    // 현재 바라보고 있는 InteractableObject 컴포넌트를 저장할 변수
    private IInteractable currentLookAtInteractable;
    // 현재 레이캐스트에 맞은 오브젝트의 Transform을 저장
    private Transform currentHitTransform;
    private Vector3 moveInput; // 이동 입력을 저장할 변수

    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        // 시점이 고정되어 있지 않을 때만 이동
        if (!playerLook.IsViewFixed)
        {
            Move();
        }
        else
        {
            // 시점이 고정되면 움직임을 멈춥니다.
            myRigid.linearVelocity = Vector3.zero;
        }
    }
    void Update()
    {
        // 매 프레임
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(_moveDirX, 0f, _moveDirZ);

        // PlayerLook 스크립트가 준비되면 상호작용 처리 시작
        if (playerLook != null && playerLook.PlayerCamera != null)
            HandleInteraction();
    }

    private void Move()
    {
        // FixedUpdate에서 물리 효과를 적용
        float _moveDirX = moveInput.x;
        float _moveDirZ = moveInput.z;

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        // 속도를 적용하여 최종 이동 벡터 생성
        Vector3 _targetVelocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        // y축 속도는 유지하여 중력 등의 효과를 보존
        _targetVelocity.y = myRigid.linearVelocity.y;

        // Rigidbody의 속도를 직접 제어하여 부드러운 움직임
        myRigid.linearVelocity = _targetVelocity;
    }

    private void HandleInteraction()
    {
        Ray ray;
        LayerMask targetLayer;

        // 1. 시점 고정 상태에 따라 레이와 타겟 레이어를 다르게 설정합니다.
        if (playerLook.IsViewFixed)
        {
            // 시점이 고정된 경우: 마우스 위치에서 레이를 쏘고, 슬롯머신 레이어만 감지
            ray = playerLook.PlayerCamera.ScreenPointToRay(Input.mousePosition);
            targetLayer = slotMachineInteractableLayer;
        }
        else
        {
            // 일반 모드인 경우: 화면 중앙에서 레이를 쏘고, 기본 상호작용 레이어를 감지
            ray = playerLook.PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            targetLayer = defaultInteractableLayer;
        }

        RaycastHit hit;

        // 2. 설정된 레이와 레이어 마스크로 레이캐스트를 발사합니다.
        if (Physics.Raycast(ray, out hit, interactRange, targetLayer))
        {
            // 새로운 오브젝트를 바라보고 있는지 확인
            if (hit.transform != currentHitTransform)
            {
                // 이전에 바라보던 오브젝트가 있다면 외곽선을 비활성화
                ClearLookAtObject();

                // 새로운 오브젝트의 Outline 컴포넌트를 찾아 저장하고 활성화
                currentLookAtOutline = hit.transform.GetComponent<Outline>();
                if (currentLookAtOutline != null)
                {
                    currentLookAtOutline.enabled = true;
                }

                // 새로운 오브젝트에서 IInteractable 인터페이스를 구현한 컴포넌트를 찾아 저장
                currentLookAtInteractable = hit.transform.GetComponent<IInteractable>();

                // 현재 바라보는 오브젝트의 Transform을 갱신
                currentHitTransform = hit.transform;

                if (currentLookAtInteractable != null)
                {
                    Debug.Log(currentLookAtInteractable.InteractionPrompt + "와(과) 상호작용 가능합니다!");
                }
            }
        }
        else // 레이캐스트에 아무것도 맞지 않았을 경우
        {
            ClearLookAtObject();
        }

        // 3. 마우스 클릭 입력 처리
        if (currentLookAtInteractable != null && Input.GetMouseButtonDown(0))
        {
            // 시점 고정 상태가 아니고, 클릭한 오브젝트가 ViewFixObject일 경우
            if (!playerLook.IsViewFixed && currentHitTransform.TryGetComponent(out ViewFixObject viewFix))
            {
                // 1. 상호작용(AutoLockTrigger 등)을 먼저 호출
                currentLookAtInteractable.Interact();

                // 2. 그 다음 시점을 고정
                ClearLookAtObject();
                playerLook.FixViewPoint(viewFix.GetCameraTarget());
            }
            else // 그 외의 모든 상호작용 (레버, 버튼 등)
            {
                currentLookAtInteractable.Interact();
            }
        }
    }

    // 바라보는 오브젝트 정보를 초기화하는 함수
    private void ClearLookAtObject()
    {
        if (currentLookAtOutline != null)
        {
            currentLookAtOutline.enabled = false;
        }
        currentLookAtOutline = null;
        currentLookAtInteractable = null;
        currentHitTransform = null;
    }
}