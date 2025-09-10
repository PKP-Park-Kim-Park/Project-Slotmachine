using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [Header("Machine Settings")] [SerializeField] private int machineLevel = 1;
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

    [Header("Outline Settings")]
    [SerializeField] private Outline outline;
    [SerializeField] private Color enabledColor = Color.yellow;
    [SerializeField] private Color disabledColor = Color.red;


    public int MachineLevel => machineLevel;
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

    private Money _money;
    private LevelData _levelData;

    public int MinBettingGold { get; private set; }
    public int MaxBettingGold { get; private set; }

    /// <summary>
    /// 레벨 데이터에 따라 베팅 금액 리밋 업뎃
    /// </summary>
    private void UpdateBettingLimits()
    {
        if (_levelData != null)
        {
            MinBettingGold = _levelData._minGold;
            MaxBettingGold = _levelData._maxGold;

            // 베팅 금액이 새로운 리밋을 벗어나는 경우 조정
            if (BettingGold < MinBettingGold)
            {
                BettingGold = MinBettingGold;
            }
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
    [Tooltip("고장 화면 UI")]
    [SerializeField] private BreakdownScreen[] breakdownScreens;

    private int[,] matrix;
    private SymbolWeightProcessor[] reelWeightProcessors;
    private Coroutine spinCoroutine;
    
    // 배율 데이터의 런타임 복사본
    private Dictionary<Patterns, float> patternOddsRuntimeCopy;
    private Dictionary<Symbols, float> symbolOddsRuntimeCopy;

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

        // 런타임 데이터 초기화
        ResetToDefaults();

        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }
        if (outline != null) {
            outline.enabled = false;
        }

        if (breakdownScreens == null || breakdownScreens.Length == 0)
        {
            breakdownScreens = GetComponentsInChildren<BreakdownScreen>();
        }
    }

    private void Start()
    {
        if (GameManager.instance != null)
        {
            // GameManager에 의존성 주입 요청
            GameManager.instance.InitializeSlotMachine(this);
            // GameManager의 세션 리셋 이벤트에 초기화 메서드 구독
            GameManager.instance.OnResetSession += ResetToDefaults;
        }
    }

    /// <summary>
    /// 모든 런타임 배율과 확률 데이터를 원본 ScriptableObject 값으로 리셋합니다.
    /// </summary>
    public void ResetToDefaults()
    {
        Debug.Log("슬롯머신 데이터를 기본값으로 초기화합니다.");

        // 1. 패턴 및 심볼 '보상 배율'을 원본 SO에서 다시 복사
        patternOddsRuntimeCopy = new Dictionary<Patterns, float>();
        foreach (var entry in patternOddsData.rewardPatterns)
        {
            patternOddsRuntimeCopy.Add(entry.patternType, entry.rewardOddsMultiplier);
        }

        symbolOddsRuntimeCopy = new Dictionary<Symbols, float>();
        foreach (var entry in symbolOddsData.rewardSymbols)
        {
            symbolOddsRuntimeCopy.Add(entry.symbolType, entry.rewardOddsMultiplier);
        }

        // 2. 릴의 '심볼 출현 확률'을 원본 SO를 사용해 재생성
        reelWeightProcessors = new SymbolWeightProcessor[reels.Length];
        for (int i = 0; i < reels.Length; i++) reelWeightProcessors[i] = new SymbolWeightProcessor(reelProbability);
    }

    public void Initialize(Money money, LevelData levelData)
    {
        _money = money;
        _levelData = levelData;

        if (_levelData != null)
        {
            _levelData.OnLevelChanged += UpdateBettingLimits;
            UpdateBettingLimits(); // 초기 한도 설정
            BettingGold = MinBettingGold;
            
            // 레벨업 이벤트에 아웃라인 업데이트 연결
            GameManager.instance.levelManager.OnLevelUp += (level) => UpdateOutlineColor();
            // 슬롯머신 활성화/비활성화 이벤트에 아웃라인 업데이트 연결
            OnActivationStart += UpdateOutlineColor;
            OnActivationEnd += UpdateOutlineColor;
            UpdateOutlineColor(); // 초기 색상 설정
        }
    }

    private void OnDestroy()
    {
        if (_levelData != null)
        {
            if (GameManager.instance != null)
            {
                // GameManager 이벤트 구독 해제
                GameManager.instance.OnResetSession -= ResetToDefaults;
            }
            _levelData.OnLevelChanged -= UpdateBettingLimits;
            // 이벤트 구독 해제
            GameManager.instance.levelManager.OnLevelUp -= (level) => UpdateOutlineColor();
            OnActivationStart -= UpdateOutlineColor;
            OnActivationEnd -= UpdateOutlineColor;
        }
    }
    
    public void Bet(bool isIncrease)
    {
        // 대기 상태가 아니면 베팅 불가
        if (CurrentState != SlotMachineState.Idle) return;

        int unitGold = GameManager.instance.levelData._unitGold;

        if (isIncrease == true)
        {
            // 다음 베팅 금액이 최대치를 초과하는지 확인
            if (bettingGold + unitGold > MaxBettingGold)
            {
                OnBetAttemptFailed?.Invoke();
                // 최대치로 설정하고 더 이상 진행하지 않음
                BettingGold = MaxBettingGold;
                return;
            }

            BettingGold += unitGold;
        }
        else if (isIncrease == false)
        {
            // 다음 베팅 금액이 최소치 미만인지 확인
            if (bettingGold - unitGold < MinBettingGold)
            {
                OnBetAttemptFailed?.Invoke();
                // 최소치로 설정하고 더 이상 진행하지 않음
                BettingGold = MinBettingGold;
                return;
            }

            BettingGold -= unitGold;
        }
    }
    public void Spin()
    {
        // 대기 상태가 아니면 스핀 불가
        if (CurrentState != SlotMachineState.Idle) return;

        // 플레이어 레벨과 슬롯머신 레벨이 다르면 스핀 불가
        if (_levelData != null && _levelData._level != machineLevel)
        {
            Debug.Log($"레벨이 맞지 않아 슬롯머신을 사용할 수 없습니다. (플레이어 레벨: {_levelData._level}, 슬롯머신 레벨: {machineLevel})");
            return;
        }

        // 스핀 코루틴이 이미 실행 중이면 중복 실행 방지
        if (spinCoroutine != null) return;
        if (bettingGold < MinBettingGold)
        {
            // 배팅액 부족!
            return;
        }

        if (_money == null || _money._gold < bettingGold)
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
        _money?.SpendGold(bettingGold);

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
        float totalOdds = rewardChecker.CheckReward(matrix, patternOddsRuntimeCopy, symbolOddsRuntimeCopy);

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
            _money?.AddGold(rewardGold);
        }
        else
        {
            Debug.Log("당첨된 라인이 없습니다.");
        }
    }

    /// <summary>
    /// 슬롯머신의 아웃라인을 활성화
    /// </summary>
    public void EnableOutline()
    {
        if (outline != null)
        {
            UpdateOutlineColor();
            outline.enabled = true;
        }
    }

    /// <summary>
    /// 슬롯머신의 아웃라인을 비활성화
    /// </summary>
    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    /// <summary>
    /// 상태에 따라 아웃라인 색상을 업데이트
    /// </summary>
    public void UpdateOutlineColor()
    {
        if (outline == null) return;

        bool isInteractable = (_levelData._level == machineLevel) && !IsActivating;
        outline.OutlineColor = isInteractable ? enabledColor : disabledColor;

        // 모든 BreakdownScreen의 상태를 업데이트합니다.
        if (breakdownScreens != null)
        {
            foreach (var screen in breakdownScreens)
            {
                if (_levelData._level != machineLevel) screen.Show();
                else screen.Hide();
            }
        }
    }
}
