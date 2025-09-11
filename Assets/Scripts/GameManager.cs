using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤

    public event Action OnResetSession; // 세션 리셋을 알리는 이벤트
    public event Action<Vector3> OnPlayerPosChanged;
    public event Action OnUnlockDoor;
    public event Action OnLockAllDoors;
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

        //// ItemManager가 로드된 후 이벤트를 구독합니다.
        //ItemManager.Instance.OnRequestLevelData += RequestLevelData;
        //ItemManager.Instance.OnCheckCanBuyItem += CheckCanBuyItem;
        //ItemManager.Instance.OnBuyItem += BuyItem;
    }
    private void Start()
    {
        // ItemManager가 로드된 후 이벤트를 구독합니다.
        ItemManager.Instance.OnRequestLevelData += RequestLevelData;
        ItemManager.Instance.OnCheckCanBuyItem += CheckCanBuyItem;
        ItemManager.Instance.OnBuyItem += BuyItem;
    }

    private void OnDestroy()
    {
        ItemManager.Instance.OnRequestLevelData -= RequestLevelData;
        ItemManager.Instance.OnCheckCanBuyItem -= CheckCanBuyItem;
        ItemManager.Instance.OnBuyItem -= BuyItem;
    }

    // 이 메서드는 ItemManager의 이벤트가 호출될 때 실행됩니다.
    private LevelData RequestLevelData()
    {
        return levelData;
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

        // 모든 구독자에게 세션 리셋하고, 모든 문 잠금
        OnResetSession?.Invoke();
        OnLockAllDoors?.Invoke();
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
    /// OnUnlockDoor 이벤트를 외부에서 발생시키기 위한 public 메서드
    /// </summary>
    public void TriggerUnlockDoor()
    {
        OnUnlockDoor?.Invoke();
    }

    /// <summary>
    /// 모든 문을 잠그는 OnLockAllDoors 이벤트를 발생
    /// </summary>
    public void TriggerLockAllDoors()
    {
        OnLockAllDoors?.Invoke();
    }

     /// <summary>
     /// IInitializable 인터페이스를 구현하는 모든 오브젝트를 초기화
     /// </summary>
     public void InitializeObject(IInitializable initializableObject)
     {
         initializableObject.Initialize(this);
     }

    public bool CheckCanBuyItem(int price)
    {
        if(price <= money._gold)
        {
            return true;
        }

        Debug.Log("구매하기에는 돈이 부족");
        return false;
    }

    public void BuyItem(int price)
    {
        money.SpendGold(price);
        Debug.Log(price + ": 구매");
    }
}
