using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;

    [Header("Dependencies")]
    [SerializeField] private PlayerLook playerLook; // 시점 및 카메라 제어 스크립트

    private Rigidbody myRigid;

    // 상호작용 상태 관련 변수
    // 현재 바라보고 있는 Outline 컴포넌트를 저장할 변수
    private Outline currentLookAtOutline;
    // 현재 바라보고 있는 InteractableObject 컴포넌트를 저장할 변수
    private IInteractable currentLookAtInteractable;
    // 현재 레이캐스트에 맞은 오브젝트의 Transform을 저장
    private Transform currentHitTransform;

    void Start()
    {
        // Rigidbody 컴포넌트 가져오기
        myRigid = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
            Move();
    }
    void Update()
    {
            HandleInteraction();
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        // 속도를 적용하여 최종 이동 벡터 생성
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        // Rigidbody를 사용하여 물리적으로 이동
        myRigid.MovePosition(transform.position + _velocity * Time.fixedDeltaTime);
    }

    private void HandleInteraction()
    {
        Ray ray = playerLook.PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
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

                // 새로운 오브젝트에서 IInteractable 인터페이스를 구현한 컴포넌트를 찾아 저장
                currentLookAtInteractable = hit.transform.GetComponent<IInteractable>();

                // 현재 바라보는 오브젝트의 Transform을 갱신
                currentHitTransform = hit.transform;
                Debug.Log(currentLookAtInteractable.InteractionPrompt + "와(과) 상호작용 가능합니다!");
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
            // 시점 고정이 필요한 오브젝트인지 확인
            ViewFixObject viewFix = currentHitTransform.GetComponent<ViewFixObject>();
            if (viewFix != null)
            {
                // 시점 고정 전, 현재 바라보고 있는 오브젝트의 아웃라인을 끈다.
                if (currentLookAtOutline != null)
                {
                    currentLookAtOutline.enabled = false;
                    currentLookAtOutline = null;
                    currentLookAtInteractable = null;
                    currentHitTransform = null;
                }
                playerLook.FixViewPoint(viewFix.GetCameraTarget());
            }
            else
            {
                currentLookAtInteractable.Interact();
            }
        }
    }
}