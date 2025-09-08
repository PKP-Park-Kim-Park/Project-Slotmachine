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

    private void Start()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnLevelUp += SpawnKeyIfLevelMatches;
        }

        if (keyPrefab == null)
        {
            Debug.LogError("Key Prefab이 할당되지 않았습니다.", this);
        }
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point가 할당되지 않았습니다.", this);
        }
    }

    private void OnDestroy()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnLevelUp -= SpawnKeyIfLevelMatches;
        }
    }

    private void SpawnKeyIfLevelMatches(int newLevel)
    {
        // 현재 레벨이 목표랑 일치하고 열쇠가 없을 때 열쇠 생성
        if (newLevel == targetLevel && keyPrefab != null && spawnPoint != null && _spawnedKey == null)
        {
            // 열쇠 프리팹
            _spawnedKey = Instantiate(keyPrefab, spawnPoint.position, spawnPoint.rotation);

            // 생성된 열쇠에서 Unlock 스크립트
            Unlock unlockScript = _spawnedKey.GetComponent<Unlock>();
            if (unlockScript != null)
            {
                // 열쇠 생성기가 붙어있는 문 찾기
                unlockScript.targetDoor = GetComponent<Door>();
            }

            Debug.Log($"레벨 {newLevel} 달성! '{gameObject.name}' 문 앞에 열쇠가 생성되었습니다.");
        }
    }
}
