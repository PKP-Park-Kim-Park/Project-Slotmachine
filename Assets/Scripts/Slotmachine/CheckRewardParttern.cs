using System.Collections.Generic;

public class CheckRewardParttern
{
    private int[,] matrix = new int[3, 5]; // 가져온 배열 결과값
    private const int ROW = 3;
    private const int COLUMN = 5;
    private float totalOdds; // 총 배율

    private Dictionary<Symbols, float> symbolOdds = new Dictionary<Symbols, float>(); // 심볼 배율
    private Dictionary<string, float> parttern = new Dictionary<string, float>(); // 패턴 배율

    public float CheckReward(int[,] _matrix)
    {
        matrix = _matrix;

        CalculatePattern();
        totalOdds = CalculateReward();

        return totalOdds;
    }

    private void CalculatePattern()
    {
        CheckColumn();
        CheckRow();
        CheckLowerDiagonal();
        CheckUpperDiagonal();
        CheckZig();
        CheckZag();
        CheckUp();
        CheckDown();
        CheckEyes();
        CheckJackpot();
    }

    private void CheckColumn()
    {
        // 가로
        for (int y = 0; y < ROW; y++)
        {
            int count = 0;
            int symbolNum = 0;

            int highCount = 0;
            int highSymbolNum = 0;

            for (int x = 0; x < COLUMN; x++)
            {
                // 가로 시작 심볼 체크
                if (symbolNum == 0)
                {
                    symbolNum = matrix[y, x];
                    continue;
                }

                // 다음 자리 심볼이 다르면 초기화
                if (symbolNum != matrix[y, x])
                {
                    symbolNum = matrix[y, x];
                    count = 0;
                    continue;
                }

                // 아니면 카운트 추가
                count++;

                // 가로 한줄의 최고의 결과 저장
                if(count + 1 >= 3)
                {
                    highCount = count + 1;
                    highSymbolNum = symbolNum;
                }
            }

            // 결과 반환
            switch (highCount)
            {
                case 3:
                    // 총 배율 += (highSymbolNum)심볼 배율 * 패턴 배율
                    break;
                case 4:
                    // 총 배율 += (highSymbolNum)심볼 배율 * 패턴 배율
                    break;
                case 5:
                    // 총 배율 += (highSymbolNum)심볼 배율 * 패턴 배율
                    break;
                default:
                    break;
            }
        }
    }

    private void CheckRow()
    {
        // 세로
        for (int x = 0; x < COLUMN; x++)
        {
            int count = 0;
            int symbolNum = 0;

            for (int y = 0; y < ROW; y++)
            {
                // 세로 시작 심볼 체크
                if (symbolNum == 0)
                {
                    symbolNum = matrix[y, x];
                    continue;
                }

                // 다음 자리 심볼이 다르면 초기화
                if (symbolNum != matrix[y, x])
                {
                    break;
                }

                // 아니면 카운트 추가
                count++;
            }

            // 결과 반환
            if (count + 1 == 3)
            {
                // 총 배율 += 심볼 배율 * 패턴 배율
            }
        }
    }

    private void CheckLowerDiagonal()
    {
        // 아래로 대각선
        for (int x = 0; x + 2 < COLUMN; x++)
        {
            int count = 0;
            int symbolNum = matrix[0, x];

            for (int i = 0; i < ROW; i++)
            {
                if (symbolNum == matrix[i, x + i])
                {
                    count++;
                }
            }

            // 결과 반환
            if (count == 3)
            {
                // 총 배율 += 심볼 배율 * 패턴 배율
            }
        }
    }

    private void CheckUpperDiagonal()
    {
        // 위로 대각선
        for (int x = 0; x + 2 < COLUMN; x++)
        {
            int count = 0;
            int symbolNum = matrix[2, x];

            for (int i = 0; i < ROW; i++)
            {
                if (symbolNum == matrix[2 - i, x + i])
                {
                    count++;
                }
            }

            // 결과 반환
            if (count == 3)
            {
                // 총 배율 += 심볼 배율 * 패턴 배율
            }
        }
    }

    private void CheckZig()
    {
        int symbolNum = matrix[2, 0];

        if (symbolNum != matrix[1, 1] ||
           symbolNum != matrix[0, 2] ||
           symbolNum != matrix[1, 3] ||
           symbolNum != matrix[2, 4])
        {
            return;
        }

        // 총 배율 += 심볼 배율 * 패턴 배율
    }

    private void CheckZag()
    {
        int symbolNum = matrix[0, 0];

        if (symbolNum != matrix[1, 1] ||
           symbolNum != matrix[2, 2] ||
           symbolNum != matrix[1, 3] ||
           symbolNum != matrix[0, 4])
        {
            return;
        }

        // 총 배율 += 심볼 배율 * 패턴 배율
    }

    private void CheckUp()
    {
        int symbolNum = matrix[2, 0];

        if (symbolNum != matrix[1, 1] ||
           symbolNum != matrix[0, 2] ||
           symbolNum != matrix[1, 3] ||
           symbolNum != matrix[2, 4] ||
           symbolNum != matrix[2, 1] ||
           symbolNum != matrix[2, 2] ||
           symbolNum != matrix[2, 3])
        {
            return;
        }

        // 총 배율 += 심볼 배율 * 패턴 배율
    }

    private void CheckDown()
    {
        int symbolNum = matrix[0, 0];

        if (symbolNum != matrix[1, 1] ||
           symbolNum != matrix[2, 2] ||
           symbolNum != matrix[1, 3] ||
           symbolNum != matrix[0, 4] ||
           symbolNum != matrix[0, 1] ||
           symbolNum != matrix[0, 2] ||
           symbolNum != matrix[0, 3])
        {
            return;
        }


        // 총 배율 += 심볼 배율 * 패턴 배율
    }

    private void CheckEyes()
    {
        int symbolNum = matrix[1, 0];

        if (symbolNum != matrix[0, 1] ||
           symbolNum != matrix[1, 1] ||
           symbolNum != matrix[2, 1] ||
           symbolNum != matrix[0, 2] ||
           symbolNum != matrix[2, 2] ||
           symbolNum != matrix[0, 3] ||
           symbolNum != matrix[1, 3] ||
           symbolNum != matrix[2, 3] ||
           symbolNum != matrix[1, 4])
        {
            return;
        }

        // 총 배율 += 심볼 배율 * 패턴 배율
    }

    private void CheckJackpot()
    {
        int symbolNum = matrix[0, 0];

        for (int y = 0; y < ROW; y++)
        {
            for (int x = 0; x < COLUMN; x++)
            {
                // 심볼 체크
                if (symbolNum != matrix[y, x])
                {
                    return;
                }
            }
        }

        // 총 배율 += 심볼 배율 * 패턴 배율
    }

    private float CalculateReward()
    {
        float Odds = 0f;
        return Odds;
    }
}
