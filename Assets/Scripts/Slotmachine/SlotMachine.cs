using System.Collections;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    private int bettingGold;
    private int rewardGold;
    private int[,] matrix; // [row, column]
    private bool isActivating;
    [SerializeField] private Reel[] reels; // 릴들을 관리할 배열
    [SerializeField] private float spinTime = 3f;

    private void Awake()
    {
        bettingGold = 0;
        rewardGold = 0;
        isActivating = false;

        if (reels == null || reels.Length == 0)
        {
            Debug.LogError("Reels are not assigned in the SlotMachine.", this);
            return;
        }
        matrix = new int[3, reels.Length]; // 3행, reel 개수만큼의 열
    }
    public void Bet(bool isIncrease)
    {
        // 슬롯머신 작동중
        if (isActivating)
        {
            return;
        }

        if (isIncrease == true)
        {
            if (GameManager.instance.levelData._maxGold < bettingGold + GameManager.instance.levelData._unitGold)
            {
                // 배팅가능한 최대치이다.
                return;
            }

            bettingGold += GameManager.instance.levelData._unitGold;
        }
        else if (isIncrease == false)
        {
            if (GameManager.instance.levelData._minGold > bettingGold - GameManager.instance.levelData._unitGold)
            {
                // 배팅가능한 최소치이다.
                return;
            }

            bettingGold -= GameManager.instance.levelData._unitGold;
        }
    }
    public void Spin()
    {
        // 슬롯머신 작동중
        if (isActivating)
        {
            return;
        }

        /*
        if (bettingGold < GameManager.instance.levelData._minGold)
        {
            // 배팅액 부족!
            return;
        }

        if (GameManager.instance.money._gold < bettingGold)
        {
            // 소지한 골드 부족
            return;
        }
        */

        // 모든 릴의 회전 애니메이션 시작
        foreach (var reel in reels)
        {
            StartCoroutine(reel.StartSpin());
        }
        isActivating = true;

        GameManager.instance.money.SpendGold(bettingGold);

        // 일정 시간 후 릴을 정지시키는 코루틴 시작
        StartCoroutine(IStopSpin());
    }

    // 릴들을 순차적으로 멈추고 결과를 처리하는 코루틴
    private IEnumerator IStopSpin()
    {
        yield return new WaitForSeconds(spinTime);

        // 각 릴을 순차적으로 멈춤
        for (int i = 0; i < reels.Length; i++)
        {
            // 1. 릴의 최종 결과를 결정 (RelocateSymbols가 내부적으로 호출됨)
            reels[i].RelocateSymbols();

            // 2. 릴의 정지 애니메이션을 실행하고 끝날 때까지 대기
            yield return StartCoroutine(reels[i].StopSpin(reels[i].row));

            // 3. 릴의 최종 결과(중앙 3개 심볼)를 가져옴
            int[] resultRow = reels[i].GetResultSymbols();

            // 4. 결과를 matrix에 저장
            ConvertMatrix(i, resultRow);

            // 릴 순차적으로 멈추는 딜레이
            yield return new WaitForSeconds(0.3f);
        }

        // 결과 로그 출력 (디버깅용)
        string resultLog = "Spin Result Matrix:\n";
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                resultLog += ((Symbols)matrix[row, col]).ToString().PadRight(10);
            }
            resultLog += "\n";
        }
        Debug.Log(resultLog);

        // 스핀 종료 및 보상/초기화 처리
        DropGold();
    }

    // 릴에서 반환된 1차원 배열(행)을 2차원 결과 매트릭스의 열에 저장
    private void ConvertMatrix(int column, int[] inputRow)
    {
        for (int row = 0; row < inputRow.Length; row++)
        {
            if (row < matrix.GetLength(0) && column < matrix.GetLength(1))
            {
                matrix[row, column] = inputRow[row];
            }
        }
    }

    // 스핀 종료 후 보상 처리
    private void DropGold()
    {
        // 보상 계산 로직
        CheckRewardPattern rewardChecker = new CheckRewardPattern();
        float finalOdds = rewardChecker.CheckReward(this.matrix);

        rewardGold = (int)(finalOdds * bettingGold);
        GameManager.instance.money.AddGold(rewardGold);

        // TODO: CheckRewardPattern 의 CheckReward 호출
        // rewardGold = CheckReward(matrix) * bettingGold;
        // GameManager.instance.money.AddGold(rewardGold);


        //결과 처리
        // money.AddGold(rewardGold)
        // bool isGameover = GameManager.CheckGameOver()
        // if(isGameover == true)
        // {
        // money.ConvertToken()
        // GameManager.Init()
        // }

        // 초기화
        bettingGold = 0;
        rewardGold = 0;
        isActivating = false;
    }
}
