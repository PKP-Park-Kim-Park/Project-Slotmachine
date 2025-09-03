using UnityEngine;
using System.Collections;

// Reel 클래스는 슬롯머신 릴의 동작을 관리
public class Reel : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("릴의 애니메이션을 담당하는 컴포넌트")]
    [SerializeField] private ReelAnimator reelAnimator;

    [Header("Probabilities")]
    [Tooltip("이 릴에서 사용할 심볼 확률 설정 에셋")]
    [SerializeField] private SymbolWeight probabilities;

    /// <summary>
    /// 특정 아이템 사용 시 적용법:
    /// private Reel reels;
    /// reels.Probabilities.SetProbability(Symbols.Cherry, 10f); <== 체리 확률 10% 증가
    /// </summary>
    // 확률 조작 테스트용임 테스트 끝날 시 없애시오.
    public SymbolWeight Probabilities => probabilities;

    public bool isSpinning { get; private set; }
    public int[] row { get; private set; } = new int[7];

    private void Awake()
    {
        if (probabilities == null)
        {
            Debug.LogError("SymbolWeight 에셋이 할당되지 않았습니다!", this);
        }
    }

    // 확률에 따라 나온 심볼을 릴에 재배치
    public void RelocateSymbols()
    {
        if (probabilities == null) return;

        probabilities.NormalizeProbabilities();

        for (int i = 0; i < row.Length; i++)
        {
            row[i] = probabilities.GetRandomWeightedSymbol();
        }
        // Debug.Log("릴 심볼이 확률에 따라 재배치되었습니다.");
        // probabilities.LogAllProbabilities(); // 릴 재배치 시에도 확률 로그 추가
    }

    // 릴 회전 시작
    public IEnumerator StartSpin()
    {
        if (isSpinning)
        {
            yield break;
        }
        isSpinning = true;

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
        // Debug.Log("릴 회전 중지!");

        // int -> Sprite 변환
        Sprite[] finalSprites = new Sprite[3];
        for (int i = 0; i < 3; i++)
        {
            // 중앙 3개 심볼(인덱스 2, 3, 4)을 결과로 사용
            finalSprites[i] = SymbolManager.Instance.GetSprite((Symbols)row[i + 2]);
        }

        yield return reelAnimator.StopSpin(finalSprites);
    }

    // SlotMachine에서 최종 결과 가져가기
    public int[] GetResultSymbols()
    {
        // 중앙 3개 심볼(인덱스 2, 3, 4)을 반환합니다.
        return new int[] { row[2], row[3], row[4] };
    }
}