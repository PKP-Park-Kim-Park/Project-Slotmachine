using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    // 설정 패널을 연결할 변수
    public GameObject settingsPanel;

    // 새 게임 버튼
    public void OnNewGameButtonClicked()
    {
        // 메인 씬 로드
        DataManager.instance.LoadNewData();
        SceneManager.LoadScene("MainScene");
        Debug.Log("새 게임 시작!");
    }

    // 불러오기 버튼
    public void OnLoadGameButtonClicked()
    {
        bool canLoad = DataManager.instance.LoadGameData();

        if (canLoad)
        {
            SceneManager.LoadScene("MainScene");
            Debug.Log("게임 불러오기!");
        }
    }

    // 설정 버튼
    public void OnSettingsButtonClicked()
    {
        // 설정 패널이 연결되어 있는지 확인하고 활성화
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            Debug.Log("설정 창 열기");
        }
        else
        {
            Debug.LogWarning("설정 패널이 연결되지 않았습니다.");
        }
    }

    // 설정 창 닫기 버튼
    public void OnCloseSettingsButtonClicked()
    {
        // 설정 패널이 연결되어 있는지 확인하고 비활성화
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Debug.Log("설정 창 닫기");
        }
    }

    // 게임 종료 버튼
    public void OnQuitButtonClicked()
    {
        Debug.Log("게임 종료..");

        // 에디터에서 실행 중일 경우, 플레이 모드 중지
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서는 애플리케이션 종료
        Application.Quit();
#endif
    }
}
