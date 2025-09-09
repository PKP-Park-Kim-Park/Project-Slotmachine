using UnityEngine;
using TMPro;
using System.Collections;

public class BreakdownScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _breakdownText;
    [SerializeField] private GameObject _container; 

    private void Awake()
    {
        if (_breakdownText == null)
        {
            Debug.LogError("TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.", this);
            enabled = false;
            return;
        }

        if (transform.parent != null)
        {
            _container = transform.parent.gameObject; 
        }
    }

    private void Start()
    {
        if (_container != null) _container.SetActive(false);
    }

    /// <summary>
    /// "Not Working" 화면 활성화
    /// </summary>
    public void Show()
    {
        _breakdownText.text = "Not Working..";
        _breakdownText.color = Color.red;
        if (_container != null) _container.SetActive(true);
    }

    /// <summary>
    /// "Not Working" 화면 비활성화
    /// </summary>
    public void Hide()
    {
        if (_container != null) _container.SetActive(false);
    }
}