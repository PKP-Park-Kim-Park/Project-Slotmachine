using System;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤

    public event Action<Vector3> OnPlayerPosChanged;
    public event Action OnUnlockDoor;
    public event Func<int> OnRequestDoorLock;

    private bool isGaimng;
    public LevelData levelData { get; private set; }
    public Money money { get; private set; }

    private GameData _gameData;

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

        levelData = new LevelData(1);
        money = new Money(100_000, 0);

        // 레벨 변경 시 잠금해제
        levelData.OnLevelChanged += HandleLevelChange;
    }

    private void OnDestroy()
    {
        if (levelData != null)
        {
            levelData.OnLevelChanged -= HandleLevelChange;
        }
    }

    public GameData SaveData()
    {
        GameData gameData = new GameData();
        gameData.gold = money._gold;
        gameData.token = money._token;
        gameData.level = levelData._level;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            gameData.playerPos = playerObject.transform.position;
        }

        return gameData;
    }

    public void LoadData(GameData gameData)
    {
        _gameData = gameData;
        money.AddGold(_gameData.gold - 100_000);
        money.AddToken(_gameData.token);
        levelData.SetLevel(_gameData.level);
    }

    public Vector3 LoadPlayerPos()
    {
        return _gameData.playerPos;
    }

    public void Init()
    {
        levelData.SetLevel(1);
        money.ConvertToken();
        OnPlayerPosChanged?.Invoke(new Vector3(1f, 1f, 0f));
        // Init() 시점에 문 잠금 해제 이벤트를 발생시킵니다.
        OnUnlockDoor?.Invoke();
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
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void HandleLevelChange()
    {
        Debug.Log($"레벨이 {levelData._level}(으)로 변경되었습니다. 문 잠금 해제를 시도합니다.");
        OnUnlockDoor?.Invoke();
    }
}
