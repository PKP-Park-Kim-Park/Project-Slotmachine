using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    // 슬롯머신의 상태를 나타내는 열거형
    public enum SlotMachineState
    {
        Idle,           // 대기
        Spinning,       // 릴 회전 중
        Stopping,       // 릴 정지 중
        ShowingResults  // 결과 표시 중
    }

    public event Action OnActivationStart;  // 슬롯머신 동작 시작
    public event Action OnActivationEnd;    // 슬롯머신 동작 종료
    public event Action<int> OnBetGoldChanged; // 베팅 골드 변경 시
    public event Action OnBetAttemptFailed; // 베팅 +/- 실패 시
    public event Action<int> OnRewardGold;  // 보상 골드 변경 시
    //===============================================
    private SlotMachineState _currentState;
    public SlotMachineState CurrentState
    {
        get { return _currentState; }
        private set
        {
            if (_currentState == value) return;
            
            var previousState = _currentState;
            _currentState = value;

            // 슬롯머신이 활성화되거나 비활성화되는 시점에 이벤트 호출
            bool wasActivating = IsActivatingState(previousState);
            bool isActivating = IsActivatingState(_currentState);
            if (!wasActivating && isActivating) OnActivationStart?.Invoke();
            else if (wasActivating && !isActivating) OnActivationEnd?.Invoke();
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

    // IsActivating 프로퍼티를 상태 기반으로 계산하여 제공
    public bool IsActivating
    {
        get { return IsActivatingState(CurrentState); }
    }
    
    [SerializeField] private Reel[] reels; // 릴들을 관리할 배열
    [SerializeField] private float spinTime = 3f;
    [Tooltip("릴이 순차적으로 정지할 때의 시간 간격 (초)")]
    [SerializeField] private float reelStopDelay = 0.3f;

    [Header("Probabilities")]
    [Tooltip("SymbolWeight 에셋")]
    [SerializeField] private SymbolWeight reelProbability;

    [Header("Reward System")]
    [Tooltip("당첨 패턴 애니메이션 담당")]
    [SerializeField] private PatternAnimator patternAnimator;
    [Tooltip("패턴별 배율 정보가 담긴 ScriptableObject")]
    [SerializeField] private PatternRewardOdds patternOddsData;
    [Tooltip("심볼별 배율 정보가 담긴 ScriptableObject")]
    [SerializeField] private SymbolRewardOdds symbolOddsData;
    [Tooltip("배율 표시 UI")]
    [SerializeField] private ProbabilityScreen probabilityScreen;
    private CheckRewardPattern rewardChecker;

    private int[,] matrix;
    private SymbolWeightProcessor[] reelWeightProcessors;
    private Coroutine spinCoroutine;

    private void Awake()
    {
        bettingGold = 0;
        rewardGold = 0;
        _currentState = SlotMachineState.Idle; // 초기 상태는 이벤트 트리거 없이 직접 설정

        if (reels == null || reels.Length == 0)
        {
            Debug.LogError("슬롯머신에 릴이 없습니다.", this);
            return;
        }

        if (reelProbability == null)
        {
            Debug.LogError("공용 SymbolWeight 에셋(Reel Probability)이 할당되지 않았습니다.", this);
            return;
        }

        matrix = new int[3, reels.Length]; // 3행, reel 개수만큼의 열

        rewardChecker = new CheckRewardPattern();

        // 각 릴에 대한 SymbolWeightProcessor를 공통 확률 에셋으로 초기화
        reelWeightProcessors = new SymbolWeightProcessor[reels.Length];
        for (int i = 0; i < reels.Length; i++) reelWeightProcessors[i] = new SymbolWeightProcessor(reelProbability);
    }
    private void Start()
    {
        BettingGold = GameManager.instance.levelData._minGold;
    }
    public void Bet(bool isIncrease)
    {
        // 대기 상태가 아니면 베팅 불가
        if (CurrentState != SlotMachineState.Idle) return;

        int unitGold = GameManager.instance.levelData._unitGold;

        if (isIncrease == true)
        {
            if (GameManager.instance.levelData._maxGold < bettingGold + GameManager.instance.levelData._unitGold)
            {
                OnBetAttemptFailed?.Invoke();
                // 배팅가능한 최대치
                return;
            }

            BettingGold += unitGold;
        }
        else if (isIncrease == false)
        {
            if (GameManager.instance.levelData._minGold > bettingGold - GameManager.instance.levelData._unitGold)
            {
                OnBetAttemptFailed?.Invoke();
                // 배팅가능한 최소치
                return;
            }

            BettingGold -= unitGold;
        }
    }
    public void Spin()
    {
        // 대기 상태가 아니면 스핀 불가
        if (CurrentState != SlotMachineState.Idle) return;

        // 스핀 코루틴이 이미 실행 중이면 중복 실행 방지
        if (spinCoroutine != null) return;
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

        spinCoroutine = StartCoroutine(SpinSequence());
    }

    /// <summary>
    /// 스핀 시작부터 결과 처리까지의 전체 과정을 관리하는 메인 코루틴
    /// </summary>
    private IEnumerator SpinSequence()
    {
        CurrentState = SlotMachineState.Spinning;
        GameManager.instance.money.SpendGold(bettingGold);

        // 1. 모든 릴 회전 시작
        for (int i = 0; i < reels.Length; i++)
        {
            StartCoroutine(reels[i].StartSpin(reelWeightProcessors[i]));
        }

        // 2. 지정된 시간만큼 대기
        yield return new WaitForSeconds(spinTime);
        CurrentState = SlotMachineState.Stopping;

        // 3. 각 릴을 순차적으로 정지하고 결과 매트릭스 구성
        for (int i = 0; i < reels.Length; i++)
        {
            yield return StartCoroutine(reels[i].StopSpin(reels[i].row));

            int[] resultRow = reels[i].GetResultSymbols();
            ConvertMatrix(i, resultRow);

            yield return new WaitForSeconds(reelStopDelay);
        }

        // 4. 결과 처리 및 애니메이션 시작
        CurrentState = SlotMachineState.ShowingResults;
        yield return StartCoroutine(ProcessResults());

        // 5. 모든 과정 종료 후 초기화
        RewardGold = 0;
        CurrentState = SlotMachineState.Idle;
        spinCoroutine = null;
    }

    private bool IsActivatingState(SlotMachineState state)
    {
        return state != SlotMachineState.Idle;
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

    /// <summary>
    /// 스핀 종료 후 당첨 확인, 애니메이션, 보상 지급을 처리하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProcessResults()
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

            // 모든 라인 애니메이션 종료 후 총 배율 표시
            if (probabilityScreen != null)
            {
                probabilityScreen.ShowTotalOdds(totalOdds);
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
    }
}
