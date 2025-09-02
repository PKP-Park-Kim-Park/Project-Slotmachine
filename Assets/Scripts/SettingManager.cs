using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button xButton;
    public Slider mouseSensitivitySlider; // 마우스 감도 슬라이더 추가
    public PlayerLook playerLook; // PlayerLook 스크립트 참조 추가

    // TMP_InputField 변수 추가
    public TMP_InputField mouseSensitivityInputField;

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

        // 슬라이더 값이 변경될 때 OnSensitivityChangedFromSlider 함수 호출
        mouseSensitivitySlider.onValueChanged.AddListener(OnSensitivityChangedFromSlider);

        // 입력 필드 값이 변경될 때 OnSensitivityChangedFromInputField 함수 호출
        mouseSensitivityInputField.onEndEdit.AddListener(OnSensitivityChangedFromInputField);



        // 시작 시 초기값 설정
        UpdateSensitivityDisplay(savedSensitivity);
    }

    void Update()
    {
        // 'P' 키를 누를 때마다 'ToggleSettings' 함수를 호출합니다.
        if (Input.GetKeyDown(KeyCode.Escape))
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
            }

            // 플레이어 카메라 스크립트 다시 활성화
            if (playerLook != null)
            {
                playerLook.enabled = true;
            }
        }
    }

    // 슬라이더 조작
    private void OnSensitivityChangedFromSlider(float newSensitivity)
    {
        // 입력 필드와 게임 감도를 업데이트
        UpdateSensitivityValue(newSensitivity);
    }

    // 입력 필드 조작
    private void OnSensitivityChangedFromInputField(string newText)
    {
        // 입력된 텍스트를 float으로 변환
        if (float.TryParse(newText, out float newSensitivity))
        {
            // 슬라이더와 게임 감도를 업데이트
            UpdateSensitivityValue(newSensitivity);
        }
    }

    // 슬라이더, 입력 필드, 게임 감도를 모두 동기화하는 공통 함수
    private void UpdateSensitivityValue(float newSensitivity)
    {
        // 슬라이더의 범위를 벗어나지 않도록 클램프
        newSensitivity = Mathf.Clamp(newSensitivity, mouseSensitivitySlider.minValue, mouseSensitivitySlider.maxValue);

        // PlayerPrefs에 감도 값 저장
        PlayerPrefs.SetFloat(MouseSensitivityKey, newSensitivity);

        // PlayerLook 스크립트의 감도 업데이트
        if (playerLook != null)
        {
            playerLook.lookSensitivity = newSensitivity;
        }

        // 슬라이더와 입력 필드 UI를 업데이트
        UpdateSensitivityDisplay(newSensitivity);
    }

    // UI(슬라이더, 입력 필드)를 업데이트하는 함수
    private void UpdateSensitivityDisplay(float newSensitivity)
    {
        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.value = newSensitivity;
        }

        if (mouseSensitivityInputField != null)
        {
            mouseSensitivityInputField.text = newSensitivity.ToString("F3");
        }
    }
}