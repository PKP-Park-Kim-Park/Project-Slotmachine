using System.Collections;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    private int bettingGold;
    private int rewardGold;
    private int[,] matrix;
    private bool isActivating;
    [SerializeField] private float spinTime = 3f;

    private void Awake()
    {
        bettingGold = 0;
        rewardGold = 0;
        isActivating = false;
        matrix = new int[3, 5];
    }

    public void Bet(bool isIncrease)
    {
        // 슬롯머신 작동중
        if(isActivating == true)
        {
            return;
        }

        if(isIncrease == true)
        {
            // 레벨의 증감액 보다 골드가 많으면
            if(GameManager.instance.levelData._unitGold < GameManager.instance.money._gold)
            {
                bettingGold += GameManager.instance.levelData._unitGold;
                GameManager.instance.money.SpendGold(GameManager.instance.levelData._unitGold);
            }
            else
            {
                // 돈부족 !
            }
        }
        else if(isIncrease == false)
        {
            // 레벨의 증감액 보다 베팅 골드가 많으면
            if (GameManager.instance.levelData._unitGold < bettingGold)
            {
                bettingGold -= GameManager.instance.levelData._unitGold;
                GameManager.instance.money.AddGold(GameManager.instance.levelData._unitGold);
            }
            else
            {
                // 돈부족 !
            }
        }
    }

    public void Spin()
    {
        // 슬롯머신 작동중
        if (isActivating == true)
        {
            return;
        }

        if (bettingGold < GameManager.instance.levelData._minGold)
        {
            // 배팅액 부족!
            return;
        }

        isActivating = true;

        // Reel 의 RelocateSymbol 함수 호출

        StartCoroutine(IStopSpin());
    }

    private IEnumerator IStopSpin()
    {
        yield return new WaitForSeconds(spinTime);

        // Reel 의 Real[N].StopSpin() 호출 N = 상수
        // ㄴ ConvertMatrix(N, Real[N].StopSpin())

        yield return null;

        DropGold();
    }

    private void ConvertMatrix(int column, int[] inputRow)
    {
        // matrix[0,column] = inputRow[0], matrix[1,column] = inputRow[1], matrix[2,column] = result[2]
    }

    private void DropGold()
    {
        // CheckRewardPattern 의 CheckReward 호출
        // ㄴ int Odds = CheckReward(matrix)
        // rewardGold = Odds * bettingGold


        //결과 처리
        // money.AddGold(rewardGold)
        // bool isGameover = GameManager.CheckGameOver()
        // if(isGameover == true)
        {
            // money.ConvertToken()
            // GameManager.Init()
        }

        // 초기화
        bettingGold = 0;
        rewardGold = 0;
        isActivating = false;
    }
}
