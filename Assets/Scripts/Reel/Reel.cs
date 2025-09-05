using UnityEngine;
using System.Collections;

// Reel 클래스는 슬롯머신 릴의 동작을 관리
public class Reel : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("릴의 애니메이션을 담당하는 컴포넌트")]
    [SerializeField] private ReelAnimator reelAnimator;

    [Tooltip("릴에 표시될 실제 Symbol 컴포넌트들 (위->아래 순서)")]
    [SerializeField] private SymbolAnimator[] resultSymbolsUI;


    public bool isSpinning { get; private set; }
    public int[] row { get; private set; } = new int[7];

    // 확률에 따라 나온 심볼을 릴에 재배치
    public void RelocateSymbols(SymbolWeightProcessor processor)
    {
        if (processor == null) return;
        
        processor.NormalizeProbabilities(); // 확률을 100%로 보정

        for (int i = 0; i < row.Length; i++)
        {
            row[i] = processor.GetRandomWeightedSymbol();
        }
    }

    // 릴 회전 시작
    public IEnumerator StartSpin(SymbolWeightProcessor processor)
    {
        if (isSpinning)
        {
            yield break;
        }
        isSpinning = true;

        // 스핀 시작 시점에 전달받은 확률로 릴의 심볼들을 재배치
        RelocateSymbols(processor);

        // ReelAnimator에 start 넘김
        reelAnimator.StartSpin();
        yield return null;
    }

    // 회전 정지 후 최종 심볼 배열 설정
    public IEnumerator StopSpin(int[] finalRow)
    {
        isSpinning = false;
        // 최종 심볼 배열을 릴에 적용
        row = finalRow;

        // 최종 결과 심볼들을 UI에 설정
        Sprite[] finalStaticSprites = new Sprite[3];
        for (int i = 0; i < 3; i++)
        {
            Symbols symbolEnum = (Symbols)row[i + 2];
            resultSymbolsUI[i].SetSprites(SymbolManager.Instance.GetSprites(symbolEnum));
            finalStaticSprites[i] = SymbolManager.Instance.GetStaticSprite(symbolEnum);
        }

        yield return reelAnimator.StopSpin(finalStaticSprites);
    }

    // SlotMachine에서 최종 결과 가져가기
    public int[] GetResultSymbols()
    {
        // 중앙 3개 심볼(인덱스 2, 3, 4)을 반환합니다.
        return new int[] { row[2], row[3], row[4] };
    }
}