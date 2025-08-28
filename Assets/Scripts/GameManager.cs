using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤

    private bool isGaimng;
    private bool isPause;
    public LevelData levelData { get; private set; }
    public Money money { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        levelData = new LevelData(1);
        money = new Money();
    }

    public void Init()
    {
        levelData.SetLevel(1);
        // todo: 플레이어 위치이동
    }

    public bool CheckGameOver()
    {
        if(isGaimng)
        {
            if(levelData._minGold > money._gold)
            {
                return true;
            }
        }

        return false;
    }

    public void Pause()
    {
        if(!isPause)
        {
            Time.timeScale = 0f;
            isPause = true;
        }
        else
        {
            Time.timeScale = 1f;
            isPause = false;
        }
    }
}
