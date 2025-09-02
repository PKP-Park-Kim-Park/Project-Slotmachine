using System.Collections.Generic;
using UnityEngine;

public class CheckRewardParttern
{
    private int[,] matrix = new int[3, 5]; // 가져온 배열 결과값
    private const int ROW = 3;
    private const int COLUMN = 5;
    private float totalOdds;

    private Dictionary<Symbols, float> symbolOdds = new Dictionary<Symbols, float>(); // 심볼 배율
    private Dictionary<Patterns, float> partternOdds = new Dictionary<Patterns, float>(); // 패턴 배율

    private List<WinningLine> winningLines = new List<WinningLine>();
    public List<WinningLine> WinningLines { get { return winningLines; } }

    public float CheckReward(int[,] _matrix, PatternRewardOdds sheet, SymbolRewardOdds sheet2)
    {
        totalOdds = 0;
        
        winningLines.Clear();
        partternOdds.Clear();
        symbolOdds.Clear();

        foreach (var entry in sheet.rewardPatterns)
        {
            partternOdds.Add(entry.patternType, entry.rewardOddsMultiplier);
        }

        foreach (var entry in sheet2.rewardSymbols)
        {
            symbolOdds.Add(entry.symbolType, entry.rewardOddsMultiplier);
        }

        matrix = _matrix;

        CalculatePattern();

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
            int highCountStartX = -1;

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
                    highCountStartX = x - count;
                }
            }

            // 요기 조건문이 좌표 리스트 더하기
            if (highCount > 0)
            {
                List<Vector2Int> coords = new List<Vector2Int>();
                for (int i = 0; i < highCount; i++)
                {
                    coords.Add(new Vector2Int(y, highCountStartX + i));
                }
                winningLines.Add(new WinningLine(coords));
            }
            
            // 결과 반환
            switch (highCount)
            {
                case 3:
                    // 총 배율 += (highSymbolNum)심볼 배율 * 패턴 배율
                    CalculateReward(Patterns.Column_3, (Symbols)highSymbolNum);
                    break;
                case 4:
                    // 총 배율 += (highSymbolNum)심볼 배율 * 패턴 배율
                    CalculateReward(Patterns.Column_4, (Symbols)highSymbolNum);
                    break;
                case 5:
                    // 총 배율 += (highSymbolNum)심볼 배율 * 패턴 배율
                    CalculateReward(Patterns.Column_5, (Symbols)highSymbolNum);
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
                CalculateReward(Patterns.Row_3, (Symbols)symbolNum);
                List<Vector2Int> coords = new List<Vector2Int>();
                for (int i = 0; i < ROW; i++)
                {
                    coords.Add(new Vector2Int(i, x));
                }
                winningLines.Add(new WinningLine(coords));
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
                CalculateReward(Patterns.LowerDiagonal, (Symbols)symbolNum);
                List<Vector2Int> coords = new List<Vector2Int>();
                for (int i = 0; i < 3; i++)
                {
                    coords.Add(new Vector2Int(i, x + i));
                }
                winningLines.Add(new WinningLine(coords));
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
                CalculateReward(Patterns.UpperDiagonal, (Symbols)symbolNum);
                List<Vector2Int> coords = new List<Vector2Int>();
                for (int i = 0; i < 3; i++)
                {
                    coords.Add(new Vector2Int(2 - i, x + i));
                }
                winningLines.Add(new WinningLine(coords));
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
        CalculateReward(Patterns.Zig, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int> {
            new Vector2Int(2, 0), new Vector2Int(1, 1), new Vector2Int(0, 2),
            new Vector2Int(1, 3), new Vector2Int(2, 4)
        };
        winningLines.Add(new WinningLine(coords));
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
        CalculateReward(Patterns.Zag, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int> {
            new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2),
            new Vector2Int(1, 3), new Vector2Int(0, 4)
        };
        winningLines.Add(new WinningLine(coords));
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
        CalculateReward(Patterns.Up, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int> {
            new Vector2Int(2, 0), new Vector2Int(1, 1), new Vector2Int(0, 2), new Vector2Int(1, 3),
            new Vector2Int(2, 4), new Vector2Int(2, 1), new Vector2Int(2, 2), new Vector2Int(2, 3)
        };
        winningLines.Add(new WinningLine(coords));
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
        CalculateReward(Patterns.Down, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int> {
            new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2), new Vector2Int(1, 3),
            new Vector2Int(0, 4), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3)
        };
        winningLines.Add(new WinningLine(coords));
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
        CalculateReward(Patterns.Eyes, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int> {
            new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1),
            new Vector2Int(0, 2), new Vector2Int(2, 2), new Vector2Int(0, 3), new Vector2Int(1, 3),
            new Vector2Int(2, 3), new Vector2Int(1, 4)
        };
        winningLines.Add(new WinningLine(coords));
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

        CalculateReward(Patterns.Jackpot, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int>();
        for (int y = 0; y < ROW; y++) for (int x = 0; x < COLUMN; x++) coords.Add(new Vector2Int(y, x));
        winningLines.Add(new WinningLine(coords));
    }

    private void CalculateReward(Patterns pattern, Symbols symbol)
    {
        float symbolOdd;
        float patternOdd;

        if(symbolOdds.ContainsKey(symbol) && partternOdds.ContainsKey(pattern))
        {
            symbolOdd = symbolOdds[symbol];
            patternOdd = partternOdds[pattern];

            totalOdds += symbolOdd * patternOdd;
        }
    }
}
