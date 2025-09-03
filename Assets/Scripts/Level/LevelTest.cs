using UnityEngine;

/// <summary>
/// 테스트 목적으로 레벨을 올리는 스크립트입니다.
/// </summary>
public class LevelTest : MonoBehaviour
{
    void Update()
    {
        // '+' 키 또는 '=' 키를 누르면 레벨을 1 올립니다.
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals))
        {
            if (GameManager.instance != null && GameManager.instance.levelData != null)
            {
                int currentLevel = GameManager.instance.levelData._level;
                int nextLevel = currentLevel + 1;

                Debug.Log($"레벨을 {nextLevel}(으)로 올립니다.");
                GameManager.instance.levelData.SetLevel(nextLevel);
            }
        }
    }
}
