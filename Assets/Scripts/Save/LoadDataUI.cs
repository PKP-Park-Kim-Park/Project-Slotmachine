using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadDataUI : MonoBehaviour
{
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject[] loadDataButtonList;
    [SerializeField] private TitleManager titleManager;

    private void Start()
    {
        Initialize();

        scrollView.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            scrollView.SetActive(false);
        }
    }

    private void Initialize()
    {
        GameData[] gameDatas =  DataManager.instance.GetAllSlotsForUI();
        for(int i = 0; i < gameDatas.Length; i++)
        {
            int capturedIndex = i;

            TextMeshProUGUI text = loadDataButtonList[i].GetComponentInChildren<TextMeshProUGUI>();
            Button button = loadDataButtonList[i].GetComponent<Button>();

            if (text != null)
            {
                if (gameDatas[i] != null)
                {
                    if(capturedIndex == 0)
                    {
                        text.text = $"Auto Save Data ???";
                    }
                    else
                    {
                        text.text = $"Has Data {i - 1} !!!";
                    }

                    button.onClick.AddListener(() => OnclickLoadDataButton(capturedIndex));
                }
                else
                {
                    text.text = $"None Data";
                    button.enabled = false;
                }
            }
            else
            {
                Debug.Log("TextMeshProUGUI를 자식에게서 찾지 못함");
            }
        }
    }

    public void OnClickLoad()
    {
        scrollView.SetActive(true);
    }

    public void OnclickLoadDataButton(int index)
    {
        titleManager.OnLoadGameButtonClicked(index - 1);
    }
}
