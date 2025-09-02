using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button xButton;
    public Slider mouseSensitivitySlider; // 마우스 감도 슬라이더 추가
    public PlayerLook playerLook; // PlayerLook 스크립트 참조 추가

    private const string MouseSensitivityKey = "MouseSensitivity";

    void Start()
    {
        // 게임 시작 시 설정 패널을 비활성화(숨기기)합니다.
        settingsPanel.SetActive(false);

        // X 버튼 클릭 시 'ToggleSettings' 함수를 실행하도록 설정합니다.
        xButton.onClick.AddListener(ToggleSettings);

        // 슬라이더의 값을 초기화하고 저장된 값으로 설정합니다.
        // 저장된 값이 없으면 기본값인 3f로 설정합니다.
        float savedSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, 3f);
        mouseSensitivitySlider.value = savedSensitivity;

        // 슬라이더 값이 변경될 때마다 OnMouseSensitivityChanged 함수를 호출합니다.
        mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
    }

    void Update()
    {
        // 'P' 키를 누를 때마다 'ToggleSettings' 함수를 호출합니다.
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        bool isPanelActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isPanelActive);

        if (isPanelActive)
        {
            // 설정창이 열릴 때 GameManager의 PauseGame 함수 호출
            if (GameManager.instance != null)
            {
                GameManager.instance.Pause();
                Cursor.visible = true;
            }

            // 플레이어 카메라 스크립트 비활성화
            if (playerLook != null)
            {
                playerLook.enabled = false;
            }
        }
        else
        {

            // 설정창이 닫힐 때 GameManager의 ResumeGame 함수 호출
            if (GameManager.instance != null)
            {
                GameManager.instance.Resume();
                Cursor.visible = false;
            }

            // 플레이어 카메라 스크립트 다시 활성화
            if (playerLook != null)
            {
                playerLook.enabled = true;
            }
        }
    }

    // 슬라이더 값이 변경될 때 호출되는 함수
    private void OnMouseSensitivityChanged(float newSensitivity)
    {
        // 마우스 감도 값을 PlayerPrefs에 저장합니다.
        PlayerPrefs.SetFloat(MouseSensitivityKey, newSensitivity);

        // PlayerLook 스크립트에 새로운 감도 값을 전달합니다.
        if (playerLook != null)
        {
            playerLook.lookSensitivity = newSensitivity;
        }
    }
}