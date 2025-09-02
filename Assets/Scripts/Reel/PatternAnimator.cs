using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternAnimator : MonoBehaviour
{
    [Tooltip("당첨 심볼 주위에 표시될 테두리 UI 프리팹")]
    [SerializeField] private GameObject borderPrefab;

    [Tooltip("슬롯들의 UI 위치를 담는 1차원 Transform 배열 (3x5=15개)")]
    [SerializeField] private Transform[] slotPositions = new Transform[15];

    [Tooltip("각 당첨 라인 애니메이션 사이의 딜레이 (초)")]
    [SerializeField] private float animationDelay = 0.5f;

    [Header("Particle Effect")]
    [Tooltip("각 당첨 라인에 표시될 파티클 프리팹")]
    [SerializeField] private GameObject winningParticlePrefab;

    // 오브젝트 풀링을 위한 리스트와 코루틴 관리 변수
    private readonly List<GameObject> borderPool = new List<GameObject>();
    private int poolIndex = 0;
    private Coroutine m_AnimationCoroutine;
    
    private const int k_NumColumns = 5;

    private void Awake()
    {
        InitializePool();
    }

    // 미리 테두리 오브젝트를 생성하여 풀에 저장
    private void InitializePool()
    {
        if (borderPrefab == null) return;

        // 최대 15개의 테두리가 동시에 필요할 수 있음
        for (int i = 0; i < 15; i++)
        {
            GameObject border = Instantiate(borderPrefab, transform);
            border.SetActive(false);
            borderPool.Add(border);
        }
    }

    /// <summary>
    /// 외부에서 호출 하쇼. 당첨 애니매이션 시작
    /// </summary>
    public void AnimateWinning(List<WinningLine> winningLines)
    {
        // 기존 애니메이션이 실행 중이면 중지
        if (m_AnimationCoroutine != null)
        {
            StopCoroutine(m_AnimationCoroutine);
        }

        // 기존 테두리 정리
        ClearBorders();
        m_AnimationCoroutine = StartCoroutine(AnimateSeq(winningLines));
    }

    private IEnumerator AnimateSeq(List<WinningLine> winningLines)
    {
        // 각 당첨 라인을 순서대로 보여줌
        foreach (var line in winningLines)
        {
            // 현재 라인의 심볼들에 테두리 표시
            foreach (var coord in line.Coordinates)
            {
                // coord.x == 행(row), coord.y == 열(column)
                if (IsValidCoordinate(coord.x, coord.y))
                {
                    ShowBorderAt(coord.x, coord.y);
                }
            }

            // 현재 라인의 심볼들 위에 파티클 효과 재생
            if (winningParticlePrefab != null)
            {
                foreach (var coord in line.Coordinates)
                {
                    if (IsValidCoordinate(coord.x, coord.y))
                    {
                        int index = coord.x * k_NumColumns + coord.y;
                        Instantiate(winningParticlePrefab, slotPositions[index]);
                    }
                }
            }

            yield return new WaitForSeconds(animationDelay);

            // 현재 테두리 제거
            ClearBorders();
        }

        ShowAll(winningLines);
        m_AnimationCoroutine = null; // 코루틴 완료
    }

    // 모든 당첨 심볼을 한 번에 표시하는 함수
    private void ShowAll(List<WinningLine> winningLines)
    {
        ClearBorders();
        HashSet<Vector2Int> uniqueCoords = new HashSet<Vector2Int>();
        foreach (var line in winningLines)
        {
            foreach (var coord in line.Coordinates)
            {
                uniqueCoords.Add(coord);
            }
        }

        foreach (var coord in uniqueCoords)
        {
            if (IsValidCoordinate(coord.x, coord.y))
            {
                ShowBorderAt(coord.x, coord.y);
            }
        }
    }

    // 지정된 위치에 풀에서 테두리를 가져와 표시
    private void ShowBorderAt(int row, int col)
    {
        if (poolIndex >= borderPool.Count) return; // 풀이 부족한 경우 방지

        int index = row * k_NumColumns + col;
        GameObject border = borderPool[poolIndex];
        Transform slotTransform = slotPositions[index];

        border.transform.SetParent(slotTransform, false);
        border.SetActive(true);

        poolIndex++;
    }

    /// <summary>
    /// 표시된 모든 테두리를 풀로 반환
    /// </summary>
    public void ClearBorders()
    {
        for (int i = 0; i < poolIndex; i++)
        {
            borderPool[i].SetActive(false);
            borderPool[i].transform.SetParent(transform, false); // 다시 원래 부모로
        }
        poolIndex = 0;
    }

    // 좌표 유효성 검사
    private bool IsValidCoordinate(int row, int col)
    {
        if (slotPositions == null || slotPositions.Length != 15) return false;

        int index = row * k_NumColumns + col;
        return index >= 0 && index < slotPositions.Length && slotPositions[index] != null;
    }
}


/*
 체리 2배

 세로 5개 x1 =5 10점
 가로 3개 x3 =9 18점
 대각 6개 x2 =12 24점
 지그재그 2개 x3 = 6 12점
 업 1개 x4 = 4 8점
 다운 1개 x4 =4 8점
 눈 1개 x5 =5 10점
 잭팟 1개 x6 =6 12점

 total = 102
*/