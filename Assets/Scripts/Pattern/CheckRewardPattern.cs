using UnityEngine;

/// <summary>
/// ���Ըӽ� ��� ��Ʈ������ �м��Ͽ� ���� ������ ����ϴ� Ŭ����.
/// </summary>
public class CheckRewardPattern
{
    private int[,] matrix;
    private float totalOdds;
    private SymbolData oddsTable;

    public CheckRewardPattern()
    {
        // �����ڿ��� SymbolData �ν��Ͻ��� �����մϴ�.
        this.oddsTable = new SymbolData();
    }

    public float CheckReward(int[,] inputMatrix)
    {
        if (inputMatrix == null || inputMatrix.GetLength(0) == 0 || inputMatrix.GetLength(1) == 0)
        {
            Debug.LogError("�Է� ��Ʈ������ ��ȿ���� �ʽ��ϴ�.");
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
        // ���� ���� Ȯ��
        CheckHorizontalPatterns();

        // ���� ���� Ȯ��
        CheckVerticalPatterns();

        // �밢�� ���� Ȯ��
        CheckDiagonalPatterns();
    }

    private void CalculateReward()
    {
        // �߰� ���ʽ� ������ �ʿ��� ��� ���⿡ �����մϴ�.
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
