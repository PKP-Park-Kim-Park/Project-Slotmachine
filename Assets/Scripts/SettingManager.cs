using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button xButton;
    public Slider mouseSensitivitySlider; // 마우스 감도 슬라이더 추가
    private PlayerLook playerLook; // PlayerLook 스크립트 참조 추가

    // 사운드 조절 관련 변수
    public AudioMixer mixer; // 오디오 믹서 변수 추가
    public Slider bgmSlider; // BGM 슬라이더 추가
    public Slider sfxSlider; // SFX 슬라이더 추가

    // TMP_InputField 변수 추가
    public TMP_InputField mouseSensitivityInputField;
    // 텍스트 변수 추가
    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;

    public Button saveButton;
    public Button exitButton;

    private const string MouseSensitivityKey = "MouseSensitivity";
    private const string BgmVolumeKey = "BgmVolume";
    private const string SfxVolumeKey = "SfxVolume";

    private bool isSlotmachineState = false;
    void Start()
    {
        // 게임 시작 시 설정 패널을 비활성화(숨기기)합니다.
        settingsPanel.SetActive(false);
        // X 버튼 클릭 시 'ToggleSettings' 함수를 실행하도록 설정합니다.
        xButton.onClick.AddListener(ToggleSettings);


        float savedSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, 3f);
        mouseSensitivitySlider.value = savedSensitivity;
        mouseSensitivitySlider.onValueChanged.AddListener(OnSensitivityChangedFromSlider);
        mouseSensitivityInputField.onEndEdit.AddListener(OnSensitivityChangedFromInputField);
        // 시작 시 초기값 설정
        UpdateSensitivityValue(savedSensitivity);

        // BGM 슬라이더 설정
        float savedBgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, 0.75f);
        bgmSlider.value = savedBgmVolume;
        bgmSlider.onValueChanged.AddListener(SetBgmVolume);

        // SFX 슬라이더 설정
        float savedSfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.75f);
        sfxSlider.value = savedSfxVolume;
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        // 초기 텍스트 값 설정

        SetBgmVolume(savedBgmVolume);
        SetSfxVolume(savedSfxVolume);

        GameObject playerObject = GameObject.FindWithTag("Player");
        playerLook = playerObject.GetComponent<PlayerLook>();

        saveButton.onClick.AddListener(OnclickSaveButton);
        exitButton.onClick.AddListener(OnclickExitButton);

        GameManager.instance.OnSlotMachineStateChanged += HandleSlotStateChange;
    }

    void Update()
    {
        if (isSlotmachineState &&
            Input.GetKeyDown(KeyCode.Escape))
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
    public void SetBgmVolume(float volume)
    {
        // PlayerPrefs에 값 저장
        PlayerPrefs.SetFloat(BgmVolumeKey, volume);
        // 믹서의 BGM 그룹 볼륨 조절
        mixer.SetFloat("BGM_Volume", Mathf.Log10(volume) * 20);
        UpdateBgmText(volume);
    }
    public void SetSfxVolume(float volume)
    {
        // PlayerPrefs에 값 저장
        PlayerPrefs.SetFloat(SfxVolumeKey, volume);

        // 믹서의 SFX 그룹 볼륨 조절
        mixer.SetFloat("SFX_Volume", Mathf.Log10(volume) * 20);
        UpdateSfxText(volume);
    }
    // BGM 텍스트 UI 업데이트 함수
    private void UpdateBgmText(float volume)
    {
        if (bgmText != null)
        {
            bgmText.text = (volume * 100).ToString("F0") + "%";
        }
    }

    // SFX 텍스트 UI 업데이트 함수
    private void UpdateSfxText(float volume)
    {
        if (sfxText != null)
        {
            sfxText.text = (volume * 100).ToString("F0") + "%";
        }
    }

    public void OnclickSaveButton()
    {
        GameData gameData = GameManager.instance.SaveData();
        DataManager.instance.SaveGameData(gameData);
    }

    public void OnclickExitButton()
    {
        GameData gameData = GameManager.instance.SaveData();
        DataManager.instance.SaveGameData(gameData);
        SceneManager.LoadScene("Title");
    }

    private void HandleSlotStateChange(bool newState)
    {
        isSlotmachineState = newState;
    }
}