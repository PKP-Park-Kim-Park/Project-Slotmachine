using System;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤

    public event Action<Vector3> OnPlayerPosChanged;
    public event Action OnUnlockDoor;
    public event Action OnLockAllDoors;
    public event Func<int> OnRequestDoorLock;
    public event Action<bool> OnSlotMachineStateChanged;

    private bool isGaimng;
    public LevelData levelData { get; private set; }
    public Money money { get; private set; }
    public LevelManager levelManager { get; private set; }

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
        levelManager = gameObject.AddComponent<LevelManager>();
        levelManager.Initialize(money, levelData);
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
        // 값 초기화
        money.SpendToken(money._token);
        money.SpendGold(money._gold);

        money.AddGold(_gameData.gold);
        money.AddToken(_gameData.token);
        levelData.SetLevel(_gameData.level);
    }

    public Vector3 LoadPlayerPos()
    {
        if(_gameData != null)
        {
            Vector3 playerPos = _gameData.playerPos;
            _gameData = null;
            return playerPos;
        }

        return new Vector3(1f, 1f, 0f);
    }

    public void Init()
    {
        levelData.SetLevel(1);
        money.ConvertToken();
        OnPlayerPosChanged?.Invoke(new Vector3(1f, 1f, 0f));
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

    public void CheckSlotMachineStateChanged(bool newState)
    {
        OnSlotMachineStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// OnUnlockDoor 이벤트를 외부에서 발생시키기 위한 public 메서드입니다.
    /// </summary>
    public void TriggerUnlockDoor()
    {
        OnUnlockDoor?.Invoke();
    }

    /// <summary>
    /// 모든 문을 잠그는 OnLockAllDoors 이벤트를 발생시킵니다.
    /// </summary>
    public void TriggerLockAllDoors()
    {
        OnLockAllDoors?.Invoke();
    }

    /// <summary>
    /// SlotMachine에 필요한 의존성을 주입하여 초기화합니다.
    /// </summary>
    /// <param name="slotMachine">초기화할 SlotMachine 인스턴스</param>
    public void InitializeSlotMachine(SlotMachine slotMachine)
    {
        slotMachine.Initialize(this.money, this.levelData);
    }
}
