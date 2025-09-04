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
    private IInteractable currentLookAtInteractable;
    private Transform currentHitTransform;
    private Vector3 moveInput;

    private void Start()
    {
        myRigid = GetComponent<Rigidbody>();

        if(GameManager.instance != null )
        {
            GameManager.instance.OnPlayerPosChanged += ChangePosition;

            myRigid.MovePosition(GameManager.instance.LoadPlayerPos());

            GameManager.instance.Resume();
            playerLook.enabled = true;
        }
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
            myRigid.linearVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(_moveDirX, 0f, _moveDirZ);

        // PlayerLook 스크립트가 준비되면 상호작용 처리 시작
        if (playerLook != null && playerLook.PlayerCamera != null)
            HandleInteraction();
    }

    private void Move()
    {
        float _moveDirX = moveInput.x;
        float _moveDirZ = moveInput.z;

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _targetVelocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        _targetVelocity.y = myRigid.linearVelocity.y;

        myRigid.linearVelocity = _targetVelocity;
    }

    private void HandleInteraction()
    {
        Ray ray;
        LayerMask targetLayer;

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
        // 레이 발사
        if (Physics.Raycast(ray, out hit, interactRange, targetLayer))
        {
            if (hit.transform != currentHitTransform)
            {
                // 이전에 바라보던 오브젝트가 있다면 외곽선을 비활성화
                ClearLookAtObject();

                currentLookAtOutline = hit.transform.GetComponent<Outline>();
                if (currentLookAtOutline != null)
                {
                    currentLookAtOutline.enabled = true;
                }

                currentLookAtInteractable = hit.transform.GetComponent<IInteractable>();

                currentHitTransform = hit.transform;

            }
        }
        else // 레이캐스트에 아무것도 맞지 않았을 경우
        {
            ClearLookAtObject();
        }

        if (currentLookAtInteractable != null && Input.GetMouseButtonDown(0))
        {
            // 시점 고정 상태가 아니고, 클릭한 오브젝트가 ViewFixObject일 경우
            if (!playerLook.IsViewFixed && currentHitTransform.TryGetComponent(out ViewFixObject viewFix))
            {
                currentLookAtInteractable.Interact();

                ClearLookAtObject();
                playerLook.FixViewPoint(viewFix.GetCameraTarget());
            }
            else
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

    public void ChangePosition(Vector3 pos)
    {
        myRigid.MovePosition(pos);
    }
}