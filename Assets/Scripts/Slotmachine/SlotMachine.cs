using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public event Action OnActivationStart;  // 슬롯머신 동작 시작
    public event Action OnActivationEnd;    // 슬롯머신 동작 종료
    public event Action<int> OnBetGoldChanged; // 베팅 골드 변경 시
    public event Action OnBetAttemptFailed; // 베팅 +/- 실패 시
    public event Action<int> OnRewardGold;  // 보상 골드 변경 시
    //===============================================
    private bool isActivating;
    public bool IsActivating
    {
        get { return isActivating; }
        private set
        {
            if (isActivating == value) return;
            isActivating = value;

            if (isActivating) OnActivationStart?.Invoke();
            else OnActivationEnd?.Invoke();
        }
    }
    private int bettingGold;
    public int BettingGold
    {
        get { return bettingGold; }
        private set
        {
            if (bettingGold == value) return;
            bettingGold = value;
            OnBetGoldChanged?.Invoke(value);
        }
    }
    private int rewardGold;
    public int RewardGold
    {
        get { return rewardGold; }
        private set
        {
            if (rewardGold == value) return;
            rewardGold = value;
            OnRewardGold?.Invoke(value);
        }
    }
    private int[,] matrix;
    [SerializeField] private Reel[] reels; // 릴들을 관리할 배열
    [SerializeField] private float spinTime = 3f;

    [Header("Reward System")]
    [Tooltip("당첨 패턴 애니메이션을 담당하는 컴포넌트")]
    [SerializeField] private PatternAnimator patternAnimator;
    [Tooltip("패턴별 배율 정보가 담긴 ScriptableObject")]
    [SerializeField] private PatternRewardOdds patternOddsData;
    [Tooltip("심볼별 배율 정보가 담긴 ScriptableObject")]
    [SerializeField] private SymbolRewardOdds symbolOddsData;
    private CheckRewardPattern rewardChecker;
    private void Awake()
    {
        bettingGold = 0;
        rewardGold = 0;
        isActivating = false;

        if (reels == null || reels.Length == 0)
        {
            Debug.LogError("슬롯머신에 릴이 없습니다.", this);
            return;
        }
        matrix = new int[3, reels.Length]; // 3행, reel 개수만큼의 열

        rewardChecker = new CheckRewardPattern();
    }
    private void Start()
    {
        BettingGold = GameManager.instance.levelData._minGold;
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
                OnBetAttemptFailed?.Invoke();
                // 배팅가능한 최대치
                return;
            }

            BettingGold += GameManager.instance.levelData._unitGold;
        }
        else if (isIncrease == false)
        {
            if (GameManager.instance.levelData._minGold > bettingGold - GameManager.instance.levelData._unitGold)
            {
                OnBetAttemptFailed?.Invoke();
                // 배팅가능한 최소치
                return;
            }

            BettingGold -= GameManager.instance.levelData._unitGold;
        }
    }
    public void Spin()
    {
        // 슬롯머신 작동중
        if (isActivating)
        {
            return;
        }

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

        // 이전 스핀의 당첨 테두리 제거
        if (patternAnimator != null)
        {
            patternAnimator.ClearBorders();
        }

        // 모든 릴의 회전 애니메이션 시작
        foreach (var reel in reels)
        {
            StartCoroutine(reel.StartSpin());
        }

        GameManager.instance.money.SpendGold(bettingGold);
        IsActivating = true;

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
            reels[i].RelocateSymbols();

            yield return StartCoroutine(reels[i].StopSpin(reels[i].row));

            int[] resultRow = reels[i].GetResultSymbols();
            // 4. 결과를 matrix에 저장
            ConvertMatrix(i, resultRow);

            yield return new WaitForSeconds(0.3f);
        }

        // 결과 로그 출력 (디버깅용)
        /*
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
        */

        // 스핀 종료 후 애니메이션과 당첨 보상 처리
        StartCoroutine(IDropGold());
    }

    // 3X5 행렬로 변환
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

    // 스핀 종료 후 애니메이션 및 보상 처리 코루틴
    private IEnumerator IDropGold()
    {
        // 1. 보상 패턴 확인 및 배율 계산
        float totalOdds = rewardChecker.CheckReward(matrix, patternOddsData, symbolOddsData);

        // 2. 당첨 라인 정보 가져오기
        List<WinningLine> winningLines = rewardChecker.WinningLines;

        // 3. 당첨 라인이 있으면 애니메이션 실행
        if (winningLines.Count > 0)
        {
            Debug.Log($"{winningLines.Count}개의 라인 당첨! 총 배율: {totalOdds}");

            // 애니메이터에게 당첨 라인 목록을 전달하여 애니메이션 실행
            if (patternAnimator != null)
            {
                patternAnimator.AnimateWinning(winningLines);
                // 애니메이션이 끝날 때까지 대기
                yield return new WaitUntil(() => !patternAnimator.IsAnimating);
            }

            // 4. 최종 보상 계산 및 지급
            RewardGold = (int)(totalOdds * bettingGold);

            yield return new WaitForSeconds(2.0f);
            GameManager.instance.money.AddGold(rewardGold);
        }
        else
        {
            Debug.Log("당첨된 라인이 없습니다.");
        }

        // 초기화
        BettingGold = GameManager.instance.levelData._minGold;
        RewardGold = 0;
        IsActivating = false;
    }
}
