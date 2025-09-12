using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    private string autoDataFilePath;
    private string[] savelistDataFilePath = new string[10];

    public const int AutoSaveSlotIndex = -1;

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

        autoDataFilePath = Path.Combine(Application.persistentDataPath, "AutoSaveData.json");
        for (int i = 0; i < savelistDataFilePath.Length; i++)
        {
            savelistDataFilePath[i] = Path.Combine(Application.persistentDataPath, $"SlotData_{i}.json");
        }
    }

    public void SaveAutoData(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(autoDataFilePath, json);
        Debug.Log("자동 저장 완료: " + autoDataFilePath);
    }

    public void SaveSlotData(int slotIndex, GameData gameData)
    {
        if (slotIndex < 0 || slotIndex >= savelistDataFilePath.Length)
        {
            Debug.LogError("잘못된 저장 슬롯 인덱스입니다: " + slotIndex);
            return;
        }
        string filePath = savelistDataFilePath[slotIndex];
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"슬롯 {slotIndex}에 저장 완료: " + filePath);
    }

    public GameData LoadData(int slotIndex)
    {
        string filePath;

        if (slotIndex == AutoSaveSlotIndex) // -1이면 자동 저장 경로
        {
            filePath = autoDataFilePath;
        }
        else if (slotIndex >= 0 && slotIndex < savelistDataFilePath.Length) // 0~9 사이면 일반 슬롯 경로
        {
            filePath = savelistDataFilePath[slotIndex];
        }
        else
        {
            Debug.LogError("잘못된 불러오기 슬롯 인덱스입니다: " + slotIndex);
            return null;
        }

        // 2. 결정된 경로에 파일이 있는지 확인하고 데이터 불러오기
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log($"슬롯 인덱스 {slotIndex}의 데이터 불러오기 완료.");
            return data;
        }
        else
        {
            Debug.Log($"슬롯 인덱스 {slotIndex}에 저장된 데이터가 없습니다.");
            return null;
        }
    }

    public GameData[] GetAllSlotsForUI()
    {
        // 0번: 자동저장, 1~10번: 일반슬롯 0~9번
        GameData[] allData = new GameData[11];
        allData[0] = LoadData(AutoSaveSlotIndex); // 자동 저장 데이터

        for (int i = 0; i < 10; i++)
        {
            allData[i + 1] = LoadData(i); // 일반 슬롯 데이터
        }
        return allData;
    }

    public void LoadNewData()
    {
        GameData newGameData = new GameData();
        GameManager.instance.LoadData(newGameData);
    }
}