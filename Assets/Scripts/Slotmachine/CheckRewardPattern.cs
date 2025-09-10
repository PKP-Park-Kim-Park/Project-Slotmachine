using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckRewardPattern
{
    private int[,] matrix = new int[3, 5]; // 가져온 배열 결과값
    private const int ROW = 3;
    private const int COLUMN = 5;
    private float totalOdds;

    private Dictionary<Symbols, float> symbolOdds = new Dictionary<Symbols, float>(); // 심볼 배율
    private Dictionary<Patterns, float> patternOdds = new Dictionary<Patterns, float>(); // 패턴 배율

    private List<WinningLine> winningLines = new List<WinningLine>();
    public List<WinningLine> WinningLines { get { return winningLines; } }

    public float CheckReward(int[,] _matrix, Dictionary<Patterns, float> _patternOdds, Dictionary<Symbols, float> _symbolOdds)
    {
        totalOdds = 0;
        
        winningLines.Clear();

        this.patternOdds = _patternOdds;
        this.symbolOdds = _symbolOdds;

        matrix = _matrix;

        CalculatePattern();

        return totalOdds;
    }

    private void CalculatePattern()
    {
        if (IsJackpot())
        {
            // 잭팟 보상 실행
            CheckVertical();
            CheckHorizontal();
            CheckUp();
            CheckDown();
            CheckEyes();
            CheckJackpot();
        }
        else
        {
            CheckVertical();
            CheckHorizontal();
            CheckLowerDiagonal();
            CheckUpperDiagonal();
            CheckZig();
            CheckZag();
            CheckUp();
            CheckDown();
            CheckEyes();
        }
    }

    private void CheckHorizontal()
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

            // 당첨 라인이 없으면 다음 행으로 넘어감
            if (highCount < 3)
            {
                continue;
            }

            // highCount 값에 따라 다른 패턴과 배율을 적용
            Patterns winningPattern;
            switch (highCount)
            {
                case 3: winningPattern = Patterns.Column_3; break;
                case 4: winningPattern = Patterns.Column_4; break;
                case 5: winningPattern = Patterns.Column_5; break;
                default: continue; // 혹시 모를 예외 상황 방지
            }

            List<Vector2Int> coords = new List<Vector2Int>();
            for (int i = 0; i < highCount; i++)
            {
                coords.Add(new Vector2Int(y, highCountStartX + i));
            }
            
            float odds = CalculateReward(winningPattern, (Symbols)highSymbolNum);
            winningLines.Add(new WinningLine(coords, winningPattern, odds));
        }
    }

    private void CheckVertical()
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
                float odds = CalculateReward(Patterns.Row_3, (Symbols)symbolNum);
                List<Vector2Int> coords = new List<Vector2Int>();
                for (int i = 0; i < ROW; i++)
                {
                    coords.Add(new Vector2Int(i, x));
                }
                winningLines.Add(new WinningLine(coords, Patterns.Row_3, odds));
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
                float odds = CalculateReward(Patterns.LowerDiagonal, (Symbols)symbolNum);
                List<Vector2Int> coords = new List<Vector2Int>();
                for (int i = 0; i < 3; i++)
                {
                    coords.Add(new Vector2Int(i, x + i));
                }
                winningLines.Add(new WinningLine(coords, Patterns.LowerDiagonal, odds));
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
                float odds = CalculateReward(Patterns.UpperDiagonal, (Symbols)symbolNum);
                List<Vector2Int> coords = new List<Vector2Int>();
                for (int i = 0; i < 3; i++)
                {
                    coords.Add(new Vector2Int(2 - i, x + i));
                }
                winningLines.Add(new WinningLine(coords, Patterns.UpperDiagonal, odds));
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
        float odds = CalculateReward(Patterns.Zig, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int> {
            new Vector2Int(2, 0), new Vector2Int(1, 1), new Vector2Int(0, 2),
            new Vector2Int(1, 3), new Vector2Int(2, 4)
        };
        winningLines.Add(new WinningLine(coords, Patterns.Zig, odds));
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
        float odds = CalculateReward(Patterns.Zag, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int> {
            new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2),
            new Vector2Int(1, 3), new Vector2Int(0, 4)
        };
        winningLines.Add(new WinningLine(coords, Patterns.Zag, odds));
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
        float odds = CalculateReward(Patterns.Up, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int> {
            new Vector2Int(2, 0), new Vector2Int(1, 1), new Vector2Int(0, 2), new Vector2Int(1, 3),
            new Vector2Int(2, 4), new Vector2Int(2, 1), new Vector2Int(2, 2), new Vector2Int(2, 3)
        };
        winningLines.Add(new WinningLine(coords, Patterns.Up, odds));
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
        float odds = CalculateReward(Patterns.Down, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int> {
            new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2), new Vector2Int(1, 3),
            new Vector2Int(0, 4), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3)
        };
        winningLines.Add(new WinningLine(coords, Patterns.Down, odds));
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
        float odds = CalculateReward(Patterns.Eyes, (Symbols)symbolNum);
        List<Vector2Int> coords = new List<Vector2Int> {
            new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1),
            new Vector2Int(0, 2), new Vector2Int(2, 2), new Vector2Int(0, 3), new Vector2Int(1, 3),
            new Vector2Int(2, 3), new Vector2Int(1, 4)
        };
        winningLines.Add(new WinningLine(coords, Patterns.Eyes, odds));
    }

    private void CheckJackpot()
    {
        int symbolNum = matrix[0, 0];
        float odds = CalculateReward(Patterns.Jackpot, (Symbols)symbolNum);

        List<Vector2Int> coords = new List<Vector2Int>();
        for (int y = 0; y < ROW; y++)
        {
            for (int x = 0; x < COLUMN; x++)
            {
                coords.Add(new Vector2Int(y, x));
            }
        }
        winningLines.Add(new WinningLine(coords, Patterns.Jackpot, odds));
    }

    /// <summary>
    /// 잭팟 조건인지 확인
    /// </summary>
    private bool IsJackpot()
    {
        int firstSymbol = matrix[0, 0];
        for (int y = 0; y < ROW; y++)
        {
            for (int x = 0; x < COLUMN; x++)
            {
                if (matrix[y, x] != firstSymbol)
                {
                    return false; // 하나라도 다르면 X
                }
            }
        }
        return true; // 모든 심볼 동일
    }
    
    private float CalculateReward(Patterns pattern, Symbols symbol)
    {
        float symbolOdd;
        float patternOdd;

        if (symbolOdds.ContainsKey(symbol) && patternOdds.ContainsKey(pattern))
        {
            symbolOdd = symbolOdds[symbol];
            patternOdd = patternOdds[pattern];

            float currentOdds = symbolOdd * patternOdd;
            totalOdds += currentOdds;
            return currentOdds;
        }
        return 0f;
    }
}
