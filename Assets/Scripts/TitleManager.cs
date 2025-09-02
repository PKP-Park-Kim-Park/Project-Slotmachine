using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // 게임 시작 버튼
    public void OnStartButtonClicked()
    {
        // 메인 씬 로드
        SceneManager.LoadScene("MainScene");
        Debug.Log("게임 시작!");
    }

    // 게임 종료 버튼
    public void OnQuitButtonClicked()
    {
        Debug.Log("게임 끝..");

        // 에디터에서 실행 중일 경우, 플레이 모드 중지
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서는 애플리케이션 종료
        Application.Quit();
#endif
    }
}
