using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    private string dataFilePath;

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

        // 저장 경로 설정
        dataFilePath = Path.Combine(Application.persistentDataPath, "GameData.json");
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.F5))
        {
            GameData gameData = GameManager.instance.SaveData();
            SaveGameData(gameData);
        }
    }

    public void SaveGameData(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(dataFilePath, json);

        Debug.Log("게임 데이터 저장 완료: " + dataFilePath);
    }

    public void LoadGameData()
    {
        if (File.Exists(dataFilePath))
        {
            string json = File.ReadAllText(dataFilePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("게임 데이터 로드 완료.");

            GameManager.instance.LoadData(gameData);
        }
        else
        {
            Debug.Log("저장된 파일이 없어");
        }
    }
}