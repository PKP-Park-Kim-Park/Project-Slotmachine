using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDataUI : MonoBehaviour
{
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject[] loadDataButtonList;
    [SerializeField] private Button xButton;

    private void Start()
    {
        Initialize();

        scrollView.SetActive(false);
        xButton.gameObject.SetActive(false);
    }

    private void Initialize()
    {
        GameData[] gameDatas = DataManager.instance.GetAllSlotsForUI();
        for (int i = 0; i < gameDatas.Length; i++)
        {
            int capturedIndex = i;

            TextMeshProUGUI text = loadDataButtonList[i].GetComponentInChildren<TextMeshProUGUI>();
            Button button = loadDataButtonList[i].GetComponent<Button>();

            if (text != null)
            {
                if (gameDatas[i + 1] != null)
                {
                    text.text = $"Has Data {i} !!!";
                    button.onClick.AddListener(() => OnclickSaveDataButton(capturedIndex));
                }
                else
                {
                    text.text = $"None Data";
                    button.onClick.AddListener(() => OnclickSaveDataButton(capturedIndex));
                }
            }
            else
            {
                Debug.Log("TextMeshProUGUI를 자식에게서 찾지 못함");
            }
        }
    }

    public void OnClickSave()
    {
        scrollView.SetActive(true);
        xButton.gameObject.SetActive(true);
    }

    public void OnClick_X()
    {
        scrollView.SetActive(false);
        xButton.gameObject.SetActive(false);
    }

    public void OnclickSaveDataButton(int index)
    {
        GameData gameData = GameManager.instance.SaveData();
        DataManager.instance.SaveSlotData(index, gameData);
        TextMeshProUGUI text = loadDataButtonList[index].GetComponentInChildren<TextMeshProUGUI>();
        text.text = $"Has Data {index} !!!";
    }
}
