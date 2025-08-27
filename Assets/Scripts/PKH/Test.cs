using UnityEngine;

public class Test : MonoBehaviour
{
    private Money money;
    private Level level;

    void Start()
    {
        money = new Money();
        Debug.Log("new Money() 골드: " + money._gold + ", 토큰: " + money._token);

        level = new Level(1);
        Debug.Log("레벨 데이터 // 레벨: " + level._level + ", 최대: " + level._maxGold + ", 최소: " + level._minGold + ", 증감단위: " + level._unitGold);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) == true)
        {
            money.AddGold(50_000);
            Debug.Log("AddGold 골드: " + money._gold + ", 토큰: " + money._token);
        }

        if (Input.GetKeyDown(KeyCode.W) == true)
        {
            bool a = money.SpendGold(30_000);
            Debug.Log("SpendGold 골드: " + money._gold + ", 토큰: " + money._token);
            if(a == false)
            {
                Debug.Log("골드 부족");
            }
        }

        if (Input.GetKeyDown(KeyCode.E) == true)
        {
            money.AddToken(2);
            Debug.Log("AddToken 골드: " + money._gold + ", 토큰: " + money._token);
        }

        if (Input.GetKeyDown(KeyCode.R) == true)
        {
            bool a = money.SpendToken(1);
            Debug.Log("SpendToken 골드: " + money._gold + ", 토큰: " + money._token);
            if (a == false)
            {
                Debug.Log("토큰 부족");
            }
        }

        if (Input.GetKeyDown(KeyCode.T) == true)
        {
            money.ConvertToken();
            Debug.Log("ConvertToken 골드: " + money._gold + ", 토큰: " + money._token);
        }
    }
}
