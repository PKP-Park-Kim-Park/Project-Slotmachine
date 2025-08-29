using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI tokenText;

    private void Update()
    {
        if (GameManager.instance != null && goldText != null && tokenText != null)
        {
            goldText.text = $"{GameManager.instance.money._gold}";
            tokenText.text = $"{GameManager.instance.money._token}";
        }
    }
}
