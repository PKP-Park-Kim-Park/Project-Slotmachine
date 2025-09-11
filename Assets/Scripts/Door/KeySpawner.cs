using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    [Tooltip("생성할 열쇠 프리팹")]
    public GameObject keyPrefab;

    [Tooltip("열쇠가 생성될 위치")]
    public Transform spawnPoint;

    [Tooltip("이 열쇠가 생성될 레벨")]
    public int targetLevel = 2;

    private GameObject _spawnedKey;
    private PlayerLook _playerLook;

    private void Start()
    {
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnLevelUp += SpawnKey;
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.OnResetSession += HandleSessionReset;
        }

        _playerLook = FindFirstObjectByType<PlayerLook>();

        if (keyPrefab == null)
        {
            Debug.LogError("Key Prefab이 할당되지 않았습니다.", this);
        }
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point가 할당되지 않았습니다.", this);
        }

        // 게임 시작 시점에도 레벨을 확인하여 열쇠를 생성할지 결정
        if (GameManager.instance != null && GameManager.instance.levelData != null)
        {
            SpawnKey(GameManager.instance.levelData._level);
        }
    }

    private void OnDestroy()
    {
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnLevelUp -= SpawnKey;
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.OnResetSession -= HandleSessionReset;
        }
    }

    private void SpawnKey(int newLevel)
    {
        // 현재 레벨이 목표랑 일치하고 열쇠가 없을 때 열쇠 생성
        if (newLevel == targetLevel && keyPrefab != null && spawnPoint != null && _spawnedKey == null)
        {
            // 열쇠 프리팹
            _spawnedKey = Instantiate(keyPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"[KeySpawner] '{keyPrefab.name}' 열쇠를 '{gameObject.name}' 문 앞에 생성했습니다. (TargetLevel: {targetLevel}, CurrentLevel: {newLevel})");

            // 생성된 열쇠에서 Unlock 스크립트
            Unlock unlockScript = _spawnedKey.GetComponent<Unlock>();
            if (unlockScript != null)
            {
                // 열쇠 생성기가 붙어있는 문 찾기
                unlockScript.targetDoor = GetComponent<Door>();
            }

            // 플레이어 시점이 고정된 상태라면 해제하고 생성된 키를 바라보게 함
            if (_playerLook != null && _playerLook.IsViewFixed)
            {
                _playerLook.UnfixViewAndLookAt(_spawnedKey.transform);
            }
        }
    }

    /// <summary>
    /// 게임 세션이 리셋될 때 호출
    /// </summary>
    private void HandleSessionReset()
    {
        // 이미 생성된 열쇠가 있다면 파괴
        if (_spawnedKey != null)
        {
            Destroy(_spawnedKey);
            _spawnedKey = null;
        }

        // 현재 레벨에 맞춰 열쇠 생성
        SpawnKey(GameManager.instance.levelData._level);
    }
}
