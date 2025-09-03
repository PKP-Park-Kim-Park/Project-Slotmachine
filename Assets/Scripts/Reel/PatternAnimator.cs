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

    [Header("Pop Effect")]
    [Tooltip("당첨 심볼이 커지는 배율")]
    [SerializeField] private float popScale = 1.2f;

    [Tooltip("당첨 심볼이 커졌다가 돌아오는 총 애니메이션 시간")]
    [SerializeField] private float popDuration = 0.2f;

    [Header("Particle Effect")]
    [Tooltip("각 당첨 라인에 표시될 파티클 프리팹")]
    [SerializeField] private GameObject winningParticlePrefab;

    // 오브젝트 풀링을 위한 리스트와 코루틴 관리 변수
    private readonly List<GameObject> borderPool = new List<GameObject>();
    private int poolIndex = 0;
    private Coroutine animationCoroutine;
    public bool IsAnimating { get{ return animationCoroutine != null; }}

    private const int numColumns = 5;

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
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // 기존 테두리 정리
        ClearBorders();
        animationCoroutine = StartCoroutine(AnimateSeq(winningLines));
    }

    private IEnumerator AnimateSeq(List<WinningLine> winningLines)
    {
        // 각 당첨 라인을 순서대로 보여줌
        foreach (var line in winningLines)
        {
            List<Transform> transformsToAnimate = new List<Transform>();

            // 현재 라인의 심볼에 테두리 표시
            foreach (var coord in line.Coordinates)
            {
                // coord.x == 행(row), coord.y == 열(column)
                if (IsValidCoordinate(coord.x, coord.y))
                {
                    ShowBorderAt(coord.x, coord.y);
                    int index = coord.x * numColumns + coord.y;
                    transformsToAnimate.Add(slotPositions[index]);
                }
            }

            // 현재 라인의 심볼들 위에 파티클 효과 재생
            if (winningParticlePrefab != null)
            {
                foreach (var coord in line.Coordinates)
                {
                    if (IsValidCoordinate(coord.x, coord.y))
                    {
                        int index = coord.x * numColumns + coord.y;
                        Instantiate(winningParticlePrefab, slotPositions[index]);
                    }
                }
            }

            // 튀나오는 애니메이션
            if (transformsToAnimate.Count > 0)
            {
                yield return StartCoroutine(AnimatePop(transformsToAnimate, popScale, popDuration / 2f));
            }

            yield return new WaitForSeconds(animationDelay);

            // 튀들어가는 애니메이션
            if (transformsToAnimate.Count > 0)
            {
                yield return StartCoroutine(AnimatePop(transformsToAnimate, 1.0f, popDuration / 2f));
            }

            // 현재 테두리 제거
            ClearBorders();
        }

        animationCoroutine = null; // 코루틴 완료
    }

    private IEnumerator AnimatePop(List<Transform> targets, float targetScale, float duration)
    {
        if (targets.Count == 0 || duration <= 0)
        {
            yield break;
        }

        List<Vector3> originalScales = new List<Vector3>();
        foreach (var t in targets)
        {
            originalScales.Add(t.localScale);
        }
        Vector3 finalScale = Vector3.one * targetScale;

        float timer = 0f;
        while (timer < duration)
        {
            float progress = timer / duration;
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].localScale = Vector3.Lerp(originalScales[i], finalScale, progress);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        foreach (var t in targets)
        {
            t.localScale = finalScale;
        }
    }

    // 지정된 위치에 풀에서 테두리를 가져와 표시
    private void ShowBorderAt(int row, int col)
    {
        if (poolIndex >= borderPool.Count) return; // 풀이 부족한 경우 방지

        int index = row * numColumns + col;
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

        int index = row * numColumns + col;
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