using UnityEngine;

/// <summary>
/// 슬롯머신 결과 매트릭스를 분석하여 보상 배당률을 계산하는 클래스.
/// </summary>
public class CheckRewardPattern
{
    private int[,] matrix;
    private float totalOdds;
    private SymbolData oddsTable;

    public CheckRewardPattern()
    {
        // 생성자에서 SymbolData 인스턴스를 생성합니다.
        this.oddsTable = new SymbolData();
    }

    public float CheckReward(int[,] inputMatrix)
    {
        if (inputMatrix == null || inputMatrix.GetLength(0) == 0 || inputMatrix.GetLength(1) == 0)
        {
            Debug.LogError("입력 매트릭스가 유효하지 않습니다.");
            return 0f;
        }

        this.matrix = inputMatrix;
        this.totalOdds = 0f;

        CalculatePattern();
        CalculateReward();

        return this.totalOdds;
    }

    private void CalculatePattern()
    {
        // 가로 패턴 확인
        CheckHorizontalPatterns();

        // 세로 패턴 확인
        CheckVerticalPatterns();

        // 대각선 패턴 확인
        CheckDiagonalPatterns();
    }

    private void CalculateReward()
    {
        // 추가 보너스 로직이 필요할 경우 여기에 구현합니다.
    }

    private void CheckHorizontalPatterns()
    {
    }

    private void CheckVerticalPatterns()
    {
    }

    private void CheckDiagonalPatterns()
    {
    }
}
